using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Npgsql;



namespace BlueCat.ORM.DbProviders.PostgreSql
{
    public class PostgreSqlQueryFactory : BlueCat.ORM.SqlQueryFactory
    {
        public PostgreSqlQueryFactory() : base('"', '"', ':', '%', '_', NpgsqlFactory.Instance)
        {
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
                    sb.Append(DataUtils.ToString(columnTypes[j], rows[i][j]));

                    if (j != rows[i].Length - 1)
                    {
                        sb.Append(',');
                    }
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
        }




        protected override void PrepareCommand(DbCommand cmd)
        {
            base.PrepareCommand(cmd);

            foreach (DbParameter p in cmd.Parameters)
            {
                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    p.ParameterName = string.Empty;
                }

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
                NpgsqlParameter npgParam = (NpgsqlParameter)p;

                if (npgParam.DbType != DbType.Guid && type == typeof(Guid))
                {
                    npgParam.DbType = DbType.String;
                    npgParam.Size = 36;
                    continue;
                }

                if ((p.DbType == DbType.Time || p.DbType == DbType.DateTime) && type == typeof(TimeSpan))
                {
                    npgParam.DbType = DbType.String;
                    npgParam.Value = "'" + ((TimeSpan)value).TotalDays + " days'";
                    continue;
                }

                switch (p.DbType)
                {
                    case DbType.String:
                        npgParam.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
                        p.Value = value.ToString();
                        break;
                    case DbType.Object:
                        npgParam.DbType = DbType.String;
                        p.Value = SerializationManager.Instance.Serialize(value);
                        break;
                }
            }

            //replace postgresql specific function names in cmd.CommandText
            cmd.CommandText = cmd.CommandText
                .Replace("SUBSTRING(", "substr(")
                .Replace("LEN(", "length(")
                .Replace("GETDATE()", "current_timestamp")
                .Replace("GETUTCDATE()", "LOCALTIMESTAMP")
                .Replace("DATEPART(Year", "date_part('year'")
                .Replace("DATEPART(Month", "date_part('month'")
                .Replace("DATEPART(Day", "date_part('day'");

            //replace CHARINDEX with strpos and reverse seqeunce of param items in CHARINDEX()
            int startIndexOfCharIndex = cmd.CommandText.IndexOf("CHARINDEX(");
            while (startIndexOfCharIndex > 0)
            {
                int endIndexOfCharIndex = SqlQueryUtils.GetEndIndexOfMethod(cmd.CommandText, startIndexOfCharIndex + "CHARINDEX(".Length);
                string[] itemsInCharIndex = SqlQueryUtils.SplitTwoParamsOfMethodBody(
                    cmd.CommandText.Substring(startIndexOfCharIndex + "CHARINDEX(".Length, 
                    endIndexOfCharIndex - startIndexOfCharIndex - "CHARINDEX(".Length));
                cmd.CommandText = cmd.CommandText.Substring(0, startIndexOfCharIndex) 
                    + "strpos(" + itemsInCharIndex[1] + "," + itemsInCharIndex[0] + ")" 
                    + (cmd.CommandText.Length - 1 > endIndexOfCharIndex ? 
                    cmd.CommandText.Substring(endIndexOfCharIndex + 1) : string.Empty);

                startIndexOfCharIndex = cmd.CommandText.IndexOf("CHARINDEX(", endIndexOfCharIndex);
            }
        }

        public override DbCommand CreateSelectRangeCommand(WhereClip where, string[] columns, int topCount, int skipCount, string identyColumn, bool identyColumnIsNumber)
        {
            Check.Require(((object)where) != null && where.From != null, "expr and expr.From could not be null!");
            Check.Require(columns != null && columns.Length > 0, "columns could not be null or empty!");
            Check.Require(topCount > 0, "topCount must > 0!");

            if (string.IsNullOrEmpty(where.OrderBy) && identyColumn != null)
            {
                where.SetOrderBy(new KeyValuePair<string,bool>[] { new KeyValuePair<string,bool>(identyColumn, false) });
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
                    cmd.CommandText +=  " LIMIT " + skipCount;
                    cmd.CommandText +=  " OFFSET " + topCount;
                }
                return cmd;
            }
        }
    }
}
