using Maliwan.Application.Models.MaliwanContext;
using Maliwan.Domain.Core.Messages;

namespace Maliwan.Application.Commands.MaliwanContext.OrderCommands;

public class CreateOrderCommand : Command<OrderModel?>
{
    public Guid IdCustomer { get; set; }
    public IEnumerable<ProductItem>? Items { get; set; }

    public CreateOrderCommand()
    {
    }

    public CreateOrderCommand(Guid idCustomer, IEnumerable<ProductItem> items)
    {
        IdCustomer = idCustomer;
        Items = items;
    }
}

public class ProductItem
{
    public Guid IdProduct { get; set; }
    public int Quantity { get; set; }

    public ProductItem()
    {
    }

    public ProductItem(Guid idProduct, int quantity)
    {
        IdProduct = idProduct;
        Quantity = quantity;
    }
}