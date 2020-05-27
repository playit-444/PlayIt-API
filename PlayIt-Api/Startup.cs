using System;
using System.Text;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PlayIt_Api.Logging;
using PlayIt_Api.Models.Entities;
using PlayIt_Api.Services.Account;
using PlayIt_Api.Services.Game;
using PlayIt_Api.Services.GameType;
using PlayIt_Api.Services.Mail;
using PlayIt_Api.Services.Security;
using PlayIt_Api.Services.Security.Account;
using PlayIt_Api.Services.Token;

namespace PlayIt_Api
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
            //DB
            services.AddDbContext<PlayItContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")))
                .AddUnitOfWork<PlayItContext>();

            //Scoped Services
            services.AddScoped<IHashingService, SHA512HashingService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILogger, DbExceptionLogger>();
            services.AddScoped<IGameTypeService, GameTypeService>();
            services.AddScoped<IGameService, GameService>();

            services.AddScoped<IPasswordService, PasswordService>(
                service => new PasswordService(
                    new SHA512HashingService(),
                    8,
                    32));

            //Access header
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Add automapper used to convert db models to dto
            services.AddAutoMapper(typeof(Startup));

            //JSON loop
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    //Set datetime to local og hosting machine
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    //Optimize formatting
                    options.SerializerSettings.Formatting = Formatting.None;
                    //Ignore json self ReferenceLoopHandling
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Play It Api", Version = "v1"});
            });

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                    builder => builder.WithOrigins("https://localhost:4200", "https://444.dk").AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("YdCnz8X4!dvLvtu8c&q*9JSd$BZD#^P5Wrb^PsvvJm5XfxbHW3X@8YD8D4^pe8nx")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui
            // Specifying the swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play it Api v1"); });

            app.UseCors("AllowMyOrigin");
            app.UseRouting();
            app.UseCors();


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
