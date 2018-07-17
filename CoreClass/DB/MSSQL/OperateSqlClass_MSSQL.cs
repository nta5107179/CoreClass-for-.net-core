using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CoreClass
{
	/// <summary>
	/// T-Sql语句 存储过程执行类
	/// </summary>
	public class OperateSqlClass_MSSQL : SqlProc_MSSQL
	{
		/// <summary>
		/// 打开数据库连接
		/// </summary>
		public void Open()
		{
			m_connectionstring = ConnectionStrings.GetConnectionString;
			SqlProcOpen();
		}
		/// <summary>
		/// 打开数据库连接
		/// </summary>
		public void Open(string connectionstringname)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json");
			IConfiguration Configuration = builder.Build();

			m_connectionstring = Configuration[connectionstringname];
			SqlProcOpen();
		}
		/// <summary>
		/// 关闭数据库连接
		/// </summary>
		public void Close()
		{
			SqlProcClose();
		}
		/// <summary>
		/// 数据库操作-添加行
		/// </summary>
		/// <param name="sql">T-Sql语句</param>
		/// <returns></returns>
		public bool Insert(string sql)
		{
			bool b = false;
			m_sql = sql;
			b = Execute();
			return b;
		}
        /// <summary>
        /// 数据库操作-添加行
        /// </summary>
        /// <param name="sql">T-Sql语句</param>
        /// <param name="identity">返回的新增列号</param>
        /// <returns></returns>
        public bool Insert(string sql, ref long identity)
        {
            bool b = false;
            m_sql = sql;
            sql += ";select CAST(SCOPE_IDENTITY() as bigint)";
            b = Execute(ref identity);
            return b;
        }
		/// <summary>
		/// 数据库操作-添加行
		/// </summary>
		/// <param name="objarr">目标实体数组</param>
		/// <returns></returns>
		public bool Insert(object[] objarr)
		{
			bool b = false;
			try
			{
				for (int i = 0; i < objarr.Length; i++)
				{
					object obj = objarr[i];
					string[] cellarr = getIdentityCells(obj.GetType().Name);
					string[] strarr = getProperties(obj, "add", cellarr);
					if (strarr != null)
					{
						//str格式为a,b,c,|1,2,3,
						string[] arr = strarr;
						string sql = string.Format("insert into {0}({1}) values({2})", obj.GetType().Name, arr[0].Substring(0, arr[0].Length - 1), arr[1].Substring(0, arr[1].Length - 1));
                        m_sql = sql;
						b = Execute();
					}
				}
			}
			catch (Exception e) { throw e; }
			return b;
		}
        /// <summary>
        /// 数据库操作-添加行
        /// </summary>
        /// <param name="objarr">目标实体数组</param>
        /// <param name="identity">返回的新增列号</param>
        /// <returns></returns>
        public bool Insert(object[] objarr, ref long[] identity)
        {
            bool b = false;
            try
            {
                for (int i = 0; i < objarr.Length; i++)
                {
                    object obj = objarr[i];
                    string[] cellarr = getIdentityCells(obj.GetType().Name);
                    string[] strarr = getProperties(obj, "add", cellarr);
                    if (strarr != null)
                    {
                        //str格式为a,b,c,|1,2,3,
                        string[] arr = strarr;
                        string sql = string.Format("insert into {0}({1}) values({2})", obj.GetType().Name, arr[0].Substring(0, arr[0].Length - 1), arr[1].Substring(0, arr[1].Length - 1));
                        sql += ";select CAST(SCOPE_IDENTITY() as bigint)";
                        m_sql = sql;
                        b = Execute(ref identity[i]);
                    }
                }
            }
            catch (Exception e) { throw e; }
            return b;
        }
		/// <summary>
		/// 数据库操作-删除行
		/// </summary>
		/// <param name="sql">T-Sql语句</param>
		/// <returns></returns>
		public bool Delete(string sql)
		{
			bool b = false;
			m_sql = sql;
			b = Execute();
			return b;
		}
		/// <summary>
		/// 数据库操作-删除行
		/// </summary>
		/// <param name="objarr">目标实体数组</param>
		/// <returns></returns>
		public bool Delete(object[] objarr)
		{
			bool b = false;
			try
			{
				for (int i = 0; i < objarr.Length; i++)
				{
					object obj = objarr[i];
					string[] strarr = getProperties(obj, "del", null);
					if (strarr != null)
					{
						//str格式为a,b,c,|1,2,3,
						string[] arr = strarr;
						string str = "";
						string[] arr_0 = arr[0].Substring(0, arr[0].Length - 1).Split(',');
						string[] arr_1 = arr[1].Substring(0, arr[1].Length - 1).Split(',');
						if (arr_0.Length == arr_1.Length)
						{
							for (int j = 0; j < arr_0.Length; j++)
							{
								str += string.Format(" and {0}={1}", arr_0[j], arr_1[j]);
							}
						}
						string sql = string.Format("delete {0} where 1=1{1}", obj.GetType().Name, str);

						m_sql = sql;
						b = Execute();
					}
				}
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 数据库操作-修改行
		/// </summary>
		/// <param name="sql">T-Sql语句</param>
		/// <returns></returns>
		public bool Update(string sql)
		{
			bool b = false;
			m_sql = sql;
			b = Execute();
			return b;
		}
		/// <summary>
		/// 数据库操作-修改行
		/// </summary>
		/// <param name="objarr">修改前实体数组(指定条件)</param>
		/// <param name="objarr2">修改后实体数组(指定修改内容)</param>
		/// <returns></returns>
		public bool Update(object[] objarr, object[] objarr2)
		{
			bool b = false;
			try
			{
				//判断新旧实体数组数量是否相等
				if (objarr.Length == objarr2.Length)
				{
					for (int i = 0; i < objarr.Length; i++)
					{
						object obj = objarr[i], obj2 = objarr2[i];
						string[] cellarr = getIdentityCells(obj.GetType().Name), cellarr2 = getIdentityCells(obj2.GetType().Name);
						string[] strarr = getProperties(obj, "edit", null), strarr2 = getProperties(obj2, "edit", null);
						if (strarr != null && strarr2 != null)
						{
							//str格式为a,b,c,|1,2,3,
							string[] arr = strarr, arr2 = strarr2;
							string str = "", str2 = "";
							string[] arr_0 = arr[0].Substring(0, arr[0].Length - 1).Split(','), arr_02 = arr2[0].Substring(0, arr2[0].Length - 1).Split(',');
							string[] arr_1 = arr[1].Substring(0, arr[1].Length - 1).Split(','), arr_12 = arr2[1].Substring(0, arr2[1].Length - 1).Split(',');
							if (arr_0.Length == arr_1.Length && arr_02.Length == arr_12.Length)
							{
								for (int j = 0; j < arr_0.Length; j++)
								{
									str += string.Format(" and {0}={1}", arr_0[j], arr_1[j]);
								}
								for (int k = 0; k < arr_02.Length; k++)
								{
									/*bool iscontinue = false;
									for (int j = 0; j < arr_0.Length; j++)
									{
										if (arr_02[k] == arr_0[j]) iscontinue = true;
									}
									if (iscontinue) continue;*/
									str2 += string.Format("{0}={1},", arr_02[k], arr_12[k]);
								}
							}
							string sql = string.Format("update {0} set {1} where 1=1{2}", obj.GetType().Name, str2.Substring(0, str2.Length-1), str);

							m_sql = sql;
							b = Execute();
						}
					}
				}
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 数据库操作-查询数据
		/// </summary>
		/// <param name="sql">执行语句</param>
		/// <returns>dataset数据集</returns>
		public DataSet Select(string sql)
		{
			DataSet ds = new DataSet();
			m_sql = sql;
			Execute(ds);
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				return ds;
			}
			return null;
		}
		/// <summary>
		/// 数据库操作-查询数据
		/// </summary>
		/// <param name="proceName">存储过程名</param>
		/// <returns>dataset数据集</returns>
		public bool Selectproce(string proceName, ref DataSet ds)
		{
			bool b = Execute(proceName, ds);
			return b;
		}
		/// <summary>
		/// 数据库操作-查询数据
		/// </summary>
		/// <param name="proceName">存储过程名</param>
		/// <param name="objarr">参数对象数组</param>
		/// <returns>dataset数据集</returns>
		public bool Selectproce(string proceName, object[] objarr, ref DataSet ds)
		{
			bool b = Execute(proceName, objarr, ds);
			return b;
		}

		/// <summary>
		/// 得到表中的自增长列
		/// </summary>
		/// <param name="tablename">表名</param>
		/// <returns></returns>
		private string[] getIdentityCells(string tablename)
		{
			DataSet ds = new DataSet();
			string sql = string.Format("select [name] from syscolumns where id=object_id('{0}') and COLUMNPROPERTY(id,name, 'IsIdentity')=1", tablename);
			m_sql = sql;
			Execute(ds);
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				int k = ds.Tables[0].Rows.Count;
				string[] arr = new string[k];
				for (int i = 0; i < k; i++)
				{
					arr[i] = ds.Tables[0].Rows[i]["name"].ToString();
				}
				return arr;
			}
			return null;
		}
		/// <summary>
		/// 获取实体的属性和值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">实体</param>
		/// <param name="arr">自增长列数组</param>
		/// <returns></returns>
		private string[] getProperties<T>(T t, string tp, string[] arr)
		{
			string strname = null, strvalue = null;
			if (t == null)
			{
				return null;
			}
			PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			if (properties.Length <= 0)
			{
				return null;
			}
			foreach (PropertyInfo item in properties)
			{
				string name = item.Name;
				object value = item.GetValue(t, null);
				if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
				{
					Type type = Type.GetType("System");
					switch(tp)
					{
						//添加
						case "add":
						//去掉自增长列
						bool b1 = false;
						if (arr != null)
						{
							for (int i = 0; i < arr.Length; i++)
							{
								if (name.ToLower() != arr[i].ToLower())
								{
									b1 = true;
									break;
								}
							}
						}
						else
						{
							b1 = true;
						}
						if (b1 && value != null)
						{
							//列名
							strname += string.Format("{0},", name);
							//值
							type = value.GetType();
							if (type == Type.GetType("System.String") || type == Type.GetType("System.DateTime"))
							{
								strvalue += string.Format("'{0}',", value);
							}
							else if (type == Type.GetType("System.Boolean"))
							{
								strvalue += string.Format("{0},", Convert.ToInt32(value));
							}
							else
							{
								strvalue += string.Format("{0},", value);
							}
						}
						break;
						//删除
						case "del":
						if (value != null)
						{
							//列名
							strname += string.Format("{0},", name);
							//值
							type = value.GetType();
							if (type == Type.GetType("System.String") || type == Type.GetType("System.DateTime"))
							{
								strvalue += string.Format("'{0}',", value);
							}
							else if (type == Type.GetType("System.Boolean"))
							{
								strvalue += string.Format("{0},", Convert.ToInt32(value));
							}
							else
							{
								strvalue += string.Format("{0},", value);
							}
						}
						break;
						//修改
						case "edit":
						if (value != null)
						{
							//列名
							strname += string.Format("{0},", name);
							//值
							type = value.GetType();
							if (type == Type.GetType("System.String") || type == Type.GetType("System.DateTime"))
							{
								strvalue += string.Format("'{0}',", value);
							}
							else if (type == Type.GetType("System.Boolean"))
							{
								strvalue += string.Format("{0},", Convert.ToInt32(value));
							}
							else
							{
								strvalue += string.Format("{0},", value);
							}
						}
						break;
					}
				}
				else
				{
					getProperties(value, tp, arr);
				}
			}
			return new string[] { strname, strvalue };
		}


	}
}
