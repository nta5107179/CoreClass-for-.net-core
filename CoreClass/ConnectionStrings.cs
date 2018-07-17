using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CoreClass
{
	/// <summary>
	///ConnectionStrings 连接字串类
	/// </summary>
	public static class ConnectionStrings
	{
		readonly static string sKey = "xiao0xiong.mao_";

		static string m_connectionstring = null;
		/// <summary>
		/// 得到连接字
		/// </summary>
		public static string GetConnectionString
		{
			get
			{
                /*
                //MSSQL
	            server=192.168.199.10;database=M2;uid=sa;pwd=sa.sa1
	            //MYSQL
	            server=127.0.0.1;database=panda;uid=root;pwd=;charset=utf8;
                */
                if (m_connectionstring == null)
				{
					IConfigurationBuilder builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json");
					IConfiguration Configuration = builder.Build();

					m_connectionstring = Configuration["ConnectionString"];
				}
				return m_connectionstring;
			}
		}
	}
}