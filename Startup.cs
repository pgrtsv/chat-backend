using System;
using System.IO;
using System.Threading.Tasks;
using ChatBackend.Chat.DataAccess;
using ChatBackend.Chat.Domain;
using ChatBackend.Chat.Domain.Validators;
using ChatBackend.Chat.Hubs;
using ChatBackend.Persons.DataAccess;
using ChatBackend.Persons.Domain;
using ChatBackend.Persons.Hubs;
using ChatBackend.Users.Domain.Validators;
using ChatBackend.Users.Hubs;
using ChatBackend.Users.Services;
using FluentMigrator.Runner;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
#pragma warning disable 1591

namespace ChatBackend
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
            services.AddHttpContextAccessor();

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
                Console.WriteLine("Environment variable DB_CONNECTION_STRING is not set");
            
            services
                .AddScoped(_ => new NpgsqlConnection(connectionString))
                .AddScoped<IUserService, UserService>()
                .AddScoped<IUserValidatorsService, UserService>()
                .AddScoped<IChatValidatorsRepository, ChatValidatorsRepository>()
                .AddScoped<IChatRepository, ChatRepository>()
                .AddScoped<IPersonsRepository, PersonsRepository>()
                .AddScoped<IPersonsRealTimeService, PersonsRealTimeService>()
                // .AddScoped<IUserStore<User>, UserService>()
                // .AddScoped<IRoleStore<UserRole>, UserService>()
                ;


            services
                .AddControllers()
                .AddFluentValidation(configuration =>
                {
                    configuration.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                ;

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                // .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                // {
                //     options.LoginPath = "/User/login";
                //     options.LogoutPath = "/User/logout";
                //     options.Events.OnRedirectToLogin = context =>
                //     {
                //         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //         return Task.CompletedTask;
                //     };
                //     options.Events.OnRedirectToAccessDenied = context =>
                //     {
                //         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //         return Task.CompletedTask;
                //     };
                // })
                ;
            // services.AddAuthorization(options =>
            // {
            //     options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            //         .RequireAuthenticatedUser()
            //         .Build();
            // });


            services.AddFluentMigratorCore()
                .ConfigureRunner(builder => builder
                    .AddPostgres11_0()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(Startup).Assembly).For.Migrations())
                .AddLogging(builder => builder.AddFluentMigratorConsole());

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "chat_backend",
                    new OpenApiInfo
                    {
                        Title = "chat_backend",
                        Version = GetVersion(),
                    });
                var xmlDocsFilePath = Path.Combine(AppContext.BaseDirectory, "ChatBackend.xml");
                options.IncludeXmlComments(xmlDocsFilePath);

                options.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Please insert JWT with Bearer into field",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "jwt_auth",
                            },
                        },
                        Array.Empty<string>()
                    },
                });
            });
            services.AddFluentValidationRulesToSwagger();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var cors_origins = Environment.GetEnvironmentVariable("CORS_ORIGINS");
            if (string.IsNullOrEmpty(cors_origins))
                Console.WriteLine("Environment variable CORS_ORIGINS is not set");
            
            app.UseCors(x => x.WithOrigins(cors_origins.Split(';', 
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/chat_backend/swagger.json", $"chat_backend {GetVersion()}");
                options.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<UserHub>("/userhub").RequireAuthorization();
                endpoints.MapHub<ChatHub>("/chathub").RequireAuthorization();
                endpoints.MapHub<PersonsHub>("/personshub").RequireAuthorization();
            });

            using var scope = app.ApplicationServices.CreateScope();
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
        }

        /// <summary>
        /// Возвращает строковое представление текущей версии API
        /// </summary>
        /// <returns></returns>
        public string GetVersion() => $"v{typeof(Startup).Assembly.GetName().Version!.ToString(2)}";
    }
}
