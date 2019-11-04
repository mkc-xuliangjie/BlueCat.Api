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
        protected BaseBlueCatRepository(BaseContext context) : base(TestDBDataContext.GetDatabaseTypeByConfig(context.DBType), context.ConnectionString)
        {

        }
    }
}
