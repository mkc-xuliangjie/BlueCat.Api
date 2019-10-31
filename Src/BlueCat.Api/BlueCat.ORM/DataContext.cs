using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Transactions;
using System.Web;
using BlueCat.ORM.DbProviders;
//using StackExchange.Profiling;
//using StackExchange.Profiling.Data;

namespace BlueCat.ORM
{

    /// <summary>
    /// Database Types supported.
    /// </summary>
    public enum DatabaseType
    {
        SqlServer,
        SqlServer9,
        MsAccess,
        Oracle,
        MySql,
        Sqlite,
        PostgreSql,
        Other
    }

    public delegate void LogHandler(string logMsg);

    public class DataContext
    {
        #region Private Members

        private readonly ISqlQueryFactory queryFactory;
        private readonly DbProviders.DbProvider dbProvider;

        private DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            var conn = GetConnection();
            DbCommand command = dbProvider.DbProviderFactory.CreateCommand();
            command.Connection = conn;
            command.CommandType = commandType;
            command.CommandText = commandText;

            return command;
        }
        private void DoLoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, MissingSchemaAction missingSchemaAction = MissingSchemaAction.Add)
        {
            Check.Require(tableNames != null && tableNames.Length > 0, "tableNames could not be null or empty.");
            Check.Require(dataSet != null, "dataSet could not be null.");

            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return;
            }

            using (DbDataAdapter adapter = GetDataAdapter())
            {
                WriteLog(command);
                adapter.MissingSchemaAction = missingSchemaAction;
                ((IDbDataAdapter)adapter).SelectCommand = command;

                try
                {
                    string systemCreatedTableNameRoot = "Table";
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemCreatedTableName = (i == 0)
                             ? systemCreatedTableNameRoot
                             : systemCreatedTableNameRoot + i;

                        adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                    }

                    adapter.Fill(dataSet);
                }
                catch
                {
                    throw;
                }
            }
        }
        private object DoExecuteScalar(DbCommand command)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return null;
            }

            try
            {
                WriteLog(command);

                object returnValue = command.ExecuteScalar();
                return returnValue;
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseConnection(command);
            }
        }
        private int DoExecuteNonQuery(DbCommand command)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return 0;
            }

            try
            {
                WriteLog(command);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
            catch
            {
                throw;
            }
        }
        private IDataReader DoExecuteReader(DbCommand command, CommandBehavior cmdBehavior)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return null;
            }

            try
            {
                WriteLog(command);

                IDataReader reader = command.ExecuteReader(cmdBehavior);
                return reader;
            }
            catch
            {
                throw;
            }
        }
        private DbTransaction BeginTransaction(DbConnection connection)
        {
            return connection.BeginTransaction();
        }
        private IDbTransaction BeginTransaction(DbConnection connection, System.Data.IsolationLevel il)
        {
            return connection.BeginTransaction(il);
        }
        private DbCommand PrepareCommand(DbCommand command, DbConnection connection)
        {
            Check.Require(command != null, "command could not be null.");
            Check.Require(connection != null, "connection could not be null.");

            command.Connection = connection;
            command.CommandTimeout = 60 * 3;
            //if (this.dbProvider.GetType() == typeof(DbProviders.MsAccess.AccessDbProvider))
            //{
            //    command.CommandText = FilterNTextPrefix(command.CommandText);
            //}
            return command;
            //string sqltext = command.CommandText;
            //CommandType ct = command.CommandType;

            //var wCommand = connection.CreateCommand();
            //wCommand.CommandText = sqltext;
            //wCommand.CommandType = ct;

            //foreach (DbParameter pm in command.Parameters)
            //{
            //    var wParameter = command.CreateParameter();
            //    wParameter.Direction = pm.Direction;
            //    wParameter.DbType = pm.DbType;
            //    wParameter.ParameterName = pm.ParameterName;
            //    wParameter.Size = pm.Size;
            //    wParameter.SourceColumn = pm.SourceColumn;
            //    wParameter.SourceColumnNullMapping = pm.SourceColumnNullMapping;
            //    wParameter.SourceVersion = pm.SourceVersion;
            //    wParameter.Value = pm.Value;
            //    wCommand.Parameters.Add(wParameter);
            //}

            //if (this.dbProvider.GetType() == typeof(DbProviders.MsAccess.AccessDbProvider))
            //{
            //    wCommand.CommandText = FilterNTextPrefix(wCommand.CommandText);
            //}
            //wCommand.CommandTimeout = 60 * 3;
            //return wCommand;
        }
        private DbCommand PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            Check.Require(command != null, "command could not be null.");
            Check.Require(transaction != null, "transaction could not be null.");

            var wCommand = PrepareCommand(command, transaction.Connection);
            wCommand.Transaction = transaction;

            return wCommand;
        }
        private static void ConfigureParameter(DbParameter param, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            param.DbType = dbType;
            param.Size = size;
            param.Value = (value == null) ? DBNull.Value : value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
        }
        private DbParameter CreateParameter(string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(name);
            ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }
        private DbParameter CreateParameter(string name)
        {
            DbParameter param = dbProvider.DbProviderFactory.CreateParameter();
            param.ParameterName = name;

            return param;
        }
        private string FilterNTextPrefix(string sql)
        {
            if (sql == null)
            {
                return sql;
            }

            return sql.Replace(" N'", " '");
        }

        internal void WriteLog(DbCommand command)
        {
            if (OnLog != null)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(string.Format("{0}\t{1}\t\r\n", command.CommandType, command.CommandText));
                if (command.Parameters != null && command.Parameters.Count > 0)
                {
                    sb.Append("Parameters:\r\n");
                    foreach (DbParameter p in command.Parameters)
                    {
                        sb.Append(string.Format("{0}[{2}] = {1}\r\n", p.ParameterName, DataUtils.ToString(p.DbType, p.Value), p.DbType));
                    }
                }
                sb.Append("\r\n");

                OnLog(sb.ToString());
            }
        }

        internal void WriteLog(string logMsg)
        {
            if (OnLog != null)
            {
                OnLog(logMsg);
            }
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        private void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText,
            DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> within the given 
        /// <paramref name="transaction" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            LoadDataSet(command, dataSet, new string[] { tableName });
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" /> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, DbTransaction transaction)
        {
            LoadDataSet(command, dataSet, new string[] { tableName }, transaction);
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (IsBatchConnection)
            {
                var wCommand = PrepareCommand(command, GetConnection(true));
                DoLoadDataSet(wCommand, dataSet, tableNames);
            }
            else
            {
                using (DbConnection connection = GetConnection())
                {
                    var wCommand = PrepareCommand(command, connection);
                    DoLoadDataSet(wCommand, dataSet, tableNames);
                }
            }
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in  a transaction.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction)
        {
            var wCommand = PrepareCommand(command, transaction);
            DoLoadDataSet(wCommand, dataSet, tableNames);
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        private void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        #endregion

        #region Close Connection

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="command">The command.</param>
        public void CloseConnection(DbCommand command)
        {
            if (command != null && command.Connection.State != ConnectionState.Closed && batchConnection == null)
            {
                command.CreateParameter();
                if (command.Transaction == null)
                {
                    CloseConnection(command.Connection);
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="conn">The conn.</param>
        public void CloseConnection(DbConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="tran">The tran.</param>
        public void CloseConnection(DbTransaction tran)
        {
            if (tran.Connection != null)
            {
                CloseConnection(tran.Connection);
                tran.Dispose();
            }
        }

        #endregion

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="connectionString">The connection string.</param>
        public DataContext(DatabaseType type, string connectionString)
        {
            Check.Require(!string.IsNullOrEmpty(connectionString), "connectionString could not be null or empty.");
            DbProviders.DbProvider provider=null;
            switch (type)
            {
                case DatabaseType.MsAccess:
                    //provider = new DbProviders.MsAccess.AccessDbProvider(connectionString);
                    throw new NotSupportedException("MsAccess DatabaseType.");
                case DatabaseType.SqlServer:
                    provider = new DbProviders.SqlServer.SqlDbProvider(connectionString);
                    break;
                case DatabaseType.SqlServer9:
                    provider = new DbProviders.SqlServer.SqlDbProvider9(connectionString);
                    break;
                case DatabaseType.MySql:



                    provider = new BlueCat.ORM.DbProviders.MySql.MySqlDbProvider(connectionString);
                    // provider = DbProviders.DbProviderFactory.CreateDbProvider("BlueCat.ORM.Core.AdditionalDbProviders", "BlueCat.ORM.DbProviders.MySql.MySqlDbProvider", connectionString);
                    break;
                case DatabaseType.Oracle:
                    throw new NotSupportedException("Oracle DatabaseType.");
                case DatabaseType.Sqlite:
                    //provider = DbProviders.DbProviderFactory.CreateDbProvider("BlueCat.ORM.Core.AdditionalDbProviders", "BlueCat.ORM.DbProviders.Sqlite.SqliteDbProvider", connectionString);
                    break;
                case DatabaseType.PostgreSql:

                    provider = new BlueCat.ORM.DbProviders.PostgreSql.PostgreSqlDbProvider(connectionString);
                    //provider = DbProviders.DbProviderFactory.CreateDbProvider("BlueCat.ORM.Core.AdditionalDbProviders", "BlueCat.ORM.DbProviders.PostgreSql.PostgreSqlDbProvider", connectionString);
                    break;
                default:
                    throw new NotSupportedException("Unknow DatabaseType.");
            }
            provider.OverrideDbProvider(WarpDbProvider(provider.DbProviderFactory));
            this.dbProvider = provider;
            this.queryFactory = dbProvider.GetQueryFactory();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        //public DataContext(string connectionStringName)
        //{
        //    var provider = DbProviders.DbProviderFactory.CreateDbProvider(connectionStringName);
        //    provider.OverrideDbProvider(WarpDbProvider(provider.DbProviderFactory));
        //    this.dbProvider = provider;
        //    this.queryFactory = dbProvider.GetQueryFactory();
        //}

        #endregion

        #region MiniProfiler

        protected virtual System.Data.Common.DbProviderFactory WarpDbProvider(System.Data.Common.DbProviderFactory dbproviderFactory)
        {
            //if (MiniProfilerHttpModule.EnableMiniProfiler)
            //{
            //    return new ProfiledDbProviderFactory(MiniProfiler.Current, dbproviderFactory);
            //}
            //else
            //{
            return dbproviderFactory;
            //}
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connect string.
        /// </summary>
        /// <value>The connect string.</value>
        internal string ConnectionString
        {
            get
            {
                return dbProvider.ConnectionString;
            }
        }

        /// <summary>
        /// Get the QueryFactory, which can be used to construct complex CRUD command.
        /// </summary>
        internal ISqlQueryFactory QueryFactory
        {
            get { return this.queryFactory; }
        }

        /// <summary>
        /// Gets the db provider.
        /// </summary>
        /// <value>The db provider.</value>
        internal DbProviders.DbProvider DbProvider
        {
            get
            {
                return dbProvider;
            }
        }

        #endregion

        #region Public Methods

        #region Factory Methods

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            if (batchConnection == null)
            {
                return CreateConnection();
            }
            else
            {
                return batchConnection;
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="tryOpen">if set to <c>true</c> [try open].</param>
        /// <returns></returns>
        public DbConnection GetConnection(bool tryOpen)
        {
            if (batchConnection == null)
            {
                return CreateConnection(tryOpen);
            }
            else
            {
                return batchConnection;
            }
        }

        /// <summary>
        /// <para>When overridden in a derived class, gets the connection for this database.</para>
        /// <seealso cref="DbConnection"/>        
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DbConnection"/> for this database.</para>
        /// </returns>
        public DbConnection CreateConnection()
        {
            DbConnection newConnection = dbProvider.DbProviderFactory.CreateConnection();
            newConnection.ConnectionString = ConnectionString;

            return newConnection;
        }

        /// <summary>
        /// <para>When overridden in a derived class, gets the connection for this database.</para>
        /// <seealso cref="DbConnection"/>        
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DbConnection"/> for this database.</para>
        /// </returns>
        public DbConnection CreateConnection(bool tryOpenning)
        {
            if (!tryOpenning)
            {
                return CreateConnection();
            }

            DbConnection connection = null;
            try
            {
                connection = CreateConnection();
                connection.Open();
            }
            catch (DataException)
            {
                CloseConnection(connection);

                throw;
            }

            return connection;
        }

        /// <summary>
        /// <para>When overridden in a derived class, creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            Check.Require(!string.IsNullOrEmpty(storedProcedureName), "storedProcedureName could not be null.");

            return CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
        }

        /// <summary>
        /// <para>When overridden in a derived class, creates an <see cref="DbCommand"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
        public DbCommand GetSqlStringCommand(string query)
        {
            Check.Require(!string.IsNullOrEmpty(query), "query could not be null.");

            return CreateCommandByCommandType(CommandType.Text, query);
        }

        /// <summary>
        /// Gets a DbDataAdapter with Standard update behavior.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        private DbDataAdapter GetDataAdapter()
        {
            return dbProvider.DbProviderFactory.CreateDataAdapter();
        }

        #endregion

        #region Basic Execute Methods

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public DataSet ExecuteDataSet(DbCommand command)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table");
            return dataSet;
        }

        public DataSet ExecuteDataSetWithSchema(DbCommand command)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            DoLoadDataSet(command, dataSet, new string[] { "Table" }, MissingSchemaAction.AddWithKey);
            return dataSet;
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table", transaction);
            return dataSet;
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbCommand command)
        {
            if (IsBatchConnection)
            {
                var wCommand = PrepareCommand(command, GetConnection(true));
                return ExecuteScalar(wCommand);
            }
            else
            {
                using (DbConnection connection = GetConnection(true))
                {
                    var wCommand = PrepareCommand(command, connection);
                    return DoExecuteScalar(wCommand);
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbCommand command, DbTransaction transaction)
        {
            var wCommand = PrepareCommand(command, transaction);
            return DoExecuteScalar(wCommand);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" />  and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
        /// within the given <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbCommand command)
        {
            if (IsBatchConnection)
            {
                var wCommand = PrepareCommand(command, GetConnection(true));
                return DoExecuteNonQuery(wCommand);
            }
            else
            {
                using (DbConnection connection = GetConnection(true))
                {
                    var wCommand = PrepareCommand(command, connection);
                    return DoExecuteNonQuery(wCommand);
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbCommand command, DbTransaction transaction)
        {
            var wCommand = PrepareCommand(command, transaction);
            return DoExecuteNonQuery(wCommand);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(CommandType commandType, string commandText, KeyValuePair<string, object>[] parameters)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(DbCommand command)
        {
            if (IsBatchConnection)
            {
                var wCommand = PrepareCommand(command, GetConnection(true));
                return DoExecuteReader(wCommand, CommandBehavior.Default);
            }
            else
            {
                DbConnection connection = GetConnection(true);

                var wCommand = PrepareCommand(command, connection);

                try
                {
                    return DoExecuteReader(wCommand, CommandBehavior.CloseConnection);
                }
                catch (DataException)
                {
                    CloseConnection(connection);

                    throw;
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a transaction and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
                        
            return DoExecuteReader(command, CommandBehavior.Default);
        }

        #endregion

        #region Extended Execute Methods

        /// <summary>
        /// 批量执行插入操作
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnNames">The column names.</param>
        /// <param name="columnTypes">The column types.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="tran">The tran.</param>
        /// <returns></returns>
        public int ExecuteBatchInsert(string tableName, string[] columnNames, DbType[] columnTypes, object[][] rows,
            int batchSize, DbTransaction tran)
        {
            Check.Require(!string.IsNullOrEmpty(tableName), "tableName could not be null or empty.");
            Check.Require(columnNames != null && columnNames.Length > 0, "columnNames could not be null or empty.");
            Check.Require(columnTypes != null && columnNames.Length > 0, "columnNames could not be null or empty.");
            Check.Require(rows != null && columnNames.Length > 0, "columnNames could not be null or empty.");
            Check.Require(columnNames.Length == columnTypes.Length && columnNames.Length == rows[0].Length,
                "length of column's names, types and values must equal.");
            Check.Require(batchSize > 0, "batchSize must > 0.");

            int columnCount = columnNames.Length;
            int retCount = 0;
            if (queryFactory.SupportBatchInsert())
            {
                var cmd1 = queryFactory.CreateBatchInsertCommand(tableName, columnNames, columnTypes, rows);
                retCount += (tran == null ? ExecuteNonQuery(cmd1) : ExecuteNonQuery(cmd1, tran));

                return retCount;
            }
            //不支持批量插入执行，现阶段只有Acess暂不知
            DbProviders.DbProviderOptions options = dbProvider.Options;
            if (options.SupportADO20Transaction && options.UseADO20TransactionAsDefaultIfSupport && tran == null)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    for (int i = 0; i < rows.Length; ++i)
                    {
                        DbCommand cmd = queryFactory.CreateInsertCommand(tableName, columnNames, columnTypes, rows[i]);
                        retCount += ExecuteNonQuery(cmd);
                    }

                    scope.Complete();
                }
            }
            else
            {
                if (tran != null)
                {
                    for (int i = 0; i < rows.Length; ++i)
                    {
                        DbCommand cmd = queryFactory.CreateInsertCommand(tableName, columnNames, columnTypes, rows[i]);
                        retCount += ExecuteNonQuery(cmd, tran);
                    }
                }
                else
                {
                    DbTransaction t = BeginTransaction();
                    try
                    {
                        for (int i = 0; i < rows.Length; ++i)
                        {
                            DbCommand cmd = queryFactory.CreateInsertCommand(tableName, columnNames, columnTypes, rows[i]);
                            retCount += ExecuteNonQuery(cmd, t);
                        }

                        t.Commit();
                    }
                    catch
                    {
                        t.Rollback();

                        throw;
                    }
                    finally
                    {
                        CloseConnection(t);
                    }
                }
            }


            return retCount;
        }

        /// <summary>
        /// Executes the batch insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnNames">The column names.</param>
        /// <param name="columnTypes">The column types.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public int ExecuteBatchInsert(string tableName, string[] columnNames, DbType[] columnTypes, object[][] rows, int batchSize)
        {
            return ExecuteBatchInsert(tableName, columnNames, columnTypes, rows, batchSize, null);
        }
                                          


        /// <summary>
        /// Executes the insert return auto increment ID.
        /// </summary>
        /// <param name="basicInsertCmd">The basic insert CMD.</param>
        /// <param name="tran">The tran.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="autoIncrementColumn">The auto increment column.</param>
        /// <returns></returns>
        public int ExecuteInsertReturnAutoIncrementID(DbCommand basicInsertCmd,
            DbTransaction tran, string tableName, string autoIncrementColumn)
        {
            Check.Require(basicInsertCmd != null, "basicInsertCmd could not be null.");
            Check.Require(!string.IsNullOrEmpty(tableName), "tableName could not be null or empty.");

            string selectLastInsertIDSql = null;
            if (!string.IsNullOrEmpty(autoIncrementColumn))
            {
                Dictionary<string, string> additionalOptions = null;
                //if (this.dbProvider is DbProviders.Oracle.OracleDbProvider && (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["OracleGlobalAutoIncrementSeqeunceName"])))
                //{
                //    additionalOptions = new Dictionary<string, string>();
                //    additionalOptions.Add("OracleGlobalAutoIncrementSeqeunceName", System.Configuration.ConfigurationManager.AppSettings["OracleGlobalAutoIncrementSeqeunceName"]);
                //}
                selectLastInsertIDSql = dbProvider.Options.GetSelectLastInsertAutoIncrementIDSql(tableName, autoIncrementColumn, additionalOptions);
            }
            object retVal = 0;

            if ((!IsBatchConnection) && autoIncrementColumn != null && selectLastInsertIDSql != null)
            {

                if (dbProvider is DbProviders.SqlServer.SqlDbProvider || dbProvider.Options.SupportMultiSqlStatementInOneCommand)
                {
                    basicInsertCmd.CommandText = basicInsertCmd.CommandText + ';' + selectLastInsertIDSql;
                    if (tran == null)
                    {
                        retVal = ExecuteScalar(basicInsertCmd);
                    }
                    else
                    {
                        retVal = ExecuteScalar(basicInsertCmd, tran);
                    }

                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                //else if (dbProvider is DbProviders.Oracle.OracleDbProvider || selectLastInsertIDSql.Contains("SELECT SEQ_"))
                //{
                //    if (tran == null)
                //    {
                //        retVal = ExecuteScalar(CommandType.Text, selectLastInsertIDSql);
                //        ExecuteNonQuery(basicInsertCmd);
                //    }
                //    else
                //    {
                //        retVal = ExecuteScalar(tran, CommandType.Text, selectLastInsertIDSql);
                //        ExecuteNonQuery(basicInsertCmd, tran);
                //    }

                //    if (retVal != DBNull.Value)
                //    {
                //        return Convert.ToInt32(retVal);
                //    }
                //}
                else if (!dbProvider.Options.SupportADO20Transaction)
                {
                    DbTransaction t = (tran == null ? BeginTransaction() : tran);
                    try
                    {
                        ExecuteNonQuery(basicInsertCmd, t);
                        retVal = ExecuteScalar(t, CommandType.Text, selectLastInsertIDSql);

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch (DbException)
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseConnection(t);
                            t.Dispose();
                        }
                    }

                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else
                {
                    if (tran == null)
                    {
                        ExecuteNonQuery(basicInsertCmd);
                    }
                    else
                    {
                        ExecuteNonQuery(basicInsertCmd, tran);
                    }
                }
            }
            else
            {
                if (tran == null)
                {
                    ExecuteNonQuery(basicInsertCmd);
                }
                else
                {
                    ExecuteNonQuery(basicInsertCmd, tran);
                }
            }

            return Convert.ToInt32(retVal);
        }

        /// <summary>
        /// Executes the insert return auto increment ID.
        /// </summary>
        /// <param name="basicInsertCmd">The basic insert CMD.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="autoIncrementColumn">The auto increment column.</param>
        /// <returns></returns>
        public int ExecuteInsertReturnAutoIncrementID(DbCommand basicInsertCmd,
            string tableName, string autoIncrementColumn)
        {
            return ExecuteInsertReturnAutoIncrementID(basicInsertCmd, null, tableName, autoIncrementColumn);
        }

        #endregion

        #region Extended Query Methods

        /// <summary>
        /// Query from specified custom sql.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public CustomSqlSection CustomSql(string sql)
        {
            Check.Require(!string.IsNullOrEmpty(sql), "sql could not be null or empty!");

            return new CustomSqlSection(this, sql);
        }



        /// <summary>
        /// Query from specified stored procedure.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public StoredProcedureSection StoredProcedure(string spName)
        {
            Check.Require(!string.IsNullOrEmpty(spName), "spName could not be null or empty!");

            return new StoredProcedureSection(this, spName);
        }

        #endregion

        #region ASP.NET 1.1 style Transactions

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public DbTransaction BeginTransaction()
        {
            return GetConnection(true).BeginTransaction();
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <returns></returns>
        public DbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            return GetConnection(true).BeginTransaction(il);
        }

        #endregion

        #region DbCommand Parameter Methods

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        internal void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter parameter = CreateParameter(name, dbType == DbType.Object ? DbType.String : dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        internal void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        internal void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
        {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        internal void AddInParameter(DbCommand command, string name, DbType dbType)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        internal void AddInParameter(DbCommand command, string name, DbType dbType, object value)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>      
        internal void AddInParameter(DbCommand command, string name, object value)
        {
            AddParameter(command, name, DbType.Object, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        internal void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        #endregion

        #endregion

        #region Batch Connection

        internal DbConnection batchConnection = null;
        private BatchCommander batchCommander = null;

        /// <summary>
        /// Gets the size of a batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        internal int BatchSize
        {
            get
            {
                return batchCommander == null ? 0 : batchCommander.batchSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is batch connection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is batch connection; otherwise, <c>false</c>.
        /// </value>
        internal bool IsBatchConnection
        {
            get
            {
                return (batchConnection != null);
            }
        }

        /// <summary>
        /// Begins the batch connection.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        internal void BeginBatchConnection(int batchSize)
        {
            batchConnection = CreateConnection(true);
            //this.batchSize = batchSize;
            batchCommander = new BatchCommander(this, batchSize);
        }

        /// <summary>
        /// Begins the batch connection.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="tran">The tran.</param>
        internal void BeginBatchConnection(int batchSize, DbTransaction tran)
        {
            batchConnection = CreateConnection(true);
            //this.batchSize = batchSize;
            batchCommander = new BatchCommander(this, batchSize, tran);
        }

        /// <summary>
        /// Begins the batch connection.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="il">The il.</param>
        internal void BeginBatchConnection(int batchSize, System.Data.IsolationLevel il)
        {
            batchConnection = CreateConnection(true);
            //this.batchSize = batchSize;
            batchCommander = new BatchCommander(this, batchSize, il);
        }

        /// <summary>
        /// Ends the batch connection.
        /// </summary>
        internal void EndBatchConnection()
        {
            batchCommander.Close();
            CloseConnection(batchConnection);
            batchConnection = null;
            batchCommander = null;
        }

        /// <summary>
        /// Executes the pending batch operations.
        /// </summary>
        internal void ExecutePendingBatchOperations()
        {
            batchCommander.ExecuteBatch();
        }

        /// <summary>
        /// Begins a un disconnect connection.
        /// </summary>
        internal void BeginUnDisconnectConnection()
        {
            BeginBatchConnection(1);
        }

        /// <summary>
        /// Ends the un disconnect connection.
        /// </summary>
        internal void EndUnDisconnectConnection()
        {
            EndBatchConnection();
        }

        #endregion

        #region ILogable Members

        /// <summary>
        /// OnLog event.
        /// </summary>
        public event LogHandler OnLog;

        #endregion

        //DbCommand WrappingCommand(DbCommand command)
        //{
        //    string sqltext = command.CommandText;
        //    CommandType ct = command.CommandType;
        //    var dpc = command.Parameters;
        //    var conn = GetConnection();
        //    //if (conn.State != ConnectionState.Open) conn.Open();

        //    var wpCommand = conn.CreateCommand();
        //    wpCommand.CommandText = sqltext;
        //    wpCommand.CommandType = ct;

        //    for (int i = 0; i < dpc.Count; i++)
        //    {
        //        var pm = wpCommand.CreateParameter();
        //        pm.DbType = dpc[i].DbType;
        //        pm.ParameterName = dpc[i].ParameterName;
        //        pm.Value = dpc[i].Value;
        //        wpCommand.Parameters.Add(pm);
        //    }

        //    return wpCommand;
        //}

    }

    public class DataContext<T>
        where T : DBSchema
    {
        public T DB { get; set; }

    }
}
