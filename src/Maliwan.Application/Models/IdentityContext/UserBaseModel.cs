using System.ComponentModel.DataAnnotations;

namespace Maliwan.Application.Models.IdentityContext;

public class UserBaseModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
}