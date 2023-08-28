using System.Text;
using System.Text.RegularExpressions;

namespace IEM.Application.Models.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        public static string[] Split(this string str, string separator)
        {
            return str.Split(new[] { separator }, StringSplitOptions.None);
        }

        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str.Split(new[] { separator }, options);
        }

        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        public static string? RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                {
                    return str.Left(str.Length - postFix.Length);
                }
            }

            return str;
        }

        public static string UpperFirstCase(this string str)
        {
            return Uppercase(str, 1);
        }

        public static string Uppercase(this string input, int position = 1)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            if (position > input.Length)
            {
                return input;
            }
            return input.First().ToString().ToUpper() + string.Join("", input.Skip(position));
        }

        public static bool EqualsIgnoreCase(this string first, string second)
        {
            return string.Equals(first ?? string.Empty, second ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainIgnoreCase(this string first, string second)
        {
            first ??= string.Empty;
            second ??= string.Empty;
            return first.Contains(second, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int IndexOfIgnoreCase(this string first, string second)
        {
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
            {
                return -1;
            }
            return first.IndexOf(second, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithIgnoreCase(this string text, string value)
        {
            return text.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ReplaceIgnoreCase(this string text, string oldValue, string newValue)
        {
            return text.Replace(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsValidEmail(this string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, @"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,4}$", RegexOptions.IgnoreCase);
        }

        public static bool IsAbsoluteUri(this string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return false;
                }
                var _ = new Uri(url, UriKind.Absolute);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetAuthority(this string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            var urlArray = url.Split(";");
            var combineUrl = string.Empty;
            foreach (var item in urlArray)
            {
                if (TryParseUri(item, out var uri))
                {
                    combineUrl += $"{GetAuthority(uri)};";
                }
            }
            return combineUrl;
        }

        public static string GetAuthority(this Uri? uri)
        {
            if (uri == null)
            {
                return string.Empty;
            }
            return uri.GetLeftPart(UriPartial.Authority);
        }

        public static bool TryParseUri(this string url, out Uri? uri)
        {
            uri = null;
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return false;
                }
                uri = new Uri(url);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetEmailDomain(this string email)
        {
            if (IsValidEmail(email))
            {
                var parts = email.Split('@');
                if (parts.Length == 2)
                {
                    return parts[1];
                }
            }
            return string.Empty;
        }

        public static string AddTrailingSlashForBeginning(this string str)
        {
            return AddTrailingSlash(str, true); ;
        }

        public static string AddTrailingSlash(this string str)
        {
            return AddTrailingSlash(str, false);
        }

        public static string SafeSubString(this string str, int maxlength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return str.Length > maxlength ? str[..maxlength] : str;
        }

        public static string ConvertToAsciiEncoding(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(str));
        }

        public static string GetCountryCodeAlpha2(this string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                var chunks = countryCode.ToLower().Split("-");
                if (chunks != null && chunks.Any())
                {
                    return chunks[^1];
                }
            }
            return string.Empty;
        }

        public static string GetDigitsOnly(this string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                return Regex.Replace(phoneNumber, @"[^0-9]+", "");
            }
            return string.Empty;
        }

        public static string FormatPhoneNumber(this string phoneNumberString)
        {
            if (!string.IsNullOrEmpty(phoneNumberString) && int.TryParse(phoneNumberString, out var phoneNumber))
            {
                var phoneFormat = "";
                var groupCount = phoneNumberString.Length / 2;
                for (var i = 0; i < groupCount; i++)
                {
                    phoneFormat += "## ";
                }

                if (phoneNumberString.Length > (groupCount * 2))
                {
                    phoneFormat += "#";
                }
                return Convert.ToInt64(phoneNumber).ToString(phoneFormat.TrimEnd());
            }
            return phoneNumberString ?? string.Empty;
        }

        private static string AddTrailingSlash(this string str, bool startPosition)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "/";
            }
            if (startPosition)
            {
                return "/" + str.TrimStart('/');
            }
            return str.TrimEnd('/') + "/";
        }
    }
}
