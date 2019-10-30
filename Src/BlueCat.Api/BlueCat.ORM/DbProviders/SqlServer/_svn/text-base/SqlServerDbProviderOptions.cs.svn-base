using System;
using System.Collections.Generic;
using System.Text;

namespace NBearLite.DbProviders.SqlServer
{
    public class SqlServerDbProviderOptions : NBearLite.DbProviders.DbProviderOptions
    {
        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return "SELECT SCOPE_IDENTITY()";
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return true;
            }
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return true;
            }
        }
    }
}
