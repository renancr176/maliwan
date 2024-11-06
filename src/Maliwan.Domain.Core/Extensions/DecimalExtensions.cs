using System.Globalization;

namespace Maliwan.Domain.Core.Extensions;

public static class DecimalExtensions
{
    public static string ToCurrencyString(this decimal ammount, string language = "pt-BR")
    {
        return ammount.ToString("C", new CultureInfo(language));
    }
}