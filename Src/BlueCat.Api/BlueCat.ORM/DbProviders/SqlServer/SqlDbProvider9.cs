using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;



namespace BlueCat.ORM.DbProviders.SqlServer
{
    /// <summary>
    /// Db provider implementation for SQL Server 9.X (2005)
    /// </summary>
    public class SqlDbProvider9 : SqlServer.SqlDbProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlDatabase"/> class.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDbProvider9(string connStr)
            : base(connStr)
        {
        }


        public override BlueCat.ORM.ISqlQueryFactory GetQueryFactory()
        {
            return new BlueCat.ORM.DbProviders.SqlServer.SqlServer9QueryFactory(this.DbProviderFactory);
        }
    }
}
