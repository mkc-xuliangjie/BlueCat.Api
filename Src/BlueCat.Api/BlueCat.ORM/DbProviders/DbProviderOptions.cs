using System;
using System.Collections.Generic;
using System.Text;


namespace BlueCat.ORM.DbProviders
{
    /// <summary>
    /// Options of DbProvider
    /// </summary>
    public class DbProviderOptions
    {
        public bool UseADO20TransactionAsDefaultIfSupport = true;

        private bool supportADO20Transaction;
        private bool supportMultiSqlStatementInOneCommand;

        /// <summary>
        /// if true, by default, ADO20Transaction will be used instead of System.Data.Common.DbTransaction.
        /// </summary>
        public virtual bool SupportADO20Transaction
        {
            get
            {
                return supportADO20Transaction;
            }
            set
            {
                supportADO20Transaction = value;
            }
        }

        /// <summary>
        /// If true, write queries with no return values can be executed together as one query.
        /// </summary>
        public virtual bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return supportMultiSqlStatementInOneCommand;
            }
            set
            {
                supportMultiSqlStatementInOneCommand = value;
            }
        }

        public virtual string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, System.Collections.Generic.Dictionary<string, string> options)
        {
            Check.Require(!string.IsNullOrEmpty(tableName), "tableName could not be null or empty.");
            Check.Require(!string.IsNullOrEmpty(columnName), "columnName could not be null or empty.");

            return string.Format("SELECT MAX([{0}]) FROM [{1}]", columnName, tableName);
        }
    }
}
