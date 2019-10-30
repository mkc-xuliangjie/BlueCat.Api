using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;



namespace BlueCat.ORM.DbProviders.SqlServer
{
    /// <summary>
    /// <para>Represents a Sql Server Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses Sql Server .NET Managed Provider from Microsoft (System.Data.SqlClient) to connect to the database.
    /// </para>  
    /// </remarks>
    public class SqlDbProvider : DbProvider
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public SqlDbProvider(string connStr)
            : base(connStr, SqlClientFactory.Instance)
        {
            CheckOptionsConfiguration(new SqlServerDbProviderOptions(), "SqlServerProviderOptions");
        }


        public override BlueCat.ORM.ISqlQueryFactory GetQueryFactory()
        {
            return new BlueCat.ORM.DbProviders.SqlServer.SqlServerQueryFactory(this.DbProviderFactory);
        }

        #endregion
    }
}