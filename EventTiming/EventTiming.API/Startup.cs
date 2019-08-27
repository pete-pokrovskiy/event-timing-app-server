using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventTiming.API.Auth;
using EventTiming.API.Helpers;
using EventTiming.API.Models;
using EventTiming.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;

namespace EventTiming.API
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public static readonly ILoggerFactory SqlCommandLoggerFactory = new LoggerFactory(new[] {
            new ConsoleLoggerProvider
            ((category, level) => (category == DbLoggerCategory.Query.Name || category == DbLoggerCategory.Database.Command.Name
            || category == DbLoggerCategory.Update.Name)
        && level == LogLevel.Information, true) });

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;

        }

        public void ConfigureServices(IServiceCollection services)
        {    
            services.AddCors(options =>
            {
                options.AddPolicy("AngularApp", builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                //.WithOrigins("http://localhost:4200")
                //.AllowAnyMethod()
                //.AllowAnyHeader()
                );
            });

            services.AddMvc()               
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

            var dbConnectionString = _config.GetConnectionString("EventTimingAppConnectionString");
            services.AddDbContext<EventTimingDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionString)
                                .UseLoggerFactory(SqlCommandLoggerFactory)
                                .EnableSensitiveDataLogging(); ;
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<EventTimingDbContext>();

            var jwtAppSettingsOptions = _config.GetSection(nameof(JwtIssuerOptions));

            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options => {
               options.ClaimsIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
               options.SaveToken = true;
               options.TokenValidationParameters = tokenValidationParameters;
           });

            services.AddAuthorization();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            //});

            services.AddTransient<IEventTimingDbSeeder, EventTimingDbSeeder>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseCors("AngularApp");

            app.UseMvc();


            //убедимся, что все миграции накачены
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<EventTimingDbContext>();
                context.Database.Migrate();
            }

            //делаем сид базы только если включена настройка в конфиге
            if (_config.GetValue<bool>("Seeding:Enabled"))
            {
                //обязательно нужен скоуп
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<IEventTimingDbSeeder>();
                    seeder.Seed();
                }
            }

        }
    }
}
