//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AdvantShop.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static String UTF8ByteArrayToString(Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            var encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public static string GetWindows1251(string utfText)
        {
            var win = Encoding.GetEncoding("windows-1251");
            var utf = Encoding.GetEncoding("UTF-8");

            var utfBytes = utf.GetBytes(utfText);
            var winBytes = Encoding.Convert(utf, win, utfBytes, 0, utfBytes.Length);

            var winStr = utf.GetString(winBytes, 0, winBytes.Length);

            return winStr;
        }

        public static string GetPlainFieldName(string fieldName)
        {
            return !fieldName.ToLower().Contains("as") ? fieldName : fieldName.Split(new[] { "as" }, StringSplitOptions.RemoveEmptyEntries).First();
        }

        public static string ReplaceCharInStringByIndex(string strSource, int intIndex, Char chrNewSymb)
        {
            var sb = new StringBuilder(strSource);
            sb[intIndex] = chrNewSymb;
            return sb.ToString();
        }

        public static string Translit(string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            var dic = new Dictionary<char, string>
                          {
                              {'а', "a"},
                              {'б', "b"},
                              {'в', "v"},
                              {'г', "g"},
                              {'д', "d"},
                              {'е', "e"},
                              {'ё', "e"},
                              {'ж', "zh"},
                              {'з', "z"},
                              {'и', "i"},
                              {'й', "i"},
                              {'к', "k"},
                              {'л', "l"},
                              {'м', "m"},
                              {'н', "n"},
                              {'о', "o"},
                              {'п', "p"},
                              {'р', "r"},
                              {'с', "s"},
                              {'т', "t"},
                              {'у', "u"},
                              {'ф', "f"},
                              {'х', "kh"},
                              {'ц', "ts"},
                              {'ч', "ch"},
                              {'ш', "sh"},
                              {'щ', "sch"},
                              {'ъ', ""},
                              {'ы', "y"},
                              {'ь', ""},
                              {'э', "e"},
                              {'ю', "iu"},
							  {'я', "ya"},
                          };
            var sb = new StringBuilder();
            foreach (char c in str.ToLower())
            {
                sb.Append(dic.ContainsKey(c) ? dic[c] : c.ToString());
            }
            return sb.ToString();
        }


        public static string TranslitToRus(string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            var dic = new Dictionary<char, string>
                          {
                              {'a', "а"},
                              {'b', "и"},
                              {'c', "к"},
                              {'d', "д"},
                              {'e', "е"},
                              {'f', "ф"},
                              {'g', "г"},
                              {'h', "х"},
                              {'i', "и"},
                              {'j', "й"},
                              {'k', "к"},
                              {'l', "л"},
                              {'m', "м"},
                              {'n', "н"},
                              {'o', "о"},
                              {'p', "п"},
                              {'q', "к"},
                              {'r', "р"},
                              {'s', "с"},
                              {'t', "т"},
                              {'u', "у"},
                              {'v', "в"},
                              {'w', "в"},
                              {'x', "х"},
                              {'y', "й"},
                              {'z', "з"},
                          };
            var sb = new StringBuilder();
            foreach (char c in str.ToLower())
            {
                sb.Append(dic.ContainsKey(c) ? dic[c] : c.ToString());
            }
            return sb.ToString();
        }

        public static string TranslitToRusKeyboard(string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            var dic = new Dictionary<char, string>
                          {
                              {'`', "ё"},
                              {'q', "й"},
                              {'w', "ц"},
                              {'e', "у"},
                              {'r', "к"},
                              {'t', "е"},
                              {'y', "н"},
                              {'u', "г"},
                              {'i', "ш"},
                              {'o', "щ"},
                              {'p', "з"},
                              {'[', "х"},
                              {']', "ъ"},
                              {'a', "ф"},
                              {'s', "ы"},
                              {'d', "в"},
                              {'f', "а"},
                              {'g', "п"},
                              {'h', "р"},
                              {'j', "о"},
                              {'k', "л"},
                              {'l', "д"},
                              {';', "ж"},
                              {'\'', "э"},
                              {'z', "я"},
                              {'x', "ч"},
                              {'c', "с"},
                              {'v', "м"},
                              {'b', "и"},
                              {'n', "т"},
                              {'m', "ь"},
                              {',', "б"},
                              {'.', "ю"},
                          };
            var sb = new StringBuilder();
            foreach (char c in str.ToLower())
            {
                sb.Append(dic.ContainsKey(c) ? dic[c] : c.ToString());
            }
            return sb.ToString();
        }


        public static string TransformUrl(string url)
        {
            var rg = new Regex("[^a-zA-Z0-9_-]+", RegexOptions.Singleline);
            return rg.Replace(url, "-");
        }


        public static string GetReSpacedString(string strSource)
        {
            return GetReSpacedString(strSource, 19); // By default
        }

        public static string GetReSpacedString(string strSource, int intCountCharsBeforeSplit)
        {

            if (string.IsNullOrEmpty(strSource))
                return string.Empty;

            var sbResult = new StringBuilder();
            int j = 0;

            foreach (char t in strSource)
            {
                j += 1;

                if (t == ' ')
                    j = 0;

                if (j >= intCountCharsBeforeSplit)
                {
                    // Добавляем пробле в строку и сбрасываем счетчик.
                    sbResult.Append(t);
                    sbResult.Append(' ');
                    j = 0;
                }
                else
                {
                    // Продолжаем формировать строку.
                    sbResult.Append(t);
                }
            }

            return (sbResult.ToString().Replace(" /", "/ ").Replace(" .", ". ").Replace(" ,", ", ")); // IE Fix with " x" space.

        }

        public static string MakeASCIIUrl(string value)
        {
            try {
                IdnMapping idn = new IdnMapping();
                string domain = value;
                if (value.Contains("//")) {
                    domain = value.Substring(value.IndexOf("//") + 2);
                    if (domain.Contains("/")) {
                        domain = domain.Substring(0, domain.IndexOf("/"));
                    }
                }
                string asciiDomain = idn.GetAscii(domain);
                return asciiDomain == domain ? value : (value.Substring(0, value.IndexOf(domain)) + asciiDomain + value.Substring(value.IndexOf(domain) + domain.Length));
            }
            catch (Exception ex) {
                throw new Exception("MakeASCIIUrl - " + value, ex);
            }
        }


        public static bool GetMoneyFromString(string stringMoney, out float decimalMoney)
        {
            return float.TryParse(stringMoney.Replace(" ", "").Replace(((char)160).ToString(), "").Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture,  out decimalMoney);
        }

        public static string RemoveHTML(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return string.Empty;
            const string HTML_TAG_PATTERN = "<.*?>";
            return HttpUtility.HtmlDecode(Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty).Replace("&nbsp;", " "));

        }

    }
}
