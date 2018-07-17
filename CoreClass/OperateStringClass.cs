using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CoreClass
{
    /// <summary>
    /// 字符串操作类
    /// </summary>
    public class OperateStringClass
    {
        readonly string DESkey = "xiongmao";
        readonly Dictionary<string, string> m_escapeDic = new Dictionary<string, string>()
        {
            {"\"", "&quot;"},
            {"'", "&apos;"},
            {"<", "&lt;"},
            {">", "&gt;"},
            {",", "&#44;"}
        };

        /*
        ============加密模块============
        */
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>密文</returns>
        public string MD5Encryption(string password)
        {
			string strResult = null;
			using (var md5 = MD5.Create())
			{
				byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
				strResult = Encoding.UTF8.GetString(result);
			}
			return strResult.Replace("-", "");
		}
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="Text">内容</param>
        /// <returns></returns>
        public string DESEncryption(string Text)
        {
            string sKey = DESkey;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(Text);
            des.Key = Encoding.UTF8.GetBytes(sKey);
            des.IV = Encoding.UTF8.GetBytes(sKey);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString().ToLower();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="Text">内容</param>
        /// <returns></returns>
        public string DESDecryption(string Text)
        {
            string sKey = DESkey;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            int len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            for (int x = 0; x < len; x++)
            {
                int i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.UTF8.GetBytes(sKey);
            des.IV = Encoding.UTF8.GetBytes(sKey);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        /*
        ============转义模块============
        */
        /// <summary>
        /// 转意
        /// </summary>
        /// <param name="str">需要转意的字符串</param>
        /// <returns>处理后字符串</returns>
        public string Escape(string str)
        {
			if (str == null)
				return null;
            try
            {
                foreach (string key in m_escapeDic.Keys)
                {
                    str = str.Replace(key, m_escapeDic[key]);
                }
                //str = HttpContext.Current.Server.HtmlEncode(str);
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 转意
        /// </summary>
        /// <param name="s">需要转意的字符串对象</param>
        /// <returns>处理后字符串</returns>
        public string Escape(object s)
        {
            string str = null;
            try
            {
                str = s.ToString();
                foreach (string key in m_escapeDic.Keys)
                {
                    str = str.Replace(key, m_escapeDic[key]);
                }
                //str = HttpContext.Current.Server.HtmlEncode(str);
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 反转意
        /// </summary>
        /// <param name="str">需要反转意的字符串</param>
        /// <returns>处理后字符串</returns>
        public string unEscape(string str)
		{
			if (str == null)
				return null;
			try
            {
                //str = HttpContext.Current.Server.HtmlDecode(str);
                foreach (string key in m_escapeDic.Keys)
                {
                    str = str.Replace(m_escapeDic[key], key);
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 反转意
        /// </summary>
        /// <param name="s">需要反转意的字符串对象</param>
        /// <returns>处理后字符串</returns>
        public string unEscape(object s)
        {
            string str = null;
            try
            {
                str = s.ToString();
                //str = HttpContext.Current.Server.HtmlDecode(str);
                foreach (string key in m_escapeDic.Keys)
                {
                    str = str.Replace(m_escapeDic[key], key);
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 删除Script标记
        /// </summary>
        /// <param name="Htmlstring">字符串对象</param>
        /// <returns>处理后字符串</returns>
        public string RemoveScriptTags(string Htmlstring)
        {
            try
            {
                //删除脚本
                Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>|</script>|&lt;script.*?&gt;|&lt;/script&gt;", "", RegexOptions.IgnoreCase);
            }
            catch { }
            return Htmlstring;
        }
        /// <summary>
        /// 删除html标记
        /// </summary>
        /// <param name="Htmlstring">html字符串</param>
        /// <returns>处理后字符串</returns>
        public string RemoveHtmlTags(string Htmlstring)
        {
            try
            {
                //删除脚本  
                Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*? />", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                //删除HTML
                Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring.Replace("<", "");
                Htmlstring.Replace(">", "");
                Htmlstring.Replace("\r\n", "");
            }
            catch { }
            return Htmlstring;
        }
        /// <summary>
        /// 删除html标记
        /// </summary>
        /// <param name="s">html字符串对象</param>
        /// <returns>处理后字符串</returns>
        public string RemoveHtmlTags(object s)
        {
            string Htmlstring = "";
            try
            {
                Htmlstring = s.ToString();
                //删除脚本  
                Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*? />", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                //删除HTML
                Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring.Replace("<", "");
                Htmlstring.Replace(">", "");
                Htmlstring.Replace("\r\n", "");
            }
            catch { }
            return Htmlstring;
        }
        /*
        ============截字符串模块============
        */
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="length">长度</param>
        /// <returns>处理后字符串</returns>
        public string CutString(string s, int length)
        {
            string str = s;
            try
            {
                if (str.Length > length)
                {
                    str = str.Substring(0, length);
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="length">长度</param>
        /// <returns>处理后字符串</returns>
        public string CutString(object s, int length)
        {
            string str = "";
            try
            {
                str = s.ToString();
                if (str.Length > length)
                {
                    str = str.Substring(0, length);
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="length">长度</param>
        /// <param name="add">后缀</param>
        /// <returns>处理后字符串</returns>
        public string CutString(string s, int length, string add)
        {
            string str = s;
            try
            {
                if (str.Length > length)
                {
                    str = str.Substring(0, length) + add;
                }
                else
                {
                    str += add;
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="length">长度</param>
        /// <param name="add">后缀</param>
        /// <returns>处理后字符串</returns>
        public string CutString(object s, int length, string add)
        {
            string str = "";
            try
            {
                str = s.ToString();
                if (str.Length > length)
                {
                    str = str.Substring(0, length) + add;
                }
                else
                {
                    str += add;
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="beforelength">起始位置</param>
        /// <param name="length">结束位置（起始位置开始）</param>
        /// <returns>处理后字符串</returns>
        public string CutString(string s, int beforelength, int length)
        {
            string str = s;
            try
            {
                if (str.Length > beforelength + length)
                {
                    str = str.Substring(beforelength, length);
                }
                else
                {
                    str = str.Substring(beforelength);
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="beforelength">起始位置</param>
        /// <param name="length">结束位置（起始位置开始）</param>
        /// <returns>处理后字符串</returns>
        public string CutString(object s, int beforelength, int length)
        {
            string str = "";
            try
            {
                str = s.ToString();
                if (str.Length > beforelength + length)
                {
                    str = str.Substring(beforelength, length);
                }
                else
                {
                    str = str.Substring(beforelength);
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="beforelength">起始位置</param>
        /// <param name="length">结束位置（起始位置开始）</param>
        /// <param name="add">后缀</param>
        /// <returns>处理后字符串</returns>
        public string CutString(string s, int beforelength, int length, string add)
        {
            string str = s;
            try
            {
                if (str.Length > beforelength + length)
                {
                    str = str.Substring(beforelength, length) + add;
                }
                else
                {
                    str = str.Substring(beforelength) + add;
                }
            }
            catch { }
            return str;
        }
        /// <summary>
        /// 截字符串
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="beforelength">起始位置</param>
        /// <param name="length">结束位置（起始位置开始）</param>
        /// <param name="add">后缀</param>
        /// <returns>处理后字符串</returns>
        public string CutString(object s, int beforelength, int length, string add)
        {
            string str = "";
            try
            {
                str = s.ToString();
                if (str.Length > beforelength + length)
                {
                    str = str.Substring(beforelength, length) + add;
                }
                else
                {
                    str = str.Substring(beforelength) + add;
                }
            }
            catch { }
            return str;
        }
        /*
        ============空值判断模块============
        */
        /// <summary>
        /// 判断数组中是否有值为空
        /// </summary>
        /// <param name="arr">需要检测的数组</param>
        /// <returns></returns>
        public bool DecideNull(string[] arr)
        {
            bool b = false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null || arr[i] == "")
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        /// <summary>
        /// 判断数组中是否有包含检测的字符
        /// </summary>
        /// <param name="arr">需要检测的数组</param>
        /// <param name="parr">包含检测的字符枚举</param>
        /// <returns></returns>
        public bool DecideNull(string[] arr, params string[] parr)
        {
            bool b = false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (parr != null)
                {
                    for (int j = 0; j < parr.Length; j++)
                    {
                        if (arr[i] == parr[j])
                        {
                            b = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (arr[i] == null)
                    {
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }
        /*
        ============生成随机数模块============
        */
        /// <summary>
        /// 生成伪随机数
        /// </summary>
        /// <returns></returns>
        public int RandomNumber()
        {
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            int i = rd.Next();
            return i;
        }
        /// <summary>
        /// 生成伪随机数
        /// </summary>
        /// <param name="starnum">启始值</param>
        /// <param name="endnum">结束值</param>
        /// <returns></returns>
        public int RandomNumber(int starnum, int endnum)
        {
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            int i = rd.Next(starnum, endnum);
            return i;
        }
		/*
        ============防注入模块============
        */
		/// <summary>
		/// 检测数据库注入
		/// <param name="str">待检测的字符串数组</param>
		/// <param name="arr">需要检测的字符串数组，默认为（",", ";", "\"", "'"）</param>
		/// </summary>
		/// <returns>true为有注入，false为没有注入</returns>
		public bool DetectSql(string[] strarr, string[] arr = null)
        {
            bool b = false;
            arr = arr!=null ? arr : new string[] { ",", "\"", "'" };

            for (int i = 0; i < strarr.Length; i++)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    if (strarr[i]!=null && strarr[i].Contains(arr[j]))
                    {
                        b = true;
                        break;
                    }
                }
                if (b)
                {
                    break;
                }
            }
            return b;
        }
		/// <summary>
		/// 敏感字符屏蔽
		/// <param name="str">待检测的字符串</param>
		/// <param name="arr">需要检测的字符串数组默认为null</param>
		/// </summary>
		/// <returns>替换后的字符串</returns>
		public string WordShield(string str, string[] arr = null)
		{
			if (str != null)
			{
				for (int j = 0; j < arr.Length; j++)
				{
					if (str != null)
					{
						str = Regex.Replace(str, arr[j], "*", RegexOptions.IgnoreCase);
					}
				}
			}
			return str;
		}
		/// <summary>
		/// 敏感字符屏蔽
		/// <param name="str">待检测的字符串数组</param>
		/// <param name="arr">需要检测的字符串数组默认为null</param>
		/// </summary>
		/// <returns>替换后的字符串数组</returns>
		public string[] WordShield(string[] str, string[] arr = null)
		{
			if (str != null)
			{
				for (int i = 0; i < str.Length; i++)
				{
					for (int j = 0; j < arr.Length; j++)
					{
						if (str[i] != null)
						{
							str[i] = Regex.Replace(str[i], arr[j], "*", RegexOptions.IgnoreCase);
						}
					}
				}
			}
			return str;
		}
		/*
        ============进制转换============
        */
		/// <summary>
		/// 从汉字转换到16进制
		/// </summary>
		/// <param name="s">需要要转换的汉字</param>
		/// <param name="charset">编码,如"utf-8","gb2312"</param>
		/// <param name="separator">分隔符，没有则为null</param>
		/// <returns></returns>
		public string StrToAry16(string s, string charset, string separator)
        {
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            char[] arrChar = s.ToCharArray();
            List<string> arrStr = new List<string>();
            for (int i = 0; i < arrChar.Length; i++)
            {
                byte[] bytes = chs.GetBytes(arrChar[i].ToString());
                for (int j = 0; j < bytes.Length; j++)
                {
                    arrStr.Add(string.Format("{0:X2}", bytes[j]));
                }
            }
            string str = string.Join(separator != null ? separator : "", arrStr);
            return str.ToLower();
        }
        /// <summary>
        /// 从16进制转换成汉字
        /// </summary>
        /// <param name="hex">需要转换的16进制</param>
        /// <param name="charset">编码,如"utf-8","gb2312"</param>
        /// <param name="separator">分隔符，没有则为null</param>
        /// <returns></returns>
        public string Ary16ToStr(string hex, string charset, string separator)
        {
            // 需要将 hex 转换成 byte 数组。 
            string[] arr = hex.Split(new string[] { separator }, StringSplitOptions.None);
            byte[] bytes = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                bytes[i] = Convert.ToByte(arr[i], 16);
            }
            Encoding chs = Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
		}
		/*
        ============Json转换============
        */
		/// <summary>
		/// 转成json格式
		/// </summary>
		/// <param name="dt">DataTable对象</param>
		/// <returns></returns>
		public JArray ToJsonArray(DataTable dt)
		{
			JArray jarr = new JArray();
			DataColumnCollection dcc = dt.Columns;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				JObject jobjRow = new JObject();
				for (int j = 0; j < dcc.Count; j++)
				{
					jobjRow.Add(dcc[j].ColumnName, dt.Rows[i][dcc[j].ColumnName].ToString());
				}
				jarr.Add(jobjRow);
			}
			return jarr;
		}
		/// <summary>
		/// 转成json格式
		/// </summary>
		/// <param name="ds">DataSet</param>
		/// <returns></returns>
		public JArray ToJsonArray(DataSet ds)
		{
			JArray jarrDS = new JArray();
			for (int k = 0; k < ds.Tables.Count; k++)
			{
				DataTable dt = ds.Tables[k];
				JArray jarrDT = new JArray();
				DataColumnCollection dcc = dt.Columns;
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					JObject jobjRow = new JObject();
					for (int j = 0; j < dcc.Count; j++)
					{
						jobjRow.Add(dcc[j].ColumnName, dt.Rows[i][dcc[j].ColumnName].ToString());
					}
					jarrDT.Add(jobjRow);
				}
				jarrDS.Add(jarrDT);
			}
			return jarrDS;
		}
		/// <summary>
		/// 转成json格式
		/// </summary>
		/// <param name="obj">object实体对象</param>
		/// <returns></returns>
		public JObject ToJson(object obj)
		{
			JObject jobj = new JObject();
			try
			{
				jobj = JObject.Parse(JsonConvert.SerializeObject(obj));
			}
			catch (Exception e)
			{
				throw e;
			}
			return jobj;
		}
		/// <summary>
		/// 转成json格式
		/// </summary>
		/// <param name="list">object实体列表对象</param>
		/// <returns></returns>
		public JArray ToJsonArray(List<object> list)
		{
			JArray jarr = new JArray();
			foreach (object obj in list)
			{
				JObject jobj = null;
				try
				{
					jobj = JObject.Parse(JsonConvert.SerializeObject(obj));
				}
				catch (Exception e)
				{
					throw e;
				}
				jarr.Add(jobj);
			}
			return jarr;
		}
		/// <summary>
		/// 转成json格式
		/// </summary>
		/// <param name="arr">object实体列表对象</param>
		/// <returns></returns>
		public JArray ToJson(object[] arr)
		{
			JArray jarr = new JArray();
			foreach (object obj in arr)
			{
				JObject jobj = null;
				try
				{
					jobj = JObject.Parse(JsonConvert.SerializeObject(obj));
				}
				catch (Exception e)
				{
					throw e;
				}
				jarr.Add(jobj);
			}
			return jarr;
		}

	}
}
