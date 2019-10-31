using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BlueCat.ORM;
using BlueCat.DataAccess.QueryTable;
using BlueCat.DataAccess.Entities;
using BlueCat.DataAccess.Schema;

namespace BlueCat.DataAccess.QueryTable
{
    /// <summary>
    ///Risk_Caflowprogress的数据访问类
    /// </summary>
    public class Risk_CaflowprogressQueryTable : QueryTable<Risk_Caflowprogress, Risk_CaflowprogressTableSchema>
    {

         public Risk_CaflowprogressQueryTable(TestDBDataContext dataContext, Risk_CaflowprogressTableSchema schema) : base(dataContext, schema) { }



    }
}

namespace BlueCat.DataAccess
{
    public partial class TestDBDataContext
    {

        /// <summary>
        /// Risk_Caflowprogress的注释
        /// </summary>
        public Risk_CaflowprogressQueryTable Risk_Caflowprogress
        {
            get
            {
                if (_Risk_Caflowprogress == null)
                {
                    _Risk_Caflowprogress = new Risk_CaflowprogressQueryTable(this, TestDB.Risk_Caflowprogress);
                }
                return _Risk_Caflowprogress;
            }
        }   private Risk_CaflowprogressQueryTable _Risk_Caflowprogress;
    }
}
