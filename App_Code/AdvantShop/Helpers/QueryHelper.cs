//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AdvantShop.Helpers
{
    public static class QueryHelper
    {
        public static string RemoveQueryParam(string query, string name)
        {
            query = query.Trim('?');

            if (query.IsNullOrEmpty())
                return string.Empty;

            var list = new List<string>();
            list.AddRange(query.Split('&'));

            list.RemoveAll(item => item.ToLower().StartsWith(name.ToLower() + "="));

            return list.Any() ? "?" + string.Join("&", list.ToArray()) : string.Empty;
        }

        public static string ChangeQueryParam(string query, string name, string value)
        {
            query = query.Trim('?');

            if (query.IsNullOrEmpty())
                return value.IsNullOrEmpty() ? string.Empty : string.Format("?{0}={1}", name, value);

            var list = new List<string>();
            list.AddRange(query.Split(new [] {'&'}, StringSplitOptions.RemoveEmptyEntries));

            int paramInd;
            if ((paramInd = list.FindIndex(item => item.ToLower().StartsWith(name.ToLower() + "="))) != -1)
            {
                if (value.IsNullOrEmpty())
                    list.RemoveAt(paramInd);
                else
                    list[paramInd] = string.Format("{0}={1}", name, value);
            }
            else if (value.IsNotEmpty())
                list.Add(string.Format("{0}={1}", name, value));

            return list.Any() ? "?" + string.Join("&", list.ToArray()) : string.Empty;
        }

        public static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> pars)
        {
            return pars.Aggregate(new StringBuilder(), (sb, par) => sb.AppendFormat("{0}={1}&", par.Key, par.Value),
                                  sb => sb.ToString().TrimEnd('&'));
        }
    }
}