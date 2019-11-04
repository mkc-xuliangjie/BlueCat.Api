using BlueCat.Repository;
using BlueCat.Service.DependencyInjection;
using BuleCat.Common.DependencyInjection;
using BuleCat.Common.Extensions;
using BuleCat.Common.Http.Filters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using BlueCat.MySqlRepository.DependencyInjection;
using BlueCat.NLog.Layout;
using BlueCat.NLog.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BuleCat.Common.Cache.DependencyInjection;
using StackExchangeRedis.DependencyInjection;

namespace BlueCat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppSettings(Configuration);

            services.AddServices();

            services.AddMongodbRepository();

            services.AddMySqlRepositoryServices();

            services.AddBlueCatMongoNLogServices();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ////配置跨域处理
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(6)));
            });

            services.AddMvc().AddMvcOptions(option =>
            {
                option.Filters.Add<LogAttribute>();
                option.Filters.Add<ExceptionAttribute>();
            })
           .AddFluentValidation(fv =>
           {
               fv.RegisterValidatorsFromAssemblyContaining<Startup>();  // 当前程序集 https://fluentvalidation.net/aspnet#asp-net-core
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            //services.AddRedisSettings(Configuration);

            services.AddStackExchangeRedis(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //允许跨域全局设置
            app.UseCors("any");

            //// 保证在 Mvc 之前调用
            app.UseHttpContextGlobal()
               .UseToolTrace();

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
