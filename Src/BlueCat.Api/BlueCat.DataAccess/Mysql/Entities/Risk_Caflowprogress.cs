using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BlueCat.ORM;

namespace BlueCat.DataAccess.Entities
{
    /// <summary>
    ///Risk_Caflowprogress的注释
    /// </summary>
    public class Risk_Caflowprogress : BlueCat.ORM.EntityBase
    {

        protected override TableSchema Schema { get { return TestDB.Risk_Caflowprogress; }  }

        /// <summary>
        /// 
        /// </summary>
        public Guid Risk_CAFlowProgressID
        {
            get { return _Risk_CAFlowProgressID; }
            set
            {
                _Risk_CAFlowProgressID = value;
                SendPropertyChanged("Risk_CAFlowProgressID");
            }
        } private Guid _Risk_CAFlowProgressID;

        /// <summary>
        /// ID
        /// </summary>
        public Guid Risk_ItemID
        {
            get { return _Risk_ItemID; }
            set
            {
                _Risk_ItemID = value;
                SendPropertyChanged("Risk_ItemID");
            }
        } private Guid _Risk_ItemID;

        /// <summary>
        /// 工作ID
        /// </summary>
        public string WorkID
        {
            get { return _WorkID; }
            set
            {
                _WorkID = value;
                SendPropertyChanged("WorkID");
            }
        } private string _WorkID;

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public string CurrentNodeID
        {
            get { return _CurrentNodeID; }
            set
            {
                _CurrentNodeID = value;
                SendPropertyChanged("CurrentNodeID");
            }
        } private string _CurrentNodeID;

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CurrentNodeName
        {
            get { return _CurrentNodeName; }
            set
            {
                _CurrentNodeName = value;
                SendPropertyChanged("CurrentNodeName");
            }
        } private string _CurrentNodeName;

        /// <summary>
        /// 是否为最新
        /// </summary>
        public bool? IsNew
        {
            get { return _IsNew; }
            set
            {
                _IsNew = value;
                SendPropertyChanged("IsNew");
            }
        } private bool? _IsNew;

        /// <summary>
        /// 按钮列表
        /// </summary>
        public string BtnList
        {
            get { return _BtnList; }
            set
            {
                _BtnList = value;
                SendPropertyChanged("BtnList");
            }
        } private string _BtnList;

        /// <summary>
        /// 【验收提醒】验收提醒内容
        /// </summary>
        public string AltContent
        {
            get { return _AltContent; }
            set
            {
                _AltContent = value;
                SendPropertyChanged("AltContent");
            }
        } private string _AltContent;

        /// <summary>
        /// 【验收申请】应对措施
        /// </summary>
        public string Solutions
        {
            get { return _Solutions; }
            set
            {
                _Solutions = value;
                SendPropertyChanged("Solutions");
            }
        } private string _Solutions;

        /// <summary>
        /// 【验收申请】条件验收申请表
        /// </summary>
        public Guid? AccepteApplicanceFormFileID
        {
            get { return _AccepteApplicanceFormFileID; }
            set
            {
                _AccepteApplicanceFormFileID = value;
                SendPropertyChanged("AccepteApplicanceFormFileID");
            }
        } private Guid? _AccepteApplicanceFormFileID;

        /// <summary>
        /// 【验收申请】条件验收自检表
        /// </summary>
        public Guid? SelfInspectionFormFileID
        {
            get { return _SelfInspectionFormFileID; }
            set
            {
                _SelfInspectionFormFileID = value;
                SendPropertyChanged("SelfInspectionFormFileID");
            }
        } private Guid? _SelfInspectionFormFileID;

        /// <summary>
        /// 【验收完成】条件验收记录
        /// </summary>
        public Guid? AcceptRecordFileID
        {
            get { return _AcceptRecordFileID; }
            set
            {
                _AcceptRecordFileID = value;
                SendPropertyChanged("AcceptRecordFileID");
            }
        } private Guid? _AcceptRecordFileID;

        /// <summary>
        /// 【验收完成】条件验收整改情况
        /// </summary>
        public Guid? ReformFileID
        {
            get { return _ReformFileID; }
            set
            {
                _ReformFileID = value;
                SendPropertyChanged("ReformFileID");
            }
        } private Guid? _ReformFileID;

        /// <summary>
        /// 【验收完成】其他
        /// </summary>
        public Guid? OtherID
        {
            get { return _OtherID; }
            set
            {
                _OtherID = value;
                SendPropertyChanged("OtherID");
            }
        } private Guid? _OtherID;

        /// <summary>
        /// [人员信息]单位ID
        /// </summary>
        public Guid UnitID
        {
            get { return _UnitID; }
            set
            {
                _UnitID = value;
                SendPropertyChanged("UnitID");
            }
        } private Guid _UnitID;

        /// <summary>
        /// [人员信息]单位名称
        /// </summary>
        public string UnitName
        {
            get { return _UnitName; }
            set
            {
                _UnitName = value;
                SendPropertyChanged("UnitName");
            }
        } private string _UnitName;

        /// <summary>
        /// [人员信息]提交人ID
        /// </summary>
        public Guid UserID
        {
            get { return _UserID; }
            set
            {
                _UserID = value;
                SendPropertyChanged("UserID");
            }
        } private Guid _UserID;

        /// <summary>
        /// [人员信息]提交人
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                SendPropertyChanged("UserName");
            }
        } private string _UserName;

        /// <summary>
        /// [系统]排序序号
        /// </summary>
        public int Sort
        {
            get { return _Sort; }
            set
            {
                _Sort = value;
                SendPropertyChanged("Sort");
            }
        } private int _Sort;

        /// <summary>
        /// [系统]添加时间
        /// </summary>
        public DateTime AddTime
        {
            get { return _AddTime; }
            set
            {
                _AddTime = value;
                SendPropertyChanged("AddTime");
            }
        } private DateTime _AddTime;

        /// <summary>
        /// [系统]更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set
            {
                _UpdateTime = value;
                SendPropertyChanged("UpdateTime");
            }
        } private DateTime _UpdateTime;

    }
}
