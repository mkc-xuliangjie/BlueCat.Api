using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;


namespace BlueCat.ORM.DbProviders
{
    /// <summary>
    /// The base class of all db providers.
    /// </summary>
    public abstract class DbProvider
    {
        #region Protected Members

        /// <summary>
        /// The db provider factory.
        /// </summary>
        protected System.Data.Common.DbProviderFactory dbProviderFactory;
        /// <summary>
        /// The db connection string builder
        /// </summary>
        protected DbConnectionStringBuilder dbConnStrBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="dbProviderFactory">The db provider factory.</param>
        protected DbProvider(string connStr, System.Data.Common.DbProviderFactory dbProviderFactory)
        {
            dbConnStrBuilder = new DbConnectionStringBuilder();
            dbConnStrBuilder.ConnectionString = connStr;
            
            this.dbProviderFactory = dbProviderFactory;
        }

        /// <summary>
        /// 用于对DbProviderFactory的包装
        /// </summary>
        /// <param name="fac"></param>
        public void OverrideDbProvider(System.Data.Common.DbProviderFactory fac)
        {
            this.dbProviderFactory = fac;
        }

        private DbProviderOptions options;

        protected void CheckOptionsConfiguration(DbProviderOptions options, string configPrefix)
        {
            Check.Require(options != null, "options could not be null.");
            Check.Require(!string.IsNullOrEmpty(configPrefix), "configPrefix could not be null or empty.");

            this.options = options;

            //check configuration
            //System.Collections.Specialized.NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;
            //for (int i = 0; i < appSettings.AllKeys.Length; ++i)
            //{
            //    string lowerKey = appSettings.AllKeys[i].ToLower();
            //    if (lowerKey == (configPrefix + ".UseADO20TransactionAsDefaultIfSupport").ToLower())
            //    {
            //        this.options.UseADO20TransactionAsDefaultIfSupport = (appSettings[i].ToLower() == "true");
            //    }
            //    else if (lowerKey == (configPrefix + ".SupportADO20Transaction").ToLower())
            //    {
            //        this.options.SupportADO20Transaction = (appSettings[i].ToLower() == "true");
            //    }
            //    else if (lowerKey == (configPrefix + ".SupportMultiSqlStatementInOneCommand").ToLower())
            //    {
            //        this.options.SupportMultiSqlStatementInOneCommand = (appSettings[i].ToLower() == "true");
            //    }
            //}
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                return dbConnStrBuilder.ConnectionString;
            }
        }

        /// <summary>
        /// Gets the db provider factory.
        /// </summary>
        /// <value>The db provider factory.</value>
        public System.Data.Common.DbProviderFactory DbProviderFactory
        {
            get
            {
                return dbProviderFactory;
            }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public DbProviderOptions Options
        {
            get
            {
                return options ?? new DbProviderOptions();
            }
        }

        #endregion

        #region Abstract Members

        public abstract BlueCat.ORM.ISqlQueryFactory GetQueryFactory();

        #endregion
    }
}
