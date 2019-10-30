using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace BlueCat.ORM.DbProviders.MySql
{
    public class MySqlQueryFactory : BlueCat.ORM.SqlQueryFactory
    {
        public MySqlQueryFactory() : base('`', '`', '?', '%', '_', MySqlClientFactory.Instance)
        {
        }

        protected override void PrepareCommand(DbCommand cmd)
        {
            base.PrepareCommand(cmd);

            foreach (DbParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                {
                    continue;
                }

                object value = p.Value;
                if (value == DBNull.Value)
                {
                    continue;
                }
                Type type = value.GetType();
                MySqlParameter mySqlParam = (MySqlParameter)p;

                if (mySqlParam.DbType != DbType.Guid && type == typeof(Guid))
                {
                    mySqlParam.MySqlDbType = MySqlDbType.VarChar;
                    mySqlParam.Size = 36;
                    continue;
                }

                if ((p.DbType == DbType.Time || p.DbType == DbType.DateTime) && type == typeof(TimeSpan))
                {
                    mySqlParam.MySqlDbType = MySqlDbType.Double;
                    mySqlParam.Value = ((TimeSpan)value).TotalDays;
                    continue;
                }

                switch (p.DbType)
                {
                    case DbType.Binary:
                        if (((byte[])value).Length > 2000)
                        {
                            mySqlParam.MySqlDbType = MySqlDbType.LongBlob;
                        }
                        break;
                    case DbType.Time:
                        mySqlParam.MySqlDbType = MySqlDbType.DateTime;
                        break;
                    case DbType.DateTime:
                        mySqlParam.MySqlDbType = MySqlDbType.DateTime;
                        break;
                    case DbType.AnsiString:
                        if (value.ToString().Length > 65535)
                        {
                            mySqlParam.MySqlDbType = MySqlDbType.LongText;
                        }
                        break;
                    case DbType.String:
                        if (value.ToString().Length > 65535)
                        {
                            mySqlParam.MySqlDbType = MySqlDbType.LongText;
                        }
                        break;
                    case DbType.Object:
                        mySqlParam.MySqlDbType = MySqlDbType.LongText;
                        p.Value = SerializationManager.Instance.Serialize(value);
                        break;
                }
            }

            //replace mysql specific function names in cmd.CommandText
            cmd.CommandText = cmd.CommandText
                .Replace("LEN(", "LENGTH(")
                .Replace("GETDATE()", "NOW()")
                .Replace("GETUTCDATE()", "UTC_TIMESTAMP()")
                .Replace("DATEPART(Year,", "YEAR(")
                .Replace("DATEPART(Month,", "MONTH(")
                .Replace("DATEPART(Day,", "DAY(");

            //replace CHARINDEX with INSTR and reverse seqeunce of param items in CHARINDEX()
            int startIndexOfCharIndex = cmd.CommandText.IndexOf("CHARINDEX(");
            while (startIndexOfCharIndex > 0)
            {
                int endIndexOfCharIndex = SqlQueryUtils.GetEndIndexOfMethod(cmd.CommandText, startIndexOfCharIndex + "CHARINDEX(".Length);
                string[] itemsInCharIndex = SqlQueryUtils.SplitTwoParamsOfMethodBody(
                    cmd.CommandText.Substring(startIndexOfCharIndex + "CHARINDEX(".Length,
                    endIndexOfCharIndex - startIndexOfCharIndex - "CHARINDEX(".Length));
                cmd.CommandText = cmd.CommandText.Substring(0, startIndexOfCharIndex)
                    + "INSTR(" + itemsInCharIndex[1] + "," + itemsInCharIndex[0] + ")"
                    + (cmd.CommandText.Length - 1 > endIndexOfCharIndex ?
                    cmd.CommandText.Substring(endIndexOfCharIndex + 1) : string.Empty);

                startIndexOfCharIndex = cmd.CommandText.IndexOf("CHARINDEX(", endIndexOfCharIndex);
            }
        }


        public override bool SupportBatchInsert()
        {
            return true;
        }

        protected override string GetValuesDatas(DbType[] columnTypes, object[][] rows, DbCommand cmd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" VALUES ");
            for (int i = 0; i < rows.Length; i++)
            {
                sb.Append("(");

                for (int j = 0; j < rows[i].Length; j++)
                {
                    //var pararmName = MakeUniqueParamNameWithPrefixToken();
                   
                    sb.Append(DataUtils.ToString(columnTypes[j], rows[i][j]));

                    if (j != rows[i].Length - 1)
                    {
                        sb.Append(',');
                    }
                    //var p = cmd.CreateParameter();
                    //p.DbType = columnTypes[j];
                    //p.ParameterName = pararmName;
                    //p.Value = rows[i][j] == null ? DBNull.Value : rows[i][j];
                    //cmd.Parameters.Add(p);
                }


                sb.Append(")");
                if (i == rows.Length - 1)
                    sb.Append(";");
                else
                {
                    sb.Append(",");
                }
            }


            return sb.ToString();
            //return base.GetValuesDatas(columnTypes, rows);
        }



        public override DbCommand CreateSelectRangeCommand(WhereClip where, string[] columns, int topCount, int skipCount, string identyColumn, bool identyColumnIsNumber)
        {
            Check.Require(((object)where) != null && where.From != null, "expr and expr.From could not be null!");
            Check.Require(columns != null && columns.Length > 0, "columns could not be null or empty!");
            Check.Require(topCount > 0, "topCount must > 0!");

            if (string.IsNullOrEmpty(where.OrderBy) && identyColumn != null)
            {
                where.SetOrderBy(new KeyValuePair<string, bool>[] { new KeyValuePair<string, bool>(identyColumn, false) });
            }

            if (topCount == int.MaxValue && skipCount == 0)
            {
                return CreateSelectCommand(where, columns);
            }
            else
            {
                DbCommand cmd = CreateSelectCommand(where, columns);
                if (skipCount == 0)
                {
                    cmd.CommandText += " LIMIT " + topCount;
                }
                else
                {
                    cmd.CommandText += " LIMIT " + skipCount;
                    cmd.CommandText += "," + topCount;
                }
                return cmd;
            }
        }
    }
}
