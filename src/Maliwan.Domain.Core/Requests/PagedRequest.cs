using Maliwan.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Maliwan.Domain.Core.Requests;

public class PagedRequest
{
    [Range(1, int.MaxValue)]
    public int PageIndex { get; set; } = 1;
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; } = 50;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ConditionTypeEnum ConditionType { get; set; } = ConditionTypeEnum.And;
}