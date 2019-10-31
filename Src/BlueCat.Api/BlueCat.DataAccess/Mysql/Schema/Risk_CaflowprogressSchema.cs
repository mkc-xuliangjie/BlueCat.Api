using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BlueCat.ORM;
using BlueCat.DataAccess.Schema;

namespace BlueCat.DataAccess.Schema
{
    /// <summary>
    ///Risk_Caflowprogress的注释
    /// </summary>
    public class Risk_CaflowprogressTableSchema : TableSchema
    {



        public Risk_CaflowprogressTableSchema As(string tableAliasName)
        {
            return new Risk_CaflowprogressTableSchema(tableAliasName);
        }



        public Risk_CaflowprogressTableSchema()
            : this(null)
        {
        }



        public Risk_CaflowprogressTableSchema(string tableAliasName)
            : base("risk_caflowprogress")
        {
            TableAliasName=tableAliasName;
            Init();
        }



        private void Init()
        {

             Risk_CAFlowProgressID = new QueryColumn(this.GetTableAliasName() + ".Risk_CAFlowProgressID", DbType.Guid, "Risk_CAFlowProgressID", true);

             Risk_ItemID = new QueryColumn(this.GetTableAliasName() + ".Risk_ItemID", DbType.Guid, "Risk_ItemID", false);

             WorkID = new QueryColumn(this.GetTableAliasName() + ".WorkID", DbType.String, "WorkID", false);

             CurrentNodeID = new QueryColumn(this.GetTableAliasName() + ".CurrentNodeID", DbType.String, "CurrentNodeID", false);

             CurrentNodeName = new QueryColumn(this.GetTableAliasName() + ".CurrentNodeName", DbType.String, "CurrentNodeName", false);

             IsNew = new QueryColumn(this.GetTableAliasName() + ".IsNew", DbType.Boolean, "IsNew", false);

             BtnList = new QueryColumn(this.GetTableAliasName() + ".BtnList", DbType.String, "BtnList", false);

             AltContent = new QueryColumn(this.GetTableAliasName() + ".AltContent", DbType.String, "AltContent", false);

             Solutions = new QueryColumn(this.GetTableAliasName() + ".Solutions", DbType.String, "Solutions", false);

             AccepteApplicanceFormFileID = new QueryColumn(this.GetTableAliasName() + ".AccepteApplicanceFormFileID", DbType.Guid, "AccepteApplicanceFormFileID", false);

             SelfInspectionFormFileID = new QueryColumn(this.GetTableAliasName() + ".SelfInspectionFormFileID", DbType.Guid, "SelfInspectionFormFileID", false);

             AcceptRecordFileID = new QueryColumn(this.GetTableAliasName() + ".AcceptRecordFileID", DbType.Guid, "AcceptRecordFileID", false);

             ReformFileID = new QueryColumn(this.GetTableAliasName() + ".ReformFileID", DbType.Guid, "ReformFileID", false);

             OtherID = new QueryColumn(this.GetTableAliasName() + ".OtherID", DbType.Guid, "OtherID", false);

             UnitID = new QueryColumn(this.GetTableAliasName() + ".UnitID", DbType.Guid, "UnitID", false);

             UnitName = new QueryColumn(this.GetTableAliasName() + ".UnitName", DbType.String, "UnitName", false);

             UserID = new QueryColumn(this.GetTableAliasName() + ".UserID", DbType.Guid, "UserID", false);

             UserName = new QueryColumn(this.GetTableAliasName() + ".UserName", DbType.String, "UserName", false);

             Sort = new QueryColumn(this.GetTableAliasName() + ".Sort", DbType.Int32, "Sort", false);

             AddTime = new QueryColumn(this.GetTableAliasName() + ".AddTime", DbType.DateTime, "AddTime", false);

             UpdateTime = new QueryColumn(this.GetTableAliasName() + ".UpdateTime", DbType.DateTime, "UpdateTime", false);

            this.AddColumn(Risk_CAFlowProgressID);

            this.AddColumn(Risk_ItemID);

            this.AddColumn(WorkID);

            this.AddColumn(CurrentNodeID);

            this.AddColumn(CurrentNodeName);

            this.AddColumn(IsNew);

            this.AddColumn(BtnList);

            this.AddColumn(AltContent);

            this.AddColumn(Solutions);

            this.AddColumn(AccepteApplicanceFormFileID);

            this.AddColumn(SelfInspectionFormFileID);

            this.AddColumn(AcceptRecordFileID);

            this.AddColumn(ReformFileID);

            this.AddColumn(OtherID);

            this.AddColumn(UnitID);

            this.AddColumn(UnitName);

            this.AddColumn(UserID);

            this.AddColumn(UserName);

            this.AddColumn(Sort);

            this.AddColumn(AddTime);

            this.AddColumn(UpdateTime);

            //        Risk_Item = new Risk_ItemTableSchema();
            //Risk_Item.SetRelation(this, this.Risk_ItemID == Risk_Item.Risk_ItemID, "Risk_Item");
            //this.AddForeignTable(Risk_Item, "Risk_Item");
        }


