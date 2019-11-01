using BlueCat.DataAccess;
using BlueCat.MySqlRepository.Context;
using BlueCat.ORM;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.MySqlRepository
{
    public abstract class BaseBlueCatRepository : TestDBDataContext
    {
        protected BaseBlueCatRepository(BaseContext context) : base(GetDatabaseTypeByConfig(context.DBType), context.ConnectionString)
        {

        }

        protected static DatabaseType GetDatabaseTypeByConfig(string type)
        {
            type = type.ToLower().Trim();

            switch (type)
            {
                case "sqlserver":
                    return DatabaseType.SqlServer;

                case "sqlserver9":
                    return DatabaseType.SqlServer9;

                case "mysql":
                    return DatabaseType.MySql;

                default:
                    return DatabaseType.MySql;
            }
        }
    }
}
