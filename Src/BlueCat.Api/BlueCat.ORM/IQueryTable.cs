using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Reflection;
using BlueCat.ORM.Mapping;
using System.Linq;

namespace BlueCat.ORM
{
    
   
    public interface IQueryTable
    {
        string GetTableName();
    }

    //public class CustomQueryTable : IQueryTable, IExpression
    //{
    //    private readonly string tableName;
    //    private readonly Dictionary<string, KeyValuePair<DbType, object>> parameters = new Dictionary<string, KeyValuePair<DbType, object>>();

    //    public CustomQueryTable(string tableName)
    //    {
    //        Check.Require(tableName, "tableName", Check.NotNullOrEmpty);

    //        this.tableName = tableName;
    //    }

    //    public CustomQueryTable(SubQuery subQuery)
    //    {
    //        Check.Require(subQuery, "subQuery");

    //        this.tableName = subQuery.ToString();
    //        SqlQueryUtils.AddParameters(this.parameters, subQuery);
    //    }

    //    #region IQueryTable Members

    //    public string GetTableName()
    //    {
    //        return tableName;
    //    }

    //    #endregion

    //    #region IExpression Members

    //    public string Sql
    //    {
    //        get
    //        {
    //            return tableName;
    //        }
    //        set
    //        {
    //            throw new Exception("Could not change table name of a CustomQueryTable, you can only specify the table name in constructor.");
    //        }
    //    }

    //    public Dictionary<string, KeyValuePair<DbType, object>> Parameters
    //    {
    //        get
    //        {
    //            return parameters;
    //        }
    //    }

    //    #endregion
    //}

    

    /// <summary>
    /// Strong typed orderby clip, used internal only.
    /// </summary>
  
    //public class SubQuery : ExpressionClip
    //{
    //    public readonly DataContext Db;

    //    internal SubQuery(DataContext db)
    //    {
    //        Check.Require(db, "db");

    //        this.Db = db;
    //    }

    //    public new SubQuery Alias(string aliasName)
    //    {
    //        Check.Require(aliasName, "aliasName", Check.NotNullOrEmpty);

    //        this.sql.Append(' ');
    //        SqlQueryUtils.AppendColumnName(this.sql, aliasName);

    //        return this;
    //    }

    //    public SelectSqlSection Select(params ExpressionClip[] columns)
    //    {
    //        Check.Require(columns, "columns", Check.NotNullOrEmpty);

    //        //SelectSqlSection select = this.Db.Select(new CustomQueryTable(this), columns);
    //        //return select;
    //        //TODO
    //        return null;
    //    }
    //}
}
