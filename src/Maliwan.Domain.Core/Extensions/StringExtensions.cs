using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace Maliwan.Domain.Core.Extensions;

public static class StringExtensions
{
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Contains(";"))
        {
            var results = new List<bool>();
            foreach (var e in email.Split(";"))
            {
                results.Add(e.IsValidEmail());
            }

            return results.All(result => result);
        }

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static bool IsCpf(this string numero)
    {
        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf;
        string digito;
        int soma;
        int resto;
        numero = numero.Trim();
        numero = numero.Replace(".", "").Replace("-", "");
        if (numero.Length != 11)
            return false;
        tempCpf = numero.Substring(0, 9);
        soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = digito + resto.ToString();
        return numero.EndsWith(digito);
    }

    public static bool IsCnpj(this string numero)
    {
        int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma;
        int resto;
        string digito;
        string tempCnpj;
        numero = numero.Trim();
        numero = numero.Replace(".", "").Replace("-", "").Replace("/", "");
        if (numero.Length != 14)
            return false;
        tempCnpj = numero.Substring(0, 12);
        soma = 0;
        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
        resto = (soma % 11);
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = resto.ToString();
        tempCnpj = tempCnpj + digito;
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
        resto = (soma % 11);
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = digito + resto.ToString();
        return numero.EndsWith(digito);
    }

    public static bool IsUrl(this string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri);
    }

    public static bool IsValidJson(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        json = json.Trim();

        if ((json.StartsWith("{") && json.EndsWith("}")) || //For object
            (json.StartsWith("[") && json.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException) { }
            catch (Exception) { }
        }

        return false;
    }

    public static string Base64Encode(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static bool IsBase64Encoded(this string base64EncodedData)
    {
        try
        {
            var test = Base64Decode(base64EncodedData);
            return true;
        }
        catch (Exception)
        {
        }

        return false;
    }

    public static string HideCardNumber(this string numbers)
    {
        numbers = numbers.RemoveNonNumeric();
        var sb = new StringBuilder();
        sb.Append(numbers.Substring(0, 6));
        sb.Append(new String('*', (numbers.Length - 10)));
        sb.Append(numbers.Substring(numbers.Length - 4));
        return sb.ToString();
    }

    public static string RemoveNonNumeric(this string numbers)
    {
        return !string.IsNullOrEmpty(numbers)
            ? Regex.Replace(numbers, @"\D", "")
            : String.Empty;
    }

    public static string GenerateTokenString(this string text, int length = 6)
    {
        var builder = new StringBuilder();

        builder.Append(text);
        builder.Append(new string(Enumerable
            .Repeat(Guid.NewGuid().ToString().Replace("-", String.Empty), length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray()));

        return builder.ToString();
    }

    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string MaskCpfCnpj(this string documento)
    {
        if (!string.IsNullOrEmpty(documento) && !string.IsNullOrWhiteSpace(documento))
        {
            switch (documento.RemoveNonNumeric().Length)
            {
                case 11:
                    return Convert.ToUInt64(documento.RemoveNonNumeric()).ToString(@"000\.000\.000\-00");
                case 14:
                    return Convert.ToUInt64(documento.RemoveNonNumeric()).ToString(@"00\.000\.000\/0000\-00"); ;
            }
        }

        return documento;
    }

    public static bool ValueExistsInEnum<TEnum>(this string value) where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>().ToList();
        return enumValues != null && enumValues.Any(e => e.ToString().Trim().ToLower() == value.Trim().ToLower());
    }

    public static TEnum? StringToEnum<TEnum>(this string value) where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>().ToList();
        return enumValues != null ? enumValues.FirstOrDefault(e => e.ToString().Trim().ToLower() == value.Trim().ToLower()) : null;
    }

    public static void SaveToFile(this string content, string filename)
    {
        File.WriteAllBytes(filename, Convert.FromBase64String(content));
    }

    public static string? ToBase64(this string filePath)
    {
        if (File.Exists(filePath))
        {
            return Convert.ToBase64String(File.ReadAllBytes(filePath));
        }

        return null;
    }

    public static IEnumerable<String> SplitInParts(this string s, int partLength)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (partLength <= 0)
            throw new ArgumentException("Part length has to be positive.", nameof(partLength));

        for (var i = 0; i < s.Length; i += partLength)
            yield return s.Substring(i, Math.Min(partLength, s.Length - i));
    }

    public static bool IsValidHexColor(this string hexColor)
    {
        return Regex.IsMatch(hexColor, "^#(?:[0-9a-fA-F]{3}){1,2}$") ||
               Regex.IsMatch(hexColor, "^#(?:[0-9a-fA-F]{3,4}){1,2}$");
    }
}