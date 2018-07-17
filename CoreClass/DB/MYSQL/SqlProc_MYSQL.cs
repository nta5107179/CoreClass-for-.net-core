using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CoreClass
{
	/// <summary>
	/// 数据库连接基类
	/// </summary>
	public class SqlProc_MYSQL
	{
		protected string m_connectionstring = null;
		protected MySqlConnection m_sqlConn = null;
		protected MySqlCommand m_MySqlCmd = null;
		protected string m_sql = null;

		/// <summary>
		/// 打开数据库连接
		/// </summary>
		protected void MySqlProcOpen()
		{
			m_sqlConn = new MySqlConnection(m_connectionstring);
			m_sqlConn.Open();
		}
		/// <summary>
		/// 关闭数据库连接
		/// </summary>
		protected void MySqlProcClose()
		{
			try
			{
				//m_MySqlCmd.Cancel();
				m_MySqlCmd.Dispose();
			}
			catch { }
			try
			{
				m_sqlConn.Dispose();
				m_sqlConn.Close();
			}
			catch { }
		}
		/// <summary>
		/// 执行T-sql语句,返回执行结果
		/// </summary>
		protected bool Execute()
		{
			try
			{
				m_MySqlCmd = new MySqlCommand(m_sql, m_sqlConn);
				int rows = m_MySqlCmd.ExecuteNonQuery();
				return Convert.ToBoolean(rows);
			}
            catch (MySql.Data.MySqlClient.MySqlException E)
			{
				throw new Exception(E.Message);
			}
		}
        /// <summary>
        /// 执行T-sql语句,返回执行结果
        /// </summary>
        /// <param name="identity">返回新插入的列编号</param>
        protected bool Execute(ref long identity)
        {
            try
            {
                m_MySqlCmd = new MySqlCommand(m_sql, m_sqlConn);
                identity = (long)m_MySqlCmd.ExecuteScalar();
                return Convert.ToBoolean(identity);
            }
            catch (MySql.Data.MySqlClient.MySqlException E)
            {
                throw new Exception(E.Message);
            }
        }
		/// <summary>
		/// 执行T-sql语句,返回执数据集
		/// </summary>
		/// <param name="sql">T-sql语句</param>
		/// <param name="ds">ref 返回数据集</param>
		protected bool Execute(DataSet ds)
		{
			bool b = false;
			try
			{
				m_MySqlCmd = new MySqlCommand(m_sql, m_sqlConn);
                new MySqlDataAdapter(m_MySqlCmd).Fill(ds, "ds");
				b = true;
			}
            catch (MySql.Data.MySqlClient.MySqlException E)
			{
				throw new Exception(E.Message);
			}
			return b;
		}
		/// <summary>
		/// 不带参数的存储过程
		/// </summary>
		/// <param name="proceName">存储过程名</param>
		/// <param name="ds">ref 返回数据集</param>
		protected bool Execute(string proceName, DataSet ds)
		{
			Debug.Assert(m_sqlConn.State == ConnectionState.Open);

			m_MySqlCmd = new MySqlCommand();
			m_MySqlCmd.Connection = m_sqlConn;
			m_MySqlCmd.Parameters.Clear();
			m_MySqlCmd.CommandText = proceName;
			m_MySqlCmd.CommandType = CommandType.StoredProcedure;

			m_MySqlCmd.Parameters.Add(new MySqlParameter("_return", MySqlDbType.Int32, 4, ParameterDirection.Output, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			MySqlDataAdapter sda = new MySqlDataAdapter(m_MySqlCmd);
			sda.Fill(ds);

            object obj = m_MySqlCmd.Parameters["_return"].Value;

			bool bRetValue = false;
			if (!Convert.IsDBNull(obj))
			{
				bRetValue = Convert.ToInt32(obj) > 0;
			}

			sda.Dispose();
			return bRetValue;
		}
		/// <summary>
		/// 带参数的存储过程
		/// </summary>
		/// <param name="procName">存储过程名</param>
		/// <param name="objA">参数值数组</param>
		/// <param name="ds">返回dataset</param>
		protected bool Execute(string procName, object[] objA, DataSet ds)
		{
			Debug.Assert(m_sqlConn.State == ConnectionState.Open);
            
            object[] _objA = new object[objA.Length + 1];
            objA.CopyTo(_objA, 0);
            _objA[_objA.Length - 1] = 0;
            MySqlParameter[] parmA = ObjArrayToParmArray(procName, _objA);

			m_MySqlCmd = new MySqlCommand();
			m_MySqlCmd.Connection = m_sqlConn;
			m_MySqlCmd.Parameters.Clear();
			m_MySqlCmd.CommandText = procName;
			m_MySqlCmd.CommandType = CommandType.StoredProcedure;

			for (int i = 0; i < parmA.Length; i++)
			{
				m_MySqlCmd.Parameters.Add(parmA[i]);
			}
            //m_MySqlCmd.Parameters.Add(new MySqlParameter("_return", MySqlDbType.Int32, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));

			MySqlDataAdapter ada = new MySqlDataAdapter(m_MySqlCmd);
			ada.Fill(ds);

            object obj = m_MySqlCmd.Parameters["_return"].Value;

			bool bRetValue = false;
			if (!Convert.IsDBNull(obj))
			{
				bRetValue = Convert.ToInt32(obj) > 0;
			}

			for (int i = 0; i < parmA.Length; i++)
			{
				if (parmA[i].Direction == ParameterDirection.Output)
                    _objA[i] = parmA[i].Value;
			}
            Array.Copy(_objA, objA, objA.Length);

			return bRetValue;
		}
		/// <summary>
		/// 对象数组 转换成 参数数组
		/// </summary>
		/// <param name="procName">存储过程名</param>
		/// <param name="objA">对象数组</param>
		/// <return name="parmA">参数数组</return>
		private MySqlParameter[] ObjArrayToParmArray(string procName, object[] objA)
		{
            string sqlStr = "SELECT"
                + " PARAMETER_NAME as ParameterName,"
                + " DATA_TYPE as ParameterType,"
                + " CHARACTER_MAXIMUM_LENGTH as ParameterLength,"
                + " PARAMETER_MODE as ParameterMode"
                + " from"
                + " information_schema.PARAMETERS"
                + " where"
                + " SPECIFIC_NAME='" + procName + "'"
                + " ORDER BY ORDINAL_POSITION asc";


			MySqlDataAdapter dataAda = new MySqlDataAdapter(sqlStr, m_sqlConn);
			DataTable dt = new DataTable();
			dataAda.Fill(dt);
			if (dt == null || dt.Rows.Count <= 0)
			{
				throw new ApplicationException(string.Format("存储过程{0}不存在！", procName));
			}
			MySqlParameter[] parmA = new MySqlParameter[dt.Rows.Count];
			if (parmA.Length != objA.Length)
			{
				throw new ApplicationException(string.Format("调用存储过程{0}异常，数据库需要{1}参数,程序提供{2}参数不匹配！", procName, parmA.Length, objA.Length));
			}

			for (int i = 0; i < parmA.Length; i++)
			{
				parmA[i] = new MySqlParameter();

				parmA[i].ParameterName = dt.Rows[i]["ParameterName"].ToString();
				parmA[i].MySqlDbType = GetMySqlDBTypeFromName(dt.Rows[i]["ParameterType"].ToString());
                if (parmA[i].MySqlDbType == MySqlDbType.Text || parmA[i].MySqlDbType == MySqlDbType.LongText || parmA[i].MySqlDbType == MySqlDbType.MediumText)
				{
					parmA[i].Size = objA[i].ToString().Length;
				}
				/*else
				{
					parmA[i].Size = Convert.ToInt32(dt.Rows[i]["ParameterLength"].ToString());
				}
				parmA[i].Scale = Convert.ToByte(dt.Rows[i]["ParameterDecimalDigits"].ToString());*/
                if (dt.Rows[i]["ParameterMode"].ToString() == "OUT")
				{
					parmA[i].Direction = ParameterDirection.Output;
				}
				else
				{
					parmA[i].Direction = ParameterDirection.Input;
					if (objA[i] == null)
						parmA[i].Value = DBNull.Value;
					else
						parmA[i].Value = objA[i];
				}
			}
			return parmA;
		}
		/// <summary>
		/// 转换SQL类型
		/// </summary>
		/// <param name="typeName">类型名称</param>
		/// <return>MySqlDbType</return>
		private MySqlDbType GetMySqlDBTypeFromName(string typeName)
		{
			MySqlDbType dbtype = MySqlDbType.VarChar;
			if (typeName.ToLower() == "varchar")
			{
				dbtype = MySqlDbType.VarChar;
			}
			else
                if (typeName.ToLower() == "int32")
                {
                    dbtype = MySqlDbType.Int32;
                }
                else
                    if (typeName.ToLower() == "decimal")
                    {
                        dbtype = MySqlDbType.Decimal;
                    }
                    else
                        if (typeName.ToLower() == "timestamp")
                        {
                            dbtype = MySqlDbType.Timestamp;
                        }
                        else

                            if (typeName.ToLower() == "longtext")
                            {
                                dbtype = MySqlDbType.LongText;
                            }
                            else

                                if (typeName.ToLower() == "double")
                                {
                                    dbtype = MySqlDbType.Double;
                                }
                                else

                                    if (typeName.ToLower() == "float")
                                    {
                                        dbtype = MySqlDbType.Float;
                                    }
			return dbtype;
		}

	}
}
