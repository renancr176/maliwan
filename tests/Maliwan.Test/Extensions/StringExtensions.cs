using System.Text.RegularExpressions;
using Bogus;
using Bogus.Extensions;

namespace Maliwan.Test.Extensions;

public static class StringExtensions
{
    public static string GetSku(this string name)
    {
        var Faker = new Faker();
        var normalizedName = Regex.Replace(Regex.Replace(name, @"[^a-zA-Z0-9 \s+]", ""), @"\s+", " ").RemoveDiacritics();
        return (normalizedName.Trim().Split(" ").Length > 1
                ? $"{normalizedName.Trim().Split(" ")[0].ToCharArray()[0].ToString()}{normalizedName.Trim().Split(" ")[1].ToCharArray()[0].ToString()}"
                : $"{Faker.PickRandom(normalizedName.Trim().ToCharArray()).ToString()}{Faker.PickRandom(normalizedName.Trim().ToCharArray()).ToString()}")
            .ToUpper();
    }
}