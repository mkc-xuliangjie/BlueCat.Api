using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Xml;



namespace NBearLite.DbProviders.Oracle
{
    /// <summary>
    /// <para>Represents an Oracle Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses Oracle .NET Managed Provider from Microsoft (System.Data.OracleClient) to connect to the database.
    /// </para>  
    /// </remarks>
    public class OracleDbProvider : DbProvider
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public OracleDbProvider(string connStr)
            : base(connStr, OracleClientFactory.Instance)
        {
            CheckOptionsConfiguration(new OracleDbProviderOptions(), "OracleProviderOptions");
        }

        public override NBearLite.ISqlQueryFactory GetQueryFactory()
        {
            return new NBearLite.DbProviders.Oracle.OracleQueryFactory();
        }

        #endregion
    }
}