using Maliwan.Domain.Core.Requests;
using Maliwan.Domain.MaliwanContext.Enums;

namespace Maliwan.Application.Models.MaliwanContext.Queries.Requests;

public class StockSearchRequest : PagedRequest
{
    public Guid? IdProduct { get; set; }
    public int? IdSize { get; set; }
    public int? IdColor { get; set; }
    public DateTime? InputDate { get; set; }
    public int? CurrentQuantityMin { get; set; }
    public int? CurrentQuantityMax { get; set; }
    public StockLevelEnum? StockLevel { get; set; }
    public bool? Active { get; set; }

    public StockSearchRequest()
    {
    }

    public StockSearchRequest(Guid? idProduct = null, int? idSize = null, int? idColor = null,
        DateTime? inputDate = null, int? currentQuantityMin = null,
        int? currentQuantityMax = null, StockLevelEnum? stockLevel = null, bool? active = null)
    {
        IdProduct = idProduct;
        IdSize = idSize;
        IdColor = idColor;
        InputDate = inputDate;
        CurrentQuantityMin = currentQuantityMin;
        CurrentQuantityMax = currentQuantityMax;
        StockLevel = stockLevel;
        Active = active;
    }
}