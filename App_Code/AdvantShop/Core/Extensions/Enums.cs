//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Resources;

namespace AdvantShop
{
    /// <summary>
    /// Summary description for Enums
    /// </summary>
    public static class Enums
    {
        public static string GetLocalizedName(this Enum val)
        {
            return
                new ResourceManager(typeof(Resource)).GetString(string.Format("Enums_{0}_{1}", val.GetType().Name, val))
                    .Default(val.ToString());
        }

        public static IEnumerable<string> GetLocalizedNames(this Enum val)
        {
            var resourceManager = new ResourceManager(typeof(Resource));
            var type = val.GetType();
            return from object value in Enum.GetValues(type) select resourceManager.GetString(string.Format("Enums_{0}_{1}", type.Name, value));
        }

        public static IEnumerable<string> GetValues(this Enum val)
        {
            var type = val.GetType();
            return from object value in Enum.GetValues(type) select value.ToString();
        }



        public static TResult ToEnum<TResult>(this Enum val)
        {
            var type = typeof(TResult);
            if (type.BaseType != typeof(Enum))
                throw new ArgumentException("TResult must be an enumeration.");
            var valName = val.ToString();
            return Enum.GetValues(type).Cast<TResult>().FirstOrDefault(value => value.ToString() == valName);
        }

        //using
        //public enum TestEnum {A,B,C}
        //public void TestMethod(string StringOfEnum)
        //{
        //    TestEnum myEnum;
        //    myEnum.TryParse(StringOfEnum, out myEnum);
        //}
        public static bool TryParse<T>(this Enum theEnum, string strType, out T result)
        {
            var strTypeFixed = strType.Replace(' ', '_');
            if (Enum.IsDefined(typeof(T), strTypeFixed))
            {
                result = (T)Enum.Parse(typeof(T), strTypeFixed, true);
                return true;
            }
            foreach (string value in Enum.GetNames(typeof(T)))
            {
                if (!value.Equals(strTypeFixed, StringComparison.OrdinalIgnoreCase)) continue;
                result = (T)Enum.Parse(typeof(T), value);
                return true;
            }
            result = default(T);
            return false;
        }

        //using
        //int a = MyEnum.A.ConvertEnum<int>();
        public static TConvertType ConvertEnum<TConvertType>(this Enum e)
        {
            object o;
            Type type = typeof(TConvertType);
            if (type == typeof(int))
            {
                o = Convert.ToInt32(e);
            }
            else if (type == typeof(long))
            {
                o = Convert.ToInt64(e);
            }
            else if (type == typeof(short))
            {
                o = Convert.ToInt16(e);
            }
            else
            {
                o = Convert.ToString(e);
            }
            return (TConvertType)o;
        }

        public static string ConvertIntString(this Enum e)
        {
            return Convert.ToInt16(e).ToString(CultureInfo.InvariantCulture);
        }
    }
}