using System.ComponentModel.DataAnnotations;

namespace Maliwan.Application.Models.IdentityContext;

public class UserModel : UserBaseModel
{
    public Guid Id { get; set; }
    [Required]
    public string UserName { get; set; }
    public string RememberPhrase { get; set; }
    public long UsedBytes { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}