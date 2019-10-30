using System;
using System.Collections.Generic;
using System.Text;


namespace NBearLite.DbProviders.Oracle
{
    internal class OracleDbProviderOptions : NBearLite.DbProviders.DbProviderOptions
    {
        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            if (options != null && options.Count > 0)
            {
                Dictionary<string, string>.Enumerator en = options.GetEnumerator();
                en.MoveNext();
                if (!string.IsNullOrEmpty(en.Current.Value))
                {
                    return string.Format("SELECT {0}.nextval FROM DUAL", en.Current.Value);
                }
            }

            Check.Require(!string.IsNullOrEmpty(tableName), "tableName could not be null or empty.");
            Check.Require(!string.IsNullOrEmpty(columnName), "columnName could not be null or empty.");

            return string.Format("SELECT SEQ_{0}_{1}.nextval FROM DUAL", tableName, columnName);
        }
    }
}
