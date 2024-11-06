using Maliwan.Domain.Maliwan.Enums;

namespace Maliwan.Application.Models.MaliwanContext;

public class CustomerModel : EntityModel
{
    public string Name { get; set; }
    public string Document { get; set; }
    public CustomerTypeEnum Type { get; set; }
    public virtual IEnumerable<OrderModel> Orders { get; set; }
}