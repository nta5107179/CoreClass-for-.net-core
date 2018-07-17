using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CoreClass
{
	/// <summary>
	/// 数据集操作类
	/// </summary>
	public class OperateDataClass
	{
		/*
		============DataTable操作模块============
		*/
		/// <summary>
		/// 去掉重复的行
		/// </summary>
		/// <param name="dt">源数据集</param>
		/// <param name="colname">列名</param>
		/// <returns></returns>
		public DataTable Distinct(DataTable dt, string colname)
		{
			DataTable newdt = dt.Clone();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				bool b = true;
				for (int j = 0; j < newdt.Rows.Count; j++)
				{
					if (dt.Rows[i][colname].Equals(newdt.Rows[j][colname])) b = false;
				}
				if (b)
				{
					newdt.Rows.Add(dt.Rows[i].ItemArray);
				}
			}
			return newdt;
		}
		/// <summary>
		/// 交换行列
		/// </summary>
		/// <param name="dt">源数据集</param>
		/// <param name="prefix">新数据集的列名前缀</param>
		/// <param name="hascolname">是否需要将原列名放入新数据集的第一列</param>
		/// <returns></returns>
		public DataTable ExchangeRowCol(DataTable dt, string prefix, bool hascolname)
		{
			DataTable newdt = new DataTable();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				newdt = AddColumn(newdt, prefix + i, "System.String");
			}
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				newdt.Rows.Add(new object[] { null });
				for (int j = 0; j < dt.Rows.Count; j++)
				{
					newdt.Rows[i][j] = dt.Rows[j][i];
				}
			}
			if (hascolname)
			{
				newdt = AddColumn(newdt, prefix, "System.String");
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					newdt.Rows[i][prefix] = dt.Columns[i].ColumnName;
				}
			}
			return newdt;
		}
		/*
		============DataColumn操作模块============
		*/
		/// <summary>
		/// 创建新列
		/// </summary>
		/// <param name="dt">源数据集</param>
		/// <param name="colname">列名</param>
		/// <param name="systemtype">系统类型(如System.Int32)</param>
		/// <returns></returns>
		public DataTable AddColumn(DataTable dt, string colname, string systemtype)
		{
			dt.Columns.Add(colname, Type.GetType(systemtype));
			return dt;
		}
		/*
		============DataRow操作模块============
		*/
	}
}
