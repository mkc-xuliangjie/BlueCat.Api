using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FluentValidation.AspNetCore;

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


            ////配置跨域处理
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("any", builder => builder
            //        .AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowCredentials()
            //        .SetPreflightMaxAge(TimeSpan.FromMinutes(6)));
            //});

            services.AddMvc().AddMvcOptions(option =>
            {
                //option.Filters.Add<LogAttribute>();
                //option.Filters.Add<ExceptionAttribute>();
                //option.Filters.Add(new ServiceFilterAttribute(typeof(Filters.ValidateUserFilterAttribute)));
            })
           .AddFluentValidation(fv =>
           {
               fv.RegisterValidatorsFromAssemblyContaining<Startup>();  // 当前程序集 https://fluentvalidation.net/aspnet#asp-net-core
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //允许跨域全局设置
            //app.UseCors("any");

            //// 保证在 Mvc 之前调用
            //app.UseHttpContextGlobal()
            //   .UseToolTrace();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
