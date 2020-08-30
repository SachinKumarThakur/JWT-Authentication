using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrgDataLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OrganizationAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(x => x.EnableEndpointRouting = false);
            //services.AddMvc(x => x.Filters.Add(new AuthorizeFilter())).AddXmlSerializerFormatters()
            //                  .AddXmlDataContractSerializerFormatters();
            services.AddMvc().AddXmlSerializerFormatters()
                             .AddXmlDataContractSerializerFormatters();

            services.AddDbContext<OrganizationDbContext>();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<OrganizationDbContext>()
                .AddDefaultTokenProviders();

            //This Below Section for JWT Json Web Token Auth
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-secret-key"));
            var tokenValidationParameter = new TokenValidationParameters()
            {
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                //ValidateLifetime=false,
                //ClockSkew=TimeSpan.Zero
            };


            services.AddAuthentication(x => x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(jwt =>
                    {
                        //jwt.SaveToken = true;
                        //jwt.RequireHttpsMetadata = true;
                        jwt.TokenValidationParameters = tokenValidationParameter;
                    }
                    );
            //This Section is for Cookies Based Authentication
            //services.ConfigureApplicationCookie(opt =>
            //{
            //    opt.Events = new CookieAuthenticationEvents
            //    {
            //        OnRedirectToLogin = redirectContext =>
            //          {
            //              redirectContext.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //              return Task.CompletedTask;
            //          },
            //        OnRedirectToAccessDenied= redirectContext =>
            //        {
            //            redirectContext.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            return Task.CompletedTask;
            //        }
            //    };
            //});
            //services.AddControllers();
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseOpenApi();
            app.UseSwaggerUi3();
            //app.UseRouting();
            //app.UseAuthentication();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    //endpoints.MapGet("/", async context =>
            //    //{
            //    //    await context.Response.WriteAsync("Hello World!");
            //    //});
            //});
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
