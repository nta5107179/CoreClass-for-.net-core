using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;

namespace CoreClass
{
    public static class HttpContext
    {
		private static IHttpContextAccessor _contextAccessor;
		public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor.HttpContext;
		internal static void ConfigureHttpContext(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		private static IHostingEnvironment _hostingEnvironment;
		public static IHostingEnvironment HostingEnvironment => _hostingEnvironment;
		internal static void ConfigureHostingEnvironment(IHostingEnvironment evn)
		{
			_hostingEnvironment = evn;
		}

		private static IMemoryCache _memoryCache;
		public static IMemoryCache Cache => _memoryCache;
		internal static void ConfigureMemoryCache(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		private static HttpApplicationStatecs _application;
		public static HttpApplicationStatecs Application => _application;
		internal static void ConfigureApplication(HttpApplicationStatecs application)
		{
			_application = application;
		}
	}
}
