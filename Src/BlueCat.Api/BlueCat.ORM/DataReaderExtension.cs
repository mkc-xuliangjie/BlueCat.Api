using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BlueCat.ORM.Extension
{
    public static class DataReaderExtension
    {
        public static Guid GuidValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            var value = dr[name].ToString();
            try
            {
                var guidValue = new Guid(value);
                return guidValue;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public static string StringValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            return dr[name].ToString();
        }

        public static int IntValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            var value = 0;
            int.TryParse(dr[name].ToString(), out value);
            return value;
        }

        public static float FloatValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            float value = 0;
            float.TryParse(dr[name].ToString(), out value);
            return value;
        }


        public static short ShortValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            short value = 0;
            short.TryParse(dr[name].ToString(), out value);
            return value;
        }

        public static bool BoolenValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            bool value = false;
            bool.TryParse(dr[name].ToString(), out value);
            return value;
        }

        public static DateTime DateTimeValue(this IDataReader dr, string name)
        {
            if (dr[name] == null) new Exception("该字段不存在");
            DateTime value = DateTime.MinValue ;
            DateTime.TryParse(dr[name].ToString(), out value);
            return value;
        }
    }
}
