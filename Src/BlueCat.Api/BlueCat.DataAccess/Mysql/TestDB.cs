using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BlueCat.ORM;

namespace BlueCat.DataAccess
{
    /// <summary>
    ///TestDB的数据库结构类
    /// </summary>
    public partial class TestDB
    {
        public TestDB()
        {

        }


        internal BlueCat.ORM.DataContext DBContext { get; set; }


    }
}
