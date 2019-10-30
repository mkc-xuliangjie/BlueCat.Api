using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Xml;

namespace BlueCat.ORM.DbProviders.MsAccess
{
	/// <summary>
	/// Access数据路的Provider
	/// </summary>
    public class AccessDbProvider : DbProvider
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public AccessDbProvider(string connStr)
            : base(connStr, OleDbFactory.Instance)
        {
            CheckOptionsConfiguration(new DbProviderOptions(), "MsAccessProviderOptions");
        }

        public override BlueCat.ORM.ISqlQueryFactory GetQueryFactory()
        {
            return new BlueCat.ORM.DbProviders.MsAccess.MsAccessQueryFactory(this.DbProviderFactory);
        }

        #endregion
    }
}