        /// <summary>
        /// 
        /// </summary>
        public QueryColumn Risk_CAFlowProgressID { get; set; }


        /// <summary>
        /// ID
        /// </summary>
        public QueryColumn Risk_ItemID { get; set; }


        /// <summary>
        /// 工作ID
        /// </summary>
        public QueryColumn WorkID { get; set; }


        /// <summary>
        /// 当前节点ID
        /// </summary>
        public QueryColumn CurrentNodeID { get; set; }


        /// <summary>
        /// 当前节点名称
        /// </summary>
        public QueryColumn CurrentNodeName { get; set; }


        /// <summary>
        /// 是否为最新
        /// </summary>
        public QueryColumn IsNew { get; set; }


        /// <summary>
        /// 按钮列表
        /// </summary>
        public QueryColumn BtnList { get; set; }


        /// <summary>
        /// 【验收提醒】验收提醒内容
        /// </summary>
        public QueryColumn AltContent { get; set; }


        /// <summary>
        /// 【验收申请】应对措施
        /// </summary>
        public QueryColumn Solutions { get; set; }


        /// <summary>
        /// 【验收申请】条件验收申请表
        /// </summary>
        public QueryColumn AccepteApplicanceFormFileID { get; set; }


        /// <summary>
        /// 【验收申请】条件验收自检表
        /// </summary>
        public QueryColumn SelfInspectionFormFileID { get; set; }


        /// <summary>
        /// 【验收完成】条件验收记录
        /// </summary>
        public QueryColumn AcceptRecordFileID { get; set; }


        /// <summary>
        /// 【验收完成】条件验收整改情况
        /// </summary>
        public QueryColumn ReformFileID { get; set; }


        /// <summary>
        /// 【验收完成】其他
        /// </summary>
        public QueryColumn OtherID { get; set; }


        /// <summary>
        /// [人员信息]单位ID
        /// </summary>
        public QueryColumn UnitID { get; set; }


        /// <summary>
        /// [人员信息]单位名称
        /// </summary>
        public QueryColumn UnitName { get; set; }


        /// <summary>
        /// [人员信息]提交人ID
        /// </summary>
        public QueryColumn UserID { get; set; }


        /// <summary>
        /// [人员信息]提交人
        /// </summary>
        public QueryColumn UserName { get; set; }


        /// <summary>
        /// [系统]排序序号
        /// </summary>
        public QueryColumn Sort { get; set; }


        /// <summary>
        /// [系统]添加时间
        /// </summary>
        public QueryColumn AddTime { get; set; }


        /// <summary>
        /// [系统]更新时间
        /// </summary>
        public QueryColumn UpdateTime { get; set; }

        //public Risk_ItemTableSchema Risk_Item { get; set; }

    }
}

//分部类
namespace BlueCat.DataAccess
{
    public partial class TestDB
    {

        /// <summary>
        /// Risk_Caflowprogress的数据库结构
        /// </summary>
        public static Risk_CaflowprogressTableSchema Risk_Caflowprogress
        {
            get
            {
                if (_Risk_Caflowprogress == null)
                {
                    _Risk_Caflowprogress = new Risk_CaflowprogressTableSchema();
                }
                return _Risk_Caflowprogress;
            }
        }   private static Risk_CaflowprogressTableSchema _Risk_Caflowprogress;
    }
}
