using BlueCat.DataAccess;
using BlueCat.DataAccess.Entities;
using BlueCat.MySqlRepository.Context;
using BlueCat.MySqlRepository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.MySqlRepository.Impl
{
    public class TestRepository: BaseBlueCatRepository, ITestRepository
    {
        //private readonly string _connectionString;

        //private readonly string _dbType;

        private readonly TestDBDataContext testDBDataContext;
        public TestRepository(BlueCatScaffoldContextSample context) : base(context)
        {
            //_connectionString = context.ConnectionString;
            //_dbType = context.DBType;

            testDBDataContext = new TestDBDataContext(GetDatabaseTypeByConfig(context.DBType), context.ConnectionString);
        }

        public int  Get()
        {
           int count= testDBDataContext.Risk_Caflowprogress.Count();

            return count;
        }
    }
}
