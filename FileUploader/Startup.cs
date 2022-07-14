using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using FileUploader.Service;
using FileUploader.Service.Data;
using FileUploader.Service.Interfaces;
using FileUploader.Service.Services;
using FileUploader.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FileUploader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddDbContext<FileUploaderDbContext>(options =>
            {
                options.UseSqlServer(Configuration["SqlServerConnectionString"]);
            });
            
            
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IFileService, FileService>();

            services.AddSingleton<AwsCloudProvider.AwsCloudProvider>();
            services.AddSingleton<AzureCloudProvider.AzureCloudProvider>();
            services.AddSingleton<ICloudProviderFactory, CloudProviderFactory>();
            services.AddSingleton<ISecretManager, AzureSecretManger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FileUploaderDbContext context)
        {
            context.Database.Migrate();
            
            app.UseSwagger();
            app.UseSwaggerUI();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseExceptionHandler(options => {
                    options.Run(
                        async context => { await HandleGlobalException(context); });
                }
            );
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapControllers(); 
            });
        }
        private static async Task HandleGlobalException(HttpContext context) {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionObject != null) {
                object response;
                if (int.TryParse(exceptionObject.Error.Data[Shared.Constants.General.StatusCode]?.ToString(), out var statusCode)) {
                    context.Response.StatusCode = statusCode;
                    response = new Dictionary<string, string> {
                        {"message", $"{exceptionObject.Error.Message}"},
                    };
                } else {
                    response = new Dictionary<string, string> {
                        {"message", $"{exceptionObject.Error.Message}"},
                    };
                }
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response)).ConfigureAwait(false);
            }
        }
    }
}