using AutoMapper;
using Maliwan.Application.Models.IdentityContext.Responses;
using Maliwan.Application.Models.IdentityContext;
using Maliwan.Application.Services.Interfaces;
using Maliwan.Domain.Core.Enums;
using Maliwan.Domain.Core.Messages.CommonMessages.Notifications;
using Maliwan.Domain.Core.Options;
using Maliwan.Domain.IdentityContext.Entities;
using Maliwan.Domain.IdentityContext.Interfaces.Repositories;
using Maliwan.Domain.IdentityContext.Interfaces.Validators;
using Maliwan.Domain.IdentityContext.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Maliwan.Domain.Core.Extensions;

namespace Maliwan.Application.Services;

public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    //private readonly ILog _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer<CommonMessages> _localizer;
    private readonly UserManager<User> _userManager;
    private readonly IOptions<JwtTokenOptions> _jwtTokenOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    //private readonly IMailService _mailService;

    public UserService(IMediator mediator, IMapper mapper, /*ILog log,*/ IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<CommonMessages> localizer, UserManager<User> userManager,
        IOptions<JwtTokenOptions> jwtTokenOptions, IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenValidator refreshTokenValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        //_log = log;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _userManager = userManager;
        _jwtTokenOptions = jwtTokenOptions;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenValidator = refreshTokenValidator;
    }

    public Guid? UserId
    {
        get
        {

            try
            {
                if (Guid.TryParse(_httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                {
                    return id;
                }
            }
            catch (Exception e)
            {
            }

            return default;
        }
    }

    #region Consts

    public const string EmailConfirmationSubject = "Confirmação de e-mail";
    public const string EmailConfirmationBody = @"<p>Olá #Name</p>
<br/>
<p>Para confirmar o seu e-mail, o código abaixo no APP.</p>
<br/>
<h3 style=""text-align:center"">#EmailConfirmationToken</h3>";

    #endregion

    public async Task<User> CurrentUserAsync()
    {
        return UserId != null
            ? await _userManager.FindByIdAsync(UserId.ToString())
            : null;
    }

    public async Task<User> FindByUserName(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<bool> CurrentUserHasRole(string roleName)
    {
        var user = await CurrentUserAsync();
        if (user is null)
            return false;

        return _httpContextAccessor.HttpContext?.User.IsInRole(roleName) ?? false;
    }

    public async Task<bool> CurrentUserHasRole(RoleEnum role)
    {
        return await CurrentUserHasRole(role.ToString());
    }

    public async Task<bool> CurrentUserHasRoleAnyAsync(Func<RoleEnum?, bool> predicate)
    {
        var user = await CurrentUserAsync();
        if (user is null)
            return false;

        var userRoles = _httpContextAccessor.HttpContext?.User?
            .Claims?.Where(c => c.Type == ClaimTypes.Role && c.Value.ValueExistsInEnum<RoleEnum>())
            ?.Select(c => c.Value.StringToEnum<RoleEnum>())
            ?.Distinct();

        return userRoles != null && userRoles.Any(predicate);
    }

    public async Task<bool> HasRole(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> HasRole(Guid userId, RoleEnum role)
    {
        return await HasRole(userId, role.ToString());
    }

    public async Task<bool> SendEmailConfirmationAsync(Guid userId)
    {
        try
        {
            throw new NotImplementedException();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.EmailConfirmed)
            {
                return false;
            }

            var body = EmailConfirmationBody
                .Replace("#Name", user.Name)
                .Replace("#EmailConfirmationToken", user.EmailConfirmationToken);

            //return await _mailService.SendAsync(new SendMailResquest(user.Email, EmailConfirmationSubject, body));
        }
        catch (Exception e)
        {
        }

        return false;
    }

    #region Jwt

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[new Random().Next(32, 256)];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken(
            userId,
            Convert.ToBase64String(randomNumber),
            _jwtTokenOptions.Value.RefreshTokenValidUntil);
    }

    private async Task<ClaimsPrincipal?> GetPrincipalFromExpiredTokenAsync(string encodedJwt)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtTokenOptions.Value.IssuerSigningKey,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(encodedJwt, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(_jwtTokenOptions.Value.JwtSecurityAlgorithms,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.InvalidJwtToken),
                    _localizer.GetString(nameof(CommonMessages.InvalidJwtToken))));
                return default;
            }

            return principal;
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }

        return default;
    }

    public async Task<SignInResponseModel?> GetJwtAsync(User user)
    {
        try
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, await _jwtTokenOptions.Value.JtiGenerator()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtTokenOptions.Value.IssuedAt).ToString(), ClaimValueTypes.Integer64));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                userClaims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                userClaims.Add(new Claim("roles", role));
            }

            var jwt = new JwtSecurityToken(
                issuer: _jwtTokenOptions.Value.Issuer,
                audience: _jwtTokenOptions.Value.Audience,
                claims: userClaims,
                notBefore: _jwtTokenOptions.Value.NotBefore,
                expires: _jwtTokenOptions.Value.Expiration,
                signingCredentials: _jwtTokenOptions.Value.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = GenerateRefreshToken(user.Id);

            var retry = true;
            do
            {
                if (!await _refreshTokenValidator.IsValidAsync(refreshToken)
                    && _refreshTokenValidator.ValidationResult.Errors.All(error =>
                        error.ErrorCode == nameof(RefreshTokenValidator.TokenAlreadyExists)))
                {
                    refreshToken = GenerateRefreshToken(user.Id);
                }
                else
                {
                    retry = false;
                }
            } while (retry);

            if (!await _refreshTokenValidator.IsValidAsync(refreshToken))
            {
                foreach (var error in _refreshTokenValidator.ValidationResult.Errors)
                {
                    await _mediator.Publish(_mapper.Map<DomainNotification>(error));
                }
                return default;
            }

            await _refreshTokenRepository.InsertAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new SignInResponseModel(
                encodedJwt,
                _jwtTokenOptions.Value.ValidFor.TotalSeconds,
                refreshToken.Token,
                _jwtTokenOptions.Value.RefreshTokenValidForMore.TotalSeconds,
                _mapper.Map<UserModel>(user, act =>
                    act.AfterMap((entity, model) =>
                    {
                        model.Roles = roles.ToList();
                    })));
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }
        return default;
    }

    public async Task<SignInResponseModel?> RefreshTokenAsync(string encodedJwt, string refreshToken)
    {
        var userId = Guid.Empty;
        try
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtTokenOptions.Value.IssuerSigningKey,
                ValidateLifetime = true
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ValidateToken(encodedJwt, tokenValidationParameters, out var securityToken) != null)
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.JwtTokenIsStillValid),
                    _localizer.GetString(nameof(CommonMessages.JwtTokenIsStillValid))));
                return default;
            }

            var principal = await GetPrincipalFromExpiredTokenAsync(encodedJwt);
            Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            if (userId == null || userId == Guid.Empty)
            {
                return default;
            }

            if (!await _refreshTokenRepository.AnyAsync(e =>
                    e.UserId == userId
                    && e.Token == refreshToken
                    && e.ValidUntil > DateTime.UtcNow))
            {
                await _mediator.Publish(new DomainNotification(
                    nameof(CommonMessages.InvalidJwtRefreshToken),
                    _localizer.GetString(nameof(CommonMessages.InvalidJwtRefreshToken))));
                return default;
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await GetJwtAsync(user);
        }
        catch (Exception e)
        {
            //await _log.LogAsync(e, _httpContextAccessor.HttpContext?.TraceIdentifier);
            await _mediator.Publish(new DomainNotification(
                nameof(CommonMessages.InternalServerError),
                _localizer.GetString(nameof(CommonMessages.InternalServerError))));
        }
        finally
        {
            if (userId != Guid.Empty
            && await _refreshTokenRepository.AnyAsync(e =>
                e.UserId == userId
                && e.Token == refreshToken))
            {
                await _refreshTokenRepository.DeleteAsync(e =>
                    e.UserId == userId
                    && e.Token == refreshToken);

                await _refreshTokenRepository.SaveChangesAsync();
            }
        }
        return default;
    }

    #endregion
}