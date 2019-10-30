using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    public class Repository<T, TDBScehma>
        where T : EntityBase
        where TDBScehma : DBSchema
    {
        private DataContext dataContext;

        public DataContext Context
        {
            get { return dataContext; }
        }

        public TDBScehma DB { get; set; }

        public TableSchema Schema { get; set; }


        public Repository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <summary>
        /// 插入一条记录
        ///  var us = new User();
        ///  us.ID = Guid.NewGuid(); 
        ///  us.Name= "aa"; 
        ///  [QueryTable].User.Insert(us); 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <example>
        ///  var us = new User();
        ///  us.ID = Guid.NewGuid();
        ///  us.Name= "aa";
        ///  [QueryTable].User.Insert(us);
        ///  </example>
        public int Insert(T entity)
        {
            return new InsertSqlSection(dataContext, entity).Execute();
        }



        /// <summary>
        /// 批量插入记录
        ///  var us1 = new User();
        ///  us1.ID = Guid.NewGuid(); 
        ///  us1.Name= "aa"; 
        ///  var us2 = new User();
        ///  us2.ID = Guid.NewGuid(); 
        ///  us2.Name= "bb"; 
        ///  [QueryTable].User.Insert(new List() {  us, us2 }); 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <example>
        ///  var us1 = new User();
        ///  us1.ID = Guid.NewGuid(); 
        ///  us1.Name= "aa"; 
        ///  var us2 = new User();
        ///  us2.ID = Guid.NewGuid(); 
        ///  us2.Name= "bb"; 
        ///  [QueryTable].User.Insert(new List() {  us, us2 }); 
        ///  </example>
        public int Insert(List<T> entitys)
        {
            var cols = this.Schema.GetColumns();
            var tableName = this.Schema.GetTableName();
            var colNames = cols.Select(p => "[" + p.ShortName + "]").ToArray();
            var entityPropertyNames = cols.Select(p => p.EntityPropertyName).ToArray();
            var colTypes = cols.Select(p => p.DbType).ToArray();
            var rows = new object[entitys.Count][];
            for (int i = 0; i < entitys.Count; i++)
            {
                var row = new object[cols.Count];
                for (int j = 0; j < cols.Count; j++)
                {
                    var value = entitys[i].GetValue(entityPropertyNames[j]);
                    row[j] = value.Value;
                }
                rows[i] = row;
            }
            return dataContext.ExecuteBatchInsert(tableName, colNames, colTypes, rows, entitys.Count);
        }


        /// <summary>
        /// 自定义插入
        /// </summary>
        /// <param name="entity">要插入的数据</param>
        /// <returns></returns>
        /// <remarks>
        ///  var us = new User();
        ///  us.ID = Guid.NewGuid();
        ///  us.Name= "aa";
        ///  [QueryTable].User.Insert(us).SetTransaction(tr);;
        ///  </remarks>
        public IInsertSqlSection InsertCustom(T entity)
        {
            return new InsertSqlSection(dataContext, entity);
        }


        /// <summary>
        /// 批量插入记录
        ///  var us1 = new User();
        ///  us1.ID = Guid.NewGuid(); 
        ///  us1.Name= "aa"; 
        ///  var us2 = new User();
        ///  us2.ID = Guid.NewGuid(); 
        ///  us2.Name= "bb"; 
        ///  [QueryTable].User.Insert(new List() {  us, us2 }); 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <example>
        ///  var us1 = new User();
        ///  us1.ID = Guid.NewGuid(); 
        ///  us1.Name= "aa"; 
        ///  var us2 = new User();
        ///  us2.ID = Guid.NewGuid(); 
        ///  us2.Name= "bb"; 
        ///  [QueryTable].User.Insert(new List() {  us, us2 }); 
        ///  </example>
        public IInsertSqlSection InsertCustom(List<T> entitys)
        {
            return new BatchInsertSqlSection(dataContext, this.Schema, entitys.ToArray());
        }



        /// <summary>
        ///更新一条记录
        /// </summary>
        /// <param name="entity">要更新的实体（只有发生变动的才更新） 注意设置实体的主键</param>
        /// <returns></returns>
        public int Update(T entity)
        {
            Check.Require(entity != null, "entity could not be null.");

            return new UpdateSqlSection<T>(dataContext, entity).Execute();
        }

        /// <summary>
        /// 自定义更新.
        /// </summary>
        /// <returns></returns>
        /// <remarks>可以设置更新具体的字段，设置更新条件，设置事务
        /// [QueryTable].UpdateCustom()
        ///             .Set(p =>p.Name,  "10")
        ///             .Set(p =>p.Type,  "10")
        ///             .Where(p => p.ID == 1).Execute()
        ///             
        /// 事务的使用
        /// using(var tra == Context.BeginTransaction())  
        /// {
        ///   try
        ///   {
        ///     [QueryTable].UpdateCustom()
        ///             .Set(p =>p.Name,  "10")
        ///             .Set(p =>p.Type,  "10")
        ///             .Where(p => p.ID == 1).SetTransaction(tra);
        ///             
        ///     [QueryTable].DeleteCustom()
        ///             .Where(p => p.ID == 1).SetTransaction(tra);
        ///             
        ///     tra.Commit();
        ///   }
        ///   catch
        ///   {
        ///     tra.Rollback();
        ///   }
        ///     
        /// }
        /// </remarks>
        public UpdateSqlSection<T> UpdateCustom()
        {
            return new UpdateSqlSection<T>(dataContext, Schema);
        }

        /// <summary>
        /// 自定义更新，可用于事务
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        /// <remarks>可以设置更新具体的字段，设置更新条件，设置事务
        /// [QueryTable].UpdateCustom(entity)
        ///             .Where(p => p.ID == 1).Execute();
        /// 事务的使用
        /// using(var tra == Context.BeginTransaction())  
        /// {
        ///   try
        ///   {
        ///     [QueryTable].UpdateCustom()
        ///             .Set(p =>p.Name,  "10")
        ///             .Set(p =>p.Type,  "10")
        ///             .Where(p => p.ID == 1).SetTransaction(tra).Execute();
        ///             
        ///     [QueryTable].DeleteCustom()
        ///             .Where(p => p.ID == 1).SetTransaction(tra).Execute();
        ///             
        ///     tra.Commit();
        ///   }
        ///   catch
        ///   {
        ///     tra.Rollback();
        ///   }
        ///     
        /// }
        /// </remarks>
        public UpdateSqlSection<T> UpdateCustom(T entity)
        {
            Check.Require(entity != null, "entity could not be null.");

            return new UpdateSqlSection<T>(dataContext, entity);
        }


        /// <summary>
        ///删除指定ID的记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public int Delete(string id)
        {
            return new DeleteSqlSection(dataContext, Schema).Where(Schema.PKColumn == id).Execute();
        }


        /// <summary>
        ///删除指定ID的记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            return new DeleteSqlSection(dataContext, Schema).Where(Schema.PKColumn == id).Execute();
        }


        /// <summary>
        /// 删除指定ID的记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public int Delete(int id)
        {
            return new DeleteSqlSection(dataContext, Schema).Where(Schema.PKColumn == id).Execute();
        }


        /// <summary>
        ///删除指定条件的数据
        /// </summary>
        /// <param name="where">删除条件 p => p.Name ==1 && p.ID ==2 </param>
        /// <remarks>设置删除条件，设置事务
        /// [QueryTable].UpdateCustom(entity)
        ///             .Where(p => p.ID == 1).Execute();
        /// 事务的使用
        /// using(var tra == Context.BeginTransaction())  
        /// {
        ///   try
        ///   {
        ///     [QueryTable].UpdateCustom()
        ///             .Set(p =>p.Name,  "10")
        ///             .Set(p =>p.Type,  "10")
        ///             .Where(p => p.ID == 1).SetTransaction(tra);
        ///             
        ///     [QueryTable].DeleteCustom()
        ///             .Where(p => p.ID == 1).SetTransaction(tra);
        ///             
        ///     tra.Commit();
        ///   }
        ///   catch
        ///   {
        ///     tra.Rollback();
        ///   }
        ///     
        /// }
        /// </remarks>
        public DeleteSqlSection DeleteCustom(WhereClip where)
        {
            return new DeleteSqlSection(dataContext, Schema).Where(where);
        }

        /// <summary>
        ///删除指定条件的数据
        /// </summary>
        /// <param name="where">删除条件 p => p.Name ==1 && p.ID ==2 </param>
        /// <remarks>设置删除条件，设置事务
        /// [QueryTable].UpdateCustom(entity)
        ///             .Where(p => p.ID == 1).Execute();
        /// 事务的使用
        /// using(var tra == Context.BeginTransaction())  
        /// {
        ///   try
        ///   {
        ///     [QueryTable].UpdateCustom()
        ///             .Set(p =>p.Name,  "10")
        ///             .Set(p =>p.Type,  "10")
        ///             .Where(p => p.ID == 1).SetTransaction(tra);
        ///             
        ///     [QueryTable].DeleteCustom()
        ///             .Where(p => p.ID == 1).SetTransaction(tra);
        ///             
        ///     tra.Commit();
        ///   }
        ///   catch
        ///   {
        ///     tra.Rollback();
        ///   }
        ///     
        /// }
        /// </remarks>
        public DeleteSqlSection DeleteCustom(Func<TableSchema, WhereClip> whereExp)
        {
            return new DeleteSqlSection(dataContext, Schema).Where(whereExp);
        }


        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <returns></returns>
        public int Clear()
        {
            return new DeleteSqlSection(dataContext, Schema).Execute();
        }


        /// <summary>
        /// 查询，设置指定列
        /// </summary>
        /// <param name="columns">只要查询的列</param>
        /// <returns>查询相关操作的类</returns>
        /// <remarks> 
        /// 对返回的对象可以设置条件where,或者关联查询join,或者通过SetSelectRange 设置查询条数（分页）,或者设置orderby 等等操作
        ///只有在ToList  ToDataSet  ToDataReader 的时候返回数据
        ///使用方法：
        /// [QueryTable].Select(p=>p.Only(p.Name, p.ID))   //在数据库中只获取Name, 和ID
        ///             .Top(10)
        ///             .Where(p => p.Name =="a")
        ///             .OrderBy(p=>p.Name.Asc)
        ///             .ToList();
        /// </remarks>
        public SelectSqlSection<T> Select(params ExpressionClip[] columns)
        {
            return new SelectSqlSection<T>(dataContext, Schema, columns);
        }

        /// <summary>
        /// 查询，设置指定列
        /// </summary>
        /// <param name="exp">这样使用 p=>p.Only(p.Name, p.ID)</param>
        /// <returns>查询相关操作的类</returns>
        /// <remarks> 
        /// 对返回的对象可以设置条件where,或者关联查询join,或者通过SetSelectRange 设置查询条数（分页）,或者设置orderby 等等操作
        ///只有在ToList  ToDataSet  ToDataReader 的时候返回数据
        ///使用方法：
        /// [QueryTable].Select(p=>p.Only(p.Name, p.ID))   //在数据库中只获取Name, 和ID
        ///             .Top(10)
        ///             .Where(p => p.Name =="a")
        ///             .OrderBy(p=>p.Name.Asc)
        ///             .ToList();
        /// </remarks>
        public SelectSqlSection<T> Select(Func<TableSchema, ExpressionClip[]> exp)
        {
            return this.Select(exp(Schema));
        }

        /// <summary>
        /// 查询，设置指定列
        /// </summary>
        /// <typeparam name="T1">相关表</typeparam>
        /// <param name="tb1">相关表</param>
        /// <param name="exp">查询条件使用(t1, t2)=>t1.Field(t1.Name, t2.TypeName)参数1为当前表</param>
        /// <returns></returns>
        public SelectSqlSection<T> Select<T1>(T1 tb1, Func<TableSchema, T1, ExpressionClip[]> exp) where T1 : TableSchema
        {
            return this.Select(exp(Schema, tb1));
        }

        /// <summary>
        ///  查询，设置指定列
        /// </summary>
        /// <typeparam name="T1">相关表1</typeparam>
        /// <typeparam name="T2">相关表2</typeparam>
        /// <param name="tb1">相关表1</param>
        /// <param name="tb2">相关表2</param>
        /// <param name="exp">查询条件使用(t1, t2, t3)=>t1.Field(t1.Name, t2.TypeName, t3.Value)参数1为当前表</param>
        /// <returns></returns>
        public SelectSqlSection<T> Select<T1, T2>(T1 tb1, T2 tb2, Func<TableSchema, T1, T2, ExpressionClip[]> exp)
            where T1 : TableSchema
            where T2 : TableSchema
        {
            return this.Select(exp(Schema, tb1, tb2));
        }

        /// <summary>
        ///  查询，设置指定列
        /// </summary>
        /// <typeparam name="T1">相关表1</typeparam>
        /// <typeparam name="T2">相关表2</typeparam>
        /// <typeparam name="T2">相关表3</typeparam>
        /// <param name="tb1">相关表1</param>
        /// <param name="tb2">相关表2</param>
        /// <param name="tb3">相关表3</param>
        /// <param name="exp">查询条件使用(t1, t2, t3, t4)=>t1.Field(t1.Name, t2.TypeName, t3.Value, t4.Value2) 参数1为当前表</param>
        /// <returns></returns>
        public SelectSqlSection<T> Select<T1, T2, T3>(T1 tb1, T2 tb2, T3 tb3, Func<TableSchema, T1, T2, T3, ExpressionClip[]> exp)
            where T1 : TableSchema
            where T2 : TableSchema
            where T3 : TableSchema
        {
            return this.Select(exp(Schema, tb1, tb2, tb3));
        }

        public SelectSqlSection<T> Select(Func<TableSchema, ExpressionClip> exp)
        {
            return this.Select(exp(Schema));
        }

        /// <summary>
        /// 查询，表相关的所有列
        /// </summary>
        /// <returns>查询相关操作的类</returns>
        /// <remarks> 
        /// 对返回的对象可以设置条件where,或者关联查询join,或者通过SetSelectRange 设置查询条数（分页）,或者设置orderby 等等操作
        ///只有在ToList  ToDataSet  ToDataReader 的时候返回数据
        ///使用方法：
        /// [QueryTable].Select()
        ///             .Where(p => p.Name =="a")
        ///             .SetSelectRange(10, 0)  //获取获取数据的访问
        ///             .OrderBy(p=>p.Name.Asc)
        ///             .ToList();
        /// </remarks>
        public SelectSqlSection<T> Select()
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All);
        }

        /// <summary>
        /// 表的数据总数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return (int)new SelectSqlSection<T>(dataContext, Schema, Schema.Count).ToScalar();
        }

        /// <summary>
        /// 符合条件的数据总数
        /// </summary>
        /// <param name="whereExp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns></returns>
        /// <remarks>
        ///    [QueryTable].Count(p => p.Name ==2);
        /// </remarks>
        public int Count(WhereClip whereExp)
        {
            return (int)new SelectSqlSection<T>(dataContext, Schema, Schema.Count).Where(whereExp).ToScalar();
        }

        /// <summary>
        /// 符合条件的数据总数
        /// </summary>
        /// <param name="whereExp"></param>
        /// <returns></returns>
        ///  <remarks>[QueryTable].Count(p => p.Name ==2);</remarks>
        public int Count(Func<TableSchema, WhereClip> whereExp)
        {
            return (int)new SelectSqlSection<T>(dataContext, Schema, Schema.Count).Where(whereExp(Schema)).ToScalar();
        }

        /// <summary>
        /// 获取指定ID的数据实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntity(int id)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All)
                .Where(Schema.PKColumn == id)
                .ToSingleObject();
        }

        /// <summary>
        /// 获取指定ID的数据实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntity(string id)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All)
                .Where(Schema.PKColumn == id)
                .ToSingleObject();
        }

        /// <summary>
        /// 获取指定ID的数据实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetEntity(Guid id)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All)
                .Top(1)
                .Where(Schema.PKColumn == id)
                .ToSingleObject();
        }

        /// <summary>
        /// 按条件获取数据
        /// </summary>
        /// <param name="whereExp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <param name="orderExp">排序字段：lambda表达式 如 ： p=> p.Name.Asc 或者 p=>p.Name.Desc</param>
        /// <returns></returns>
        public List<T> GetDatas(Func<TableSchema, WhereClip> whereExp, Func<TableSchema, OrderByClip> orderExp)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All).Where(whereExp(Schema)).OrderBy(orderExp(Schema)).ToList();
        }

        /// <summary>
        /// 按条件获取数据
        /// </summary>
        /// <param name="whereExp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns></returns>
        public List<T> GetDatas(Func<TableSchema, WhereClip> whereExp)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All).Where(whereExp(Schema)).ToList();
        }


        /// <summary>
        /// 按照条件分页获取数据
        /// </summary>
        /// <param name="sIndex">获取几条数据</param>
        /// <param name="eIndex">跳过几条数据</param>
        /// <param name="whereExp">条件： lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <param name="orderExp">排序字段：lambda表达式 如 ： p=> p.Name.Asc 或者 p=>p.Name.Desc</param>
        /// <returns></returns>
        /// <remarks>
        ///     [QueryTable].GetRangeDatas(10, 20, p=>p.Name=="aa", p=>p.ID);
        /// </remarks>
        public List<T> GetRangeDatas(int takeCount, int skipCount, Func<TableSchema, WhereClip> whereExp, Func<TableSchema, OrderByClip> orderExp)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All)
                .Where(whereExp(Schema))
                .SetSelectRange(takeCount, skipCount)
                .OrderBy(orderExp(Schema))
                .ToList();
        }

        /// <summary>
        /// 按照条件分页获取数据
        /// </summary>
        /// <param name="sIndex">获取几条数据</param>
        /// <param name="eIndex">跳过几条数据</param>
        /// <param name="whereExp">条件： lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <param name="orderExp">排序字段：lambda表达式 如 ： p=> p.Name.Asc 或者 p=>p.Name.Desc</param>
        /// <returns></returns>
        /// <remarks>
        ///     [QueryTable].GetRangeDatas(10, 20, out recordCount, p=>p.Name=="aa", p=>p.ID);
        /// </remarks>
        public List<T> GetRangeDatas(int takeCount, int skipCount, out int recordCount, Func<TableSchema, WhereClip> whereExp, Func<TableSchema, OrderByClip> orderExp)
        {
            var session = new SelectSqlSection<T>(dataContext, Schema, Schema.All)
                .Where(whereExp(Schema));

            recordCount = session.GetTotalForPaging();

            return session.SetSelectRange(takeCount, skipCount)
                .OrderBy(orderExp(Schema))
                .ToList();
        }


        /// <summary>
        /// 获取所有数据， 一般不要使用
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllData()
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All).ToList();
        }


        /// <summary>
        /// 获取满足条件的第一个数据实体
        /// </summary>
        /// <param name="exp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns>符合条件的第一条数据</returns>
        /// <remarks>[QueryTable].First(p=>p.Name=="2")</remarks>
        public T First(Func<TableSchema, WhereClip> exp)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All).Top(1).Where(exp(Schema)).Top().ToSingleObject();
        }

        /// <summary>
        /// 获取满足条件的第一个数据实体
        /// </summary>
        /// <param name="exp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns>符合条件的第一条数据</returns>
        /// <remarks>[QueryTable].First(p=>p.Name=="2")</remarks>
        public T First(Func<TableSchema, WhereClip> exp, Func<TableSchema, OrderByClip> orderExp)
        {
            return new SelectSqlSection<T>(dataContext, Schema, Schema.All)
                .Top(1)
                .Where(exp(Schema))
                .OrderBy(orderExp(Schema))
                .ToSingleObject();
        }
    }
}
