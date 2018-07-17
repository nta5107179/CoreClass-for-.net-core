using System;
using System.Collections.Generic;
using System.Text;

namespace CoreClass
{
	/// <summary>
	/// 数组操作类
	/// </summary>
	public class OperateArrayClass
	{
		/// <summary>
		/// 移除重复的项
		/// </summary>
		/// <param name="arr">源数组</param>
		/// <returns></returns>
		public object[] RemoveRepeat(object[] arr)
		{
			List<object> list = new List<object>();
			for (int i = 0; i < arr.Length; i++)
			{
				bool b = true;
				for (int j = 0; j < list.Count; j++)
				{
					if (arr[i].Equals(list[j])) b = false;
				}
				if (b)
				{
					list.Add(arr[i]);
				}
			}
			return list.ToArray();
		}
		/// <summary>
		/// 移除重复的项
		/// </summary>
		/// <param name="arr">源泛型</param>
		/// <returns></returns>
		public List<object> RemoveRepeat(List<object> arr)
		{
			List<object> list = new List<object>();
			for (int i = 0; i < arr.Count; i++)
			{
				bool b = true;
				for (int j = 0; j < list.Count; j++)
				{
					if (arr[i].Equals(list[j])) b = false;
				}
				if (b)
				{
					list.Add(arr[i]);
				}
			}
			return list;
		}
		/// <summary>
		/// 冒泡排序
		/// </summary>
		/// <param name="arr">源数组</param>
		/// <param name="sort">排序方式 asc or desc</param>
		/// <returns></returns>
		public int[] BubbleSort(int[] arr, string sort)
		{
			for (int i = 1; i < arr.Length; i++)
			{
				for (int j = 0; j < arr.Length - i; j++)
				{
					int temp = 0;
					if (sort == "asc")
					{
						if (arr[j] > arr[j + 1])
						{
							temp = arr[j];
							arr[j] = arr[j + 1];
							arr[j + 1] = temp;
						}
					}
					else if (sort == "desc")
					{
						if (arr[j] < arr[j + 1])
						{
							temp = arr[j];
							arr[j] = arr[j + 1];
							arr[j + 1] = temp;
						}
					}
				}
			}
			return arr;
		}
		/// <summary>
		/// 判断多个数组数据是否相等
		/// </summary>
		/// <param name="objArr">数组数组</param>
		/// <returns></returns>
		public bool IsEqual(params int[][] arr)
		{
			bool b = true;
			for (int i = 0; i < arr.Length; i++)
			{
				if (i + 1 == arr.Length) break;
				if (arr[i].Length == arr[i + 1].Length)
				{
					for (int j = 0; j < arr[i].Length; j++)
					{
						if (arr[i][j] != arr[i + 1][j]) { b = false; break; }
					}
				}
				else
				{
					b = false;
					break;
				}
			}
			return b;
		}
		/// <summary>
		/// 判断多个数组数据是否相等
		/// </summary>
		/// <param name="objArr">数组数组</param>
		/// <returns></returns>
		public bool IsEqual(params string[][] arr)
		{
			bool b = true;
			for (int i = 0; i < arr.Length; i++)
			{
				if (i + 1 == arr.Length) break;
				if (arr[i].Length == arr[i + 1].Length)
				{
					for (int j = 0; j < arr[i].Length; j++)
					{
						if (arr[i][j] != arr[i + 1][j]) { b = false; break; }
					}
				}
				else
				{
					b = false;
					break;
				}
			}
			return b;
		}
		/// <summary>
		/// 判断多个数组数据是否相等
		/// </summary>
		/// <param name="objArr">数组数组</param>
		/// <returns></returns>
		public bool IsEqual(params object[][] arr)
		{
			bool b = true;
			for (int i = 0; i < arr.Length; i++)
			{
				if (i + 1 == arr.Length) break;
				if (arr[i].Length == arr[i + 1].Length)
				{
					if (!arr[i].Equals(arr[i + 1])) { b = false; break; }
				}
				else
				{
					b = false;
					break;
				}
			}
			return b;
		}

	}
}
