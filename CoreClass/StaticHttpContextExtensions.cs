using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreClass
{
	public static class StaticHttpContextExtensions
    {
		public static void AddHttpContextAccessor(this IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<HttpApplicationStatecs>();
		}

		public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
		{
			IHttpContextAccessor httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
			HttpContext.ConfigureHttpContext(httpContextAccessor);

			IHostingEnvironment env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
			HttpContext.ConfigureHostingEnvironment(env);

			IMemoryCache cache = app.ApplicationServices.GetRequiredService<IMemoryCache>();
			HttpContext.ConfigureMemoryCache(cache);

			HttpApplicationStatecs application = app.ApplicationServices.GetRequiredService<HttpApplicationStatecs>();
			HttpContext.ConfigureApplication(application);

			return app;
		}
	}
}
