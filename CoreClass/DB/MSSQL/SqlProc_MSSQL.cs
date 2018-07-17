using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace CoreClass
{
	/// <summary>
	/// 数据库连接基类
	/// </summary>
	public class SqlProc_MSSQL
	{
		protected string m_connectionstring = null;
		protected SqlConnection m_sqlConn = null;
		protected SqlCommand m_SqlCmd = null;
		protected string m_sql = null;

		/// <summary>
		/// 打开数据库连接
		/// </summary>
		protected void SqlProcOpen()
		{
			m_sqlConn = new SqlConnection(m_connectionstring);
			m_sqlConn.Open();
		}
		/// <summary>
		/// 关闭数据库连接
		/// </summary>
		protected void SqlProcClose()
		{
			try
			{
				m_SqlCmd.Cancel();
				m_SqlCmd.Dispose();
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
				m_SqlCmd = new SqlCommand(m_sql, m_sqlConn);
				int rows = m_SqlCmd.ExecuteNonQuery();
				return Convert.ToBoolean(rows);
			}
			catch (System.Data.SqlClient.SqlException E)
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
                m_SqlCmd = new SqlCommand(m_sql, m_sqlConn);
                identity = (long)m_SqlCmd.ExecuteScalar();
                return Convert.ToBoolean(identity);
            }
            catch (System.Data.SqlClient.SqlException E)
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
				m_SqlCmd = new SqlCommand(m_sql, m_sqlConn);
				new SqlDataAdapter(m_SqlCmd).Fill(ds, "ds");
				b = true;
			}
			catch (System.Data.SqlClient.SqlException E)
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

			m_SqlCmd = new SqlCommand();
			m_SqlCmd.Connection = m_sqlConn;
			m_SqlCmd.Parameters.Clear();
			m_SqlCmd.CommandText = proceName;
			m_SqlCmd.CommandType = CommandType.StoredProcedure;

			m_SqlCmd.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, 0, 0, string.Empty, DataRowVersion.Default, true, null, null, null, null));
			SqlDataAdapter sda = new SqlDataAdapter(m_SqlCmd);
			sda.Fill(ds);

			object obj = m_SqlCmd.Parameters["@ReturnValue"].Value;

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

			SqlParameter[] parmA = ObjArrayToParmArray(procName, objA);

			m_SqlCmd = new SqlCommand();
			m_SqlCmd.Connection = m_sqlConn;
			m_SqlCmd.Parameters.Clear();
			m_SqlCmd.CommandText = procName;
			m_SqlCmd.CommandType = CommandType.StoredProcedure;

			for (int i = 0; i < parmA.Length; i++)
			{
				m_SqlCmd.Parameters.Add(parmA[i]);
			}
			m_SqlCmd.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, 0, 0, string.Empty, DataRowVersion.Default, false, null, null, null, null));

			SqlDataAdapter ada = new SqlDataAdapter(m_SqlCmd);
			ada.Fill(ds);

			object obj = m_SqlCmd.Parameters["@ReturnValue"].Value;

			bool bRetValue = false;
			if (!Convert.IsDBNull(obj))
			{
				bRetValue = Convert.ToInt32(obj) > 0;
			}

			for (int i = 0; i < parmA.Length; i++)
			{
				if (parmA[i].Direction == ParameterDirection.Output)
					objA[i] = parmA[i].Value;
			}
			return bRetValue;
		}
		/// <summary>
		/// 对象数组 转换成 参数数组
		/// </summary>
		/// <param name="procName">存储过程名</param>
		/// <param name="objA">对象数组</param>
		/// <return name="parmA">参数数组</return>
		private SqlParameter[] ObjArrayToParmArray(string procName, object[] objA)
		{
			string sqlStr = "SELECT"
				+ " syscolumns.name as ParameterName,"
				+ " systypes.name as ParameterType,"
				+ " syscolumns.length as ParameterLength,"
				+ " syscolumns.xscale as ParameterDecimalDigits,"
				+ " syscolumns.isoutparam as IsOutputParameter"
				+ " from"
				+ " syscolumns,"
				+ " systypes"
				+ " where"
				+ " systypes.name<>'sysname' and syscolumns.id=object_id('" + procName + "')"
				+ " and"
				+ " systypes.xtype=syscolumns.xtype and systypes.xusertype=syscolumns.xusertype ORDER BY syscolumns.colid";


			SqlDataAdapter dataAda = new SqlDataAdapter(sqlStr, m_sqlConn);
			DataTable dt = new DataTable();
			dataAda.Fill(dt);
			if (dt == null || dt.Rows.Count <= 0)
			{
				throw new ApplicationException(string.Format("存储过程{0}不存在！", procName));
			}
			SqlParameter[] parmA = new SqlParameter[dt.Rows.Count];
			if (parmA.Length != objA.Length)
			{
				throw new ApplicationException(string.Format("调用存储过程{0}异常，数据库需要{1}参数,程序提供{2}参数不匹配！", procName, parmA.Length, objA.Length));
			}

			for (int i = 0; i < parmA.Length; i++)
			{
				parmA[i] = new SqlParameter();

				parmA[i].ParameterName = dt.Rows[i]["ParameterName"].ToString();
				parmA[i].SqlDbType = GetSqlDBTypeFromName(dt.Rows[i]["ParameterType"].ToString());
				if (parmA[i].SqlDbType == SqlDbType.Text)
				{
					parmA[i].Size = objA[i].ToString().Length;
				}
				else
				{
					parmA[i].Size = Convert.ToInt32(dt.Rows[i]["ParameterLength"].ToString());
				}
				parmA[i].Scale = Convert.ToByte(dt.Rows[i]["ParameterDecimalDigits"].ToString());
				if (dt.Rows[i]["IsOutputParameter"].ToString() == "1")
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
		/// <return>SqlDbType</return>
		private SqlDbType GetSqlDBTypeFromName(string typeName)
		{
			SqlDbType dbtype = SqlDbType.VarChar;
			if (typeName.ToLower() == "varchar")
			{
				dbtype = SqlDbType.VarChar;
			}
			else
				if (typeName.ToLower() == "nvarchar")
				{
					dbtype = SqlDbType.NVarChar;
				}
				else
					if (typeName == "int")
					{
						dbtype = SqlDbType.Int;
					}
					else
						if (typeName.ToLower() == "tinyint")
						{
							dbtype = SqlDbType.TinyInt;
						}
						else
							if (typeName.ToLower() == "money")
							{
								dbtype = SqlDbType.Money;
							}
							else
								if (typeName.ToLower() == "decimal")
								{
									dbtype = SqlDbType.Decimal;
								}
								else
									if (typeName.ToLower() == "datetime")
									{
										dbtype = SqlDbType.DateTime;
									}
									else
										if (typeName.ToLower() == "bit")
										{
											dbtype = SqlDbType.Bit;
										}
										else
											if (typeName.ToLower() == "nchar")
											{
												dbtype = SqlDbType.NChar;
											}
											else
												if (typeName.ToLower() == "binary")
												{
													dbtype = SqlDbType.Binary;
												}
												else
													if (typeName.ToLower() == "text")
													{
														dbtype = SqlDbType.Text;
													}
													else
														if (typeName.ToLower() == "ntext")
														{
															dbtype = SqlDbType.Text;
                                                        }
                                                        else
                                                            if (typeName == "bigint")
                                                            {
                                                                dbtype = SqlDbType.BigInt;
                                                            }
			return dbtype;
		}

	}
}
