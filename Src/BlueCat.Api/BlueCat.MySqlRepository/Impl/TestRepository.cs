using BlueCat.DataAccess;
using BlueCat.DataAccess.Entities;
using BlueCat.MySqlRepository.Context;
using BlueCat.MySqlRepository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.MySqlRepository.Impl
{
    public class TestRepository : BaseBlueCatRepository, ITestRepository
    {
        public TestRepository(BlueCatScaffoldContextSample context) : base(context)
        {
        }

        public int Get()
        {
            int count = Risk_Caflowprogress.Count();

            return count;
        }
    }
}
