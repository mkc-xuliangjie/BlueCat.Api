using System;
using System.Data;
using System.Data.Common;
using Npgsql;
using System.Xml;

namespace BlueCat.ORM.DbProviders.PostgreSql
{
	/// <summary>
	/// <para>Represents a Sqlite Database Provider.</para>
	/// </summary>
	/// <remarks> 
	/// <para>
	/// Internally uses Sqlite .NET Managed Provider to connect to the database.
	/// </para>  
	/// </remarks>
    public class PostgreSqlDbProvider : DbProvider
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public PostgreSqlDbProvider(string connStr)
            : base(connStr, NpgsqlFactory.Instance)
        {
            CheckOptionsConfiguration(new PostgreSqlDbProviderOptions(), "PostgreSqlProviderOptions");
        }

        public override BlueCat.ORM.ISqlQueryFactory GetQueryFactory()
        {
            return new BlueCat.ORM.DbProviders.PostgreSql.PostgreSqlQueryFactory();
        }
        #endregion
    }
}