using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace CoreClass
{
	/// <summary>
	/// 功能操作类
	/// </summary>
	public class OperateMemoryClass
	{
		public string m_webrootpath = HttpContext.HostingEnvironment.WebRootPath;

		/*
		============邮件操作模块============
		*/
		/// <summary>
		/// SMTP邮件群发函数
		/// </summary>
		/// <param name="SmtpClientHost">获取或设置用于 SMTP 事务的主机的名称或 IP 地址</param>
		/// <param name="MessageFromName">发件人名</param>
		/// <param name="MessageFrom">发件人邮箱</param>
		/// <param name="MessageFromPwd">发件人邮箱密码</param>
		/// <param name="MessageTo">收件人</param>
		/// <param name="MessageCC">抄送人</param>
		/// <param name="MessageSubject">邮件标题</param>
		/// <param name="MessageBody">邮件内容</param>
		/// <returns>布尔</returns>
		public bool SendMailToSmtp(string SmtpClientHost, string MessageFromName, string MessageFrom, string MessageFromPwd, string[] MessageTo, string[] MessageCC, string MessageSubject, string MessageBody)
		{
			bool b = false;
			MailMessage msg = new MailMessage();
			/*添加发送人*/
			for (int i = 0; i < MessageTo.Length; i++)
			{
				msg.To.Add(MessageTo[i]);
			}
			/*添加抄送人*/
            if (MessageCC != null && MessageCC.Length > 0)
            {
                for (int i = 0; i < MessageCC.Length; i++)
                {
                    msg.CC.Add(MessageCC[i]);
                }
            }
			msg.From = new MailAddress(MessageFrom, MessageFromName, System.Text.Encoding.UTF8);
			/* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
			msg.Subject = MessageSubject;//邮件标题
			msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码
			msg.Body = MessageBody;//邮件内容
			msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
			msg.IsBodyHtml = true;//是否是HTML邮件
			msg.Priority = MailPriority.High;//邮件优先级

			SmtpClient client = new SmtpClient();
			client.Credentials = new System.Net.NetworkCredential(MessageFrom, MessageFromPwd);
			client.Host = SmtpClientHost;
			try
			{
				client.Send(msg);
				//简单一点儿可以client.Send(msg);
				b = true;
			}
			catch (SmtpException e) { throw e; }
			return b;
		}
		/// <summary>
		/// 虚拟SMTP邮件群发函数
		/// </summary>
		/// <param name="SmtpClientHost">获取或设置用于 SMTP 事务的主机的名称或 IP 地址</param>
		/// <param name="MessageFromName">发件人名</param>
		/// <param name="MessageFrom">发件人邮箱</param>
		/// <param name="MessageTo">收件人</param>
		/// <param name="MessageCC">抄送人</param>
		/// <param name="MessageSubject">邮件标题</param>
		/// <param name="MessageBody">邮件内容</param>
		/// <returns>布尔</returns>
		public bool SendMailToLocalhost(string SmtpClientHost, string MessageFromName, string MessageFrom, string[] MessageTo, string[] MessageCC, string MessageSubject, string MessageBody)
		{
			bool b = false;
			MailMessage msg = new MailMessage();
			/*添加发送人*/
			for (int i = 0; i < MessageTo.Length; i++)
			{
				msg.To.Add(MessageTo[i]);
			}
			/*添加抄送人*/
            if (MessageCC != null && MessageCC.Length > 0)
            {
                for (int i = 0; i < MessageCC.Length; i++)
                {
                    msg.CC.Add(MessageCC[i]);
                }
            }
			msg.From = new MailAddress(MessageFrom, MessageFromName, System.Text.Encoding.UTF8);
			/* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
			msg.Subject = MessageSubject;//邮件标题
			msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码
			msg.Body = MessageBody;//邮件内容
			msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
			msg.IsBodyHtml = true;//是否是HTML邮件
			msg.Priority = MailPriority.High;//邮件优先级

			SmtpClient client = new SmtpClient();
			client.Host = SmtpClientHost;
			try
			{
				client.Send(msg);
				//简单一点儿可以client.Send(msg);
				b = true;
			}
			catch (SmtpException e) { throw e; }
			return b;
		}
		/*
		============IP地址操作模块============
		*/
		/// <summary>
		/// 获取真实IP
		/// </summary>
		public string IPAddress
		{
			get
			{
				string result = String.Empty;
				result = HttpContext.Current.Connection.RemoteIpAddress.ToString();
				
				return result;
			}
		}
		/// <summary>
		/// 判断ip地址是否正确
		/// </summary>
		/// <param name="str1">待检测ip</param>
		/// <returns>布尔</returns>
		public bool IsIPAddress(string str1)
		{
			if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;
			string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
			Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
			return regex.IsMatch(str1);
		}
		/*
		============缓存操作模块============
		*/
		/// <summary>
		/// 设置缓存
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="obj">值</param>
		/// <returns>布尔</returns>
		public bool SetCache(string key, object obj)
		{
			bool b = false;
			try
			{
				MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.MaxValue);
				HttpContext.Cache.Set(key, obj, cacheEntryOptions);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 设置缓存
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="obj">值</param>
		/// <param name="path">依赖文件路径(格式：a/b/a.txt)</param>
		/// <returns>布尔</returns>
		public bool SetCache(string key, object obj, string path)
		{
			bool b = false;
			try
			{
				PhysicalFileProvider _fileProvider = new PhysicalFileProvider(m_webrootpath);

				IChangeToken changeToken = _fileProvider.Watch(path);
				MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.MaxValue)
					.AddExpirationToken(changeToken);
				HttpContext.Cache.Set(key, obj, cacheEntryOptions);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 读取缓存
		/// </summary>
		/// <param name="key">键</param>
		/// <returns>对象</returns>
		public object GetCache(string key)
		{
			object result = null;
			HttpContext.Cache.TryGetValue(key, out result);
			return result;
		}
		/// <summary>
		/// 移除缓存
		/// </summary>
		/// <param name="key">键</param>
		/// <returns>布尔</returns>
		public bool RemoveCache(string key)
		{
			bool b = false;
			try
			{
				HttpContext.Cache.Remove(key);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/*
		============cookie操作模块============
		*/
		/// <summary>
		/// 添加或修改cookie
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <param name="domain">域名</param>
		/// <returns>布尔</returns>
		public bool SetCookie(string key, string value, string domain)
		{
			bool b = false;
			try
			{
				CookieOptions co = new CookieOptions();
				//判断是否指定domain
				if (domain != null)
				{
					co.Domain = domain;
				}
				//设置
				HttpContext.Current.Response.Cookies.Append(key, value, co);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 添加或修改cookie
		/// </summary>
		/// <param name="key">名称</param>
		/// <param name="value">值</param>
		/// <param name="days">日</param>
		/// <param name="hours">时</param>
		/// <param name="minutes">分</param>
		/// <param name="second">秒</param>
		/// <param name="domain">域名</param>
		/// <returns>布尔</returns>
		public bool SetCookie(string key, string value, int days, int hours, int minutes, int second, string domain)
		{
			bool b = false;
			try
			{
				TimeSpan ts = new TimeSpan(days, hours, minutes, second);

				CookieOptions co = new CookieOptions();
				co.Expires = DateTime.Now.Add(ts);
				//判断是否指定domain
				if (domain != null)
				{
					co.Domain = domain;
				}
				//修改
				HttpContext.Current.Response.Cookies.Append(key, value, co);
				
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 获取cookie值
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>cookie值</returns>
		public string GetCookie(string name)
		{
			string cookie = null;
			try
			{
				cookie = HttpContext.Current.Request.Cookies[name];
			}
			catch { }
			return cookie;
		}
		/// <summary>
		/// 移除cookie
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="domain">域名</param>
		/// <returns>布尔</returns>
		public bool RemoveCookie(string name, string domain)
		{
			bool b = false;
			if (HttpContext.Current.Request.Cookies[name] != null)
			{
				CookieOptions co = new CookieOptions();
				//判断是否指定domain
				if (domain != null)
				{
					co.Domain = domain;
				}
				HttpContext.Current.Response.Cookies.Delete(name, co);
				b = true;
			}
			return b;
		}
		/*
		============session操作模块============
		*/
        /// <summary>
        /// 获取session id
        /// </summary>
        /// <returns>布尔</returns>
        public string GetSessionID()
        {
            return HttpContext.Current.Session.Id;
        }
		/// <summary>
		/// 添加或修改session
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <returns>布尔</returns>
		public bool SetSession(string key, string value)
		{
			bool b = false;
			try
			{
				HttpContext.Current.Session.SetString(key, value);
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 获取session值
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>session值</returns>
		public string GetSession(string key)
		{
			string session = null;
			try
			{
				session = HttpContext.Current.Session.GetString(key);
			}
			catch { }
			return session;
		}
		/// <summary>
		/// 移除session
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>布尔</returns>
		public bool RemoveSession(string name)
		{
			bool b = false;
			HttpContext.Current.Session.Remove(name);
			b = true;
			return b;
		}
		/*
		============application操作模块============
		*/
		/// <summary>
		/// 添加或修改application
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">值</param>
		/// <returns>布尔</returns>
		public bool SetApplication(string key, object value)
		{
			bool b = false;
			try
			{
				object result = null;
				HttpContext.Application.TryGetValue(key, out result);
				//application已存在
				if (result != null)
				{
					HttpContext.Application[key] = value;
				}
				//application不存在
				else
				{
					HttpContext.Application.Add(key, value);
				}
				b = true;
			}
			catch (Exception e) { throw e; }
			return b;
		}
		/// <summary>
		/// 获取application值
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>application值</returns>
		public object GetApplication(string key)
		{
			object app = null;
			try
			{
				app = HttpContext.Application[key];
			}
			catch { }
			return app;
		}
		/// <summary>
		/// 移除application
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>布尔</returns>
		public bool RemoveApplication(string name)
		{
			bool b = false;
			HttpContext.Application.Remove(name);
			b = true;
			return b;
		}
		/*
		============跨域操作模块============
		*/
		/// <summary>
		/// 跨域访问
		/// </summary>
		/// <param name="url">访问地址</param>
		/// <param name="action">参数数组(如["action1=1","action2=2"]),可为null</param>
		/// <param name="domain">域名(如果是子域名跨域),可为null</param>
		/// <returns>返回访问页面的输出内容</returns>
		public string CDA(string url, string[] action, string domain)
		{
			CookieContainer craboCookie = new CookieContainer();
			IRequestCookieCollection hcc = HttpContext.Current.Request.Cookies;
			CookieCollection ncc = new CookieCollection();
			foreach (var item in hcc)
			{
				Cookie c = new Cookie(item.Key, item.Value);
				if (domain != null)
				{
					c.Domain = domain;
				}
				ncc.Add(c);
			}
			craboCookie.Add(ncc);

			string act = "";
			if (action != null)
			{
				act = "?" + string.Join("&", action);
			}
			HttpWebRequest wrq = (HttpWebRequest)WebRequest.Create(url + act);
			wrq.KeepAlive = false;
			wrq.CookieContainer = craboCookie;
			HttpWebResponse wrs = (HttpWebResponse)wrq.GetResponse();
			Stream sr = wrs.GetResponseStream();
			StreamReader reader = new StreamReader(sr, System.Text.Encoding.UTF8);
			string callback = reader.ReadToEnd();
			wrs.Close();
			return callback;
		}
		/*
		============序列化操作模块============
		*/
		/// <summary>
		/// 序列化
		/// </summary>
		/// <param name="dy">需要序列化的类型</param>
		/// <returns></returns>
        public byte[] Serialize(object dy)
		{
			byte[] _byte = null;
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream ms = new MemoryStream();
			bf.Serialize(ms, dy);
			_byte = ms.ToArray();
			return _byte;
		}
		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="_byte">需要反序列化的byte[]</param>
		/// <returns></returns>
        public object Deserialize(byte[] _byte)
		{
			BinaryFormatter ou_bf = new BinaryFormatter();
			MemoryStream ms = new MemoryStream(_byte);
			ms.Seek(0, SeekOrigin.Begin);
			return ou_bf.Deserialize(ms);
		}
        /*
        ============HTTP方式提交与访问============
        */
        /// <summary>
        /// HTTP方式提交与访问
        /// </summary>
        /// <param name="type">提交方式（post,get）</param>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public string HttpWebRequest(string type, string url, string data)
        {
            string str = "";
            byte[] dataArr = Encoding.UTF8.GetBytes(data);
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            switch (type.ToLower())
            {
                case "get":
                    hwr.Method = "get";
                    break;
                case "post":
                    hwr.ContentType = "application/x-www-form-urlencoded";
                    hwr.Method = "post";
                    hwr.ContentLength = dataArr.Length;
                    using (Stream s = hwr.GetRequestStream())
                    {
                        s.Write(dataArr, 0, dataArr.Length);
                    }
                    break;
            }
            WebResponse wr = hwr.GetResponse();
            using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
            {
                str = sr.ReadToEnd();
            }
            wr.Close();
            return str;
        }
        /// <summary>
        /// HTTP方式提交与访问
        /// </summary>
        /// <param name="type">提交方式（post,get）</param>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <param name="cookies">cookies</param>
        /// <returns></returns>
        public string HttpWebRequest(string type, string url, string data, CookieContainer cookies)
        {
            string str = "";
            byte[] dataArr = Encoding.UTF8.GetBytes(data);
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.CookieContainer = cookies;
            switch (type.ToLower())
            {
                case "get":
                    hwr.Method = "get";
                    break;
                case "post":
                    hwr.ContentType = "application/x-www-form-urlencoded";
                    hwr.Method = "post";
                    hwr.ContentLength = dataArr.Length;
                    using (Stream s = hwr.GetRequestStream())
                    {
                        s.Write(dataArr, 0, dataArr.Length);
                    }
                    break;
            }
            WebResponse wr = hwr.GetResponse();
            using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
            {
                str = sr.ReadToEnd();
            }
            wr.Close();
            return str;
        }


	}
}
