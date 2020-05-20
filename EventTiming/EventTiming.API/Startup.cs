using System;
using System.Text;
using EventTiming.API.Infrastructure.Auth;
using EventTiming.Data;
using EventTiming.Logic.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace EventTiming.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        // TODO: починить логирование SQL-запросов
        //public static readonly ILoggerFactory SqlCommandLoggerFactory = new LoggerFactory(new[] {
        //    new ConsoleLoggerProvider
        //    ((category, level) => (category == DbLoggerCategory.Query.Name || category == DbLoggerCategory.Database.Command.Name
        //    || category == DbLoggerCategory.Update.Name)
        //&& level == LogLevel.Information, true) });

        public Startup(IWebHostEnvironment env, IConfiguration config)
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

            services.AddHttpContextAccessor();

            services.AddMvc()               
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

            var dbConnectionString = _config.GetConnectionString("EventTimingAppConnectionString");
            services.AddDbContext<EventTimingDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionString)
                                //.UseLoggerFactory(SqlCommandLoggerFactory)
                                .EnableSensitiveDataLogging(); ;
            });

            services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                    .AddEntityFrameworkStores<EventTimingDbContext>();

            services.AddSingleton<IJwtFactory, JwtFactory>();

            //services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

            var jwtAppSettingsOptions = _config.GetSection(nameof(JwtIssuerOptions));

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingsOptions[nameof(JwtIssuerOptions.SecretKey)]));


            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

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
            services.AddScoped<ICurrentUserDataService, CurrentUserDataService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseEndpoints(ep => ep.MapControllers());


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
