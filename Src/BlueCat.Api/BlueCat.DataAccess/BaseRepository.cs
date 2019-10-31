using BlueCat.ORM;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.DataAccess
{
    public class BaseRepository : TestDBDataContext
    {
        public BaseRepository(DatabaseType type, string connectionString)
         : base(type, connectionString)
        {
           
        }

        //public BaseRepository(string cnnString)
        //    : base(DatabaseType.SqlServer9, cnnString)
        //{ }

        public BaseRepository(string cnnString)
           : base(DatabaseType.MySql, cnnString)
        { }
    }
}
