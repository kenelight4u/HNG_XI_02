using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using UserAuthNOrg.Core.Context;
using UserAuthNOrg.Core.Models;
using UserAuthNOrg.Utilities.Extensions;
using UserAuthNOrg.Utilities.CoreConstants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserAuthNOrg.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Claims;

namespace UserAuthNOrg.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProcessId()
                        .Enrich.WithThreadId()
                        .Enrich.WithThreadName()
                        .Enrich.WithMachineName()
                        .Enrich.WithClientIp()
                        .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            Log.Information("User Organization WEB Application Starting!!!");

            try
            {
                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("RequireLoggedIn",
                        policy => policy.RequireRole(ConstantsString.SuperAdmin, ConstantsString.OrgUser).RequireAuthenticatedUser());
                });

                builder.Services.Configure<DataProtectionTokenProviderOptions>(d => d.TokenLifespan = TimeSpan.FromMinutes(10));

                var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"].ToString());
                builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = false;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

                //var options = new JwtBearerOptions
                //{
                //    Audience = Configuration"Auth0:ApiIdentifier"],
                //    Authority = $"https://{Configuration"Auth0:Domain"]}/"
                //};
                //app.UseJwtBearerAuthentication(options);

                //Setting required Password properties
                builder.Services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.SignIn.RequireConfirmedEmail = true;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.AllowedForNewUsers = false;
                }).AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<UserAuthNOrgDbContext>();

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy(ConstantsString.RequireSuperAdmin, policy => policy.RequireRole(ConstantsString.SuperAdmin).RequireAuthenticatedUser());
                    options.AddPolicy(ConstantsString.RequiredOrgUser, policy => policy.RequireRole(ConstantsString.OrgUser).RequireAuthenticatedUser());
                });

                builder.Services.AddCors();

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();

                #region Swagger
                builder.Services.AddSwaggerGen(s =>
                {
                    s.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "User Organization API",
                        Description = "HNG XI Stage Two Task"
                    });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    s.IncludeXmlComments(xmlPath);

                    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                    s.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
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
                #endregion

                #region Api Versioning
                // Add API Versioning to the Project
                builder.Services.AddApiVersioning(config =>
                {
                    // Specify the default API Version as 1.0
                    config.DefaultApiVersion = new ApiVersion(1, 0);
                    // If the client hasn't specified the API version in the request, use the default API version number 
                    config.AssumeDefaultVersionWhenUnspecified = true;
                    // Advertise the API versions supported for the particular endpoint
                    config.ReportApiVersions = true;
                });
                #endregion


                // Add services to the container.
                builder.Services.AddDbContext<UserAuthNOrgDbContext>(options => options
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(UserAuthNOrgDbContext).Assembly.FullName)));

                builder.Services.AddHttpContextAccessor();

                builder.Logging.ClearProviders();

                builder.Logging.AddSerilog(Log.Logger);
                builder.Host.UseSerilog(Log.Logger);

                Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

                builder.Services.AddMemoryCache();

                builder.Services.AddInfrastructureServices(builder.Configuration);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Organization v1");
                    c.RoutePrefix = "swagger";
                    c.EnableFilter();
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });
               
                app.UseHeaderHandlerMiddleware();

                app.UseMiddleware<RequestResponseLoggingMiddleware>();
                app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
                app.UseUnHandledExceptionCatcher();

                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                app.UseHttpsRedirection();

                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                //Seeding default user and role to database
                var options = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider;
                UserAndRoleDataInitializer.SeedData(options);

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
