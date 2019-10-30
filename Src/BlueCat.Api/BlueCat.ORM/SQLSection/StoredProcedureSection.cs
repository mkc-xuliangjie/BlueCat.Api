using BlueCat.ORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{

    /// <summary>
    /// 存储过程Section
    /// </summary>
    /// <remarks>
    ///  	<para>创建：jiaj</para>
    ///  	<para>日期：2016-11-9</para>
    /// </remarks>
    public sealed class StoredProcedureSection
    {
        #region Private Members

        /// <summary>
        /// DB
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private DataContext db;
        /// <summary>
        /// 存储过程名称
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private string spName;
        /// <summary>
        /// 事务
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private DbTransaction tran;

        /// <summary>
        /// 参数名称列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<string> inputParamNames = new List<string>();
        /// <summary>
        /// 参数类型列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<DbType> inputParamTypes = new List<DbType>();

        /// <summary>
        /// 参数值列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<object> inputParamValues = new List<object>();

        /// <summary>
        /// 输出参数名称列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<string> outputParamNames = new List<string>();

        /// <summary>
        /// 输出参数类型列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<DbType> outputParamTypes = new List<DbType>();

        /// <summary>
        /// 输出参数长度列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<int> outputParamSizes = new List<int>();

        /// <summary>
        /// The input output 参数 names
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<string> inputOutputParamNames = new List<string>();
        /// <summary>
        /// The input output 参数 types
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<DbType> inputOutputParamTypes = new List<DbType>();
        /// <summary>
        /// The input output 参数 values
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<object> inputOutputParamValues = new List<object>();
        /// <summary>
        /// The input output 参数 sizes
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private List<int> inputOutputParamSizes = new List<int>();

        /// <summary>
        /// The return value 参数 name
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private string returnValueParamName;
        /// <summary>
        /// The return value 参数 type
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private DbType returnValueParamType;
        /// <summary>
        /// The return value 参数 size
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private int returnValueParamSize;

        /// <summary>
        /// Finds the data reader.
        /// </summary>
        /// <returns>IDataReader.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private IDataReader FindDataReader()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        /// <summary>
        /// Finds the data set.
        /// </summary>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private DataSet FindDataSet()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        /// <summary>
        /// Finds the data set.
        /// </summary>
        /// <param name="outValues">The out values.</param>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private DataSet FindDataSet(out Dictionary<string, object> outValues)
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            DataSet ds = (tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran));
            outValues = GetOutputParameterValues(cmd);
            return ds;
        }

        /// <summary>
        /// Gets the output parameter values.
        /// </summary>
        /// <param name="cmd">The 命令.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        private static Dictionary<string, object> GetOutputParameterValues(DbCommand cmd)
        {
            Dictionary<string, object> outValues;
            outValues = new Dictionary<string, object>();
            for (int i = 0; i < cmd.Parameters.Count; ++i)
            {
                if (cmd.Parameters[i].Direction == ParameterDirection.InputOutput || cmd.Parameters[i].Direction == ParameterDirection.Output || cmd.Parameters[i].Direction == ParameterDirection.ReturnValue)
                {
                    outValues.Add(cmd.Parameters[i].ParameterName.Substring(1, cmd.Parameters[i].ParameterName.Length - 1),
                        cmd.Parameters[i].Value == DBNull.Value ? null : cmd.Parameters[i].Value);
                }
            }
            return outValues;
        }





        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureSection"/> class.
        /// </summary>
        /// <param name="db">The 数据库.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public StoredProcedureSection(DataContext db, string spName)
            : base()
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(spName != null, "spName could not be null.");

            this.db = db;
            this.spName = spName;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Adds the input parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>StoredProcedureSection.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public StoredProcedureSection AddInputParameter(string name, DbType type, object value)
        {
            Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            inputParamNames.Add(name);
            inputParamTypes.Add(type);
            inputParamValues.Add(value);

            return this;
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="size">The size.</param>
        /// <returns>StoredProcedureSection.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public StoredProcedureSection AddOutputParameter(string name, DbType type, int size)
        {
            Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            outputParamNames.Add(name);
            outputParamTypes.Add(type);
            outputParamSizes.Add(size);

            return this;
        }

        /// <summary>
        /// Adds the input output parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="size">The size.</param>
        /// <param name="value">The value.</param>
        /// <returns>StoredProcedureSection.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public StoredProcedureSection AddInputOutputParameter(string name, DbType type, int size, object value)
        {
            Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            inputOutputParamNames.Add(name);
            inputOutputParamTypes.Add(type);
            inputOutputParamSizes.Add(size);
            inputOutputParamValues.Add(value);

            return this;
        }

        /// <summary>
        /// Sets the return parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="size">The size.</param>
        /// <returns>StoredProcedureSection.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public StoredProcedureSection SetReturnParameter(string name, DbType type, int size)
        {
            Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            returnValueParamName = name;
            returnValueParamType = type;
            returnValueParamSize = size;

            return this;
        }

        /// <summary>
        /// Sets the transaction.
        /// </summary>
        /// <param name="tran">The tran.</param>
        /// <returns>StoredProcedureSection.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public StoredProcedureSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public int ExecuteNonQuery()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="outValues">The out values.</param>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public int ExecuteNonQuery(out Dictionary<string, object> outValues)
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            int affactRows = (tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran));
            outValues = GetOutputParameterValues(cmd);
            return affactRows;
        }

        /// <summary>
        /// To the scalar.
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public object ToScalar()
        {
            IDataReader reader = FindDataReader();
            object retObj = null;
            if (reader.Read())
            {
                retObj = reader.GetValue(0);
            }
            reader.Close();
            reader.Dispose();

            return retObj;
        }

        /// <summary>
        /// To the data reader.
        /// </summary>
        /// <returns>IDataReader.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public IDataReader ToDataReader()
        {
            return FindDataReader();
        }

        /// <summary>
        /// To the data set.
        /// </summary>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public DataSet ToDataSet()
        {
            return FindDataSet();
        }

        /// <summary>
        /// To the scalar.
        /// </summary>
        /// <param name="outValues">The out values.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public object ToScalar(out Dictionary<string, object> outValues)
        {
            DataSet ds = FindDataSet(out outValues);
            object retObj = null;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retObj = ds.Tables[0].Rows[0][0];
            }
            ds.Dispose();

            return retObj;
        }

        /// <summary>
        /// To the data set.
        /// </summary>
        /// <param name="outValues">The out values.</param>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-9</para>
        /// </remarks>
        public DataSet ToDataSet(out Dictionary<string, object> outValues)
        {
            return FindDataSet(out outValues);
        }

        /// <summary>
        /// 自定义的返回结果
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        /// <remarks>
        /// 使用方法
        ///   var result =  [...].ToList(p => new { P1=p["name"], P2=p["id"] });
        ///   result[0].P1   result[0].P2
        /// </remarks>
        public List<TResult> ToList<TResult>(Func<IDataReader, TResult> exp)
        {
            List<TResult> retObjs = new List<TResult>();

            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObjs;

                while (reader.Read())
                {
                    retObjs.Add(exp(reader));
                }
                reader.Close();
            }

            return retObjs;
        }


        /// <summary>
        /// 用于返回自定义的数据结构
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        ///  <remarks>
        ///  var result = [...].ToList<Type1>();
        ///  result[0].P1   result[0].P2  返回的结果为Type1对象
        ///  </remarks>
        public List<TResult> ToList<TResult>() where TResult : class
        {
            List<TResult> retObjs = new List<TResult>();

            var drToObjectMapper = new DataReaderToObjectMapper<TResult>();
            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObjs;

                while (reader.Read())
                {
                    retObjs.Add(drToObjectMapper.ConvertObject(reader));
                }
                reader.Close();
            }
            return retObjs;
        }


        #endregion
    }

}
