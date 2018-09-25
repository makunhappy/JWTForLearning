using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using test.Helpers;
using test.Services;


namespace test
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
            services.AddCors();
            services.AddMvc();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = appSettingsSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = appSettingsSection["Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                x.IncludeErrorDetails = true;
            });
            services.AddAuthorization(Options =>
            {
                Options.AddPolicy("TrainedStaffOnly", option => option.RequireClaim("hehe"));
            });

            // configure DI for application services
            services.AddSingleton<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            //var audienceConfig = Configuration.GetSection("AppSettings");
            //var symmetricKeyAsBase64 = audienceConfig["Secret"];
            //var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            //var signingKey = new SymmetricSecurityKey(keyByteArray);
            //var SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //app.UseAuthentication(new TokenProviderOptions
            //{
            //    Audience = audienceConfig["Audience"],
            //    Issuer = audienceConfig["Issuer"],
            //    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=values}/{action=get}");

            });
        }
    }
}
