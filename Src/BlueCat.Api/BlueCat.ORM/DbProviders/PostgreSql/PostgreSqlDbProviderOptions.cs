using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.ORM.DbProviders.PostgreSql
{
    public class PostgreSqlDbProviderOptions : BlueCat.ORM.DbProviders.DbProviderOptions
    {
        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return string.Format("SELECT currval('\"{0}_{1}_seq\"')", tableName, columnName);
        }
    }
}
