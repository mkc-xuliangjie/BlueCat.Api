using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BlueCat.ORM.Core.Extensions
{
    public static class IDataRecordExtensions
    {
        public static bool? GetBoolean(this IDataRecord dataRecord, int i, bool? defaultValue)
        {
            if (dataRecord.IsDBNull(i))
            {
                return defaultValue;
            }
            return dataRecord.GetBoolean(i);
        }

        public static byte? GetByte(this IDataRecord dataRecord, int i, byte? defaultValue) {


            if (dataRecord.IsDBNull(i))
            {
                return defaultValue;
            }
            return dataRecord.GetByte(i);

        }


        public static char GetChar(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static long GetChars(this IDataRecord dataRecord, int i, long fieldoffset, char[] buffer, int bufferoffset, int length, int? defaultValue) { }

        public static string GetDataTypeName(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static DateTime GetDateTime(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static decimal GetDecimal(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static double GetDouble(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static Type GetFieldType(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static float GetFloat(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static Guid GetGuid(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static short GetInt16(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static int GetInt32(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static long GetInt64(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static string GetName(this IDataRecord dataRecord, int i, int? defaultValue) { }

        public static int GetOrdinal(this IDataRecord dataRecord, string name, int? defaultValue) { }

        public static string GetString(this IDataRecord dataRecord, int i, int? defaultValue) { }
    }
}
