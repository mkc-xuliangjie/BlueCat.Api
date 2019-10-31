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

        //public static TestDBDataContext Current
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            //var connect = ServiceExtensions.Configuration.GetConnectionString("TestDB");
        //            //var dbType = ServiceExtensions.Configuration.GetValue<string>("DBType", "MSSql");
        //            string connect = string.Empty;
        //            string dbType = string.Empty;
        //            DatabaseType type = DatabaseType.SqlServer9;
        //            if (dbType == "MySql")
        //            {
        //                type = DatabaseType.MySql;
        //            }
        //            instance = new TestDBDataContext(type, connect);
        //        }
        //        return instance;
        //    }
        //}


        private static TestDBDataContext instance;

        protected override System.Data.Common.DbProviderFactory WarpDbProvider(System.Data.Common.DbProviderFactory dbproviderFactory)
        {
            return dbproviderFactory;
        }

    }
}
