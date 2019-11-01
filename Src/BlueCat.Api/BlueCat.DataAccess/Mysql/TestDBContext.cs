using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BlueCat.ORM;

namespace BlueCat.DataAccess
{
    /// <summary>
    ///TestDB的数据库处理类
    /// </summary>
    public partial class TestDBDataContext : BlueCat.ORM.DataContext
    {
        public TestDBDataContext(DatabaseType type, string connectionString)
          : base(type, connectionString)
        {

        }

        public TestDBDataContext(string cnnString)
            : base(DatabaseType.SqlServer9, cnnString)
        { }

        protected override System.Data.Common.DbProviderFactory WarpDbProvider(System.Data.Common.DbProviderFactory dbproviderFactory)
        {
            return dbproviderFactory;
        }

    }
}
