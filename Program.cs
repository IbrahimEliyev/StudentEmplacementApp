using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentEmplacementApp.Data;
using StudentEmplacementApp.Helper;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;
using StudentEmplacementApp.Repositories;
using System.Security.Claims;
using System.Text;

namespace StudentEmplacementApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<StudentMapping>();
            builder.Services.AddScoped<EmplacementService>();
            builder.Services.AddScoped<StudentService>();
            builder.Services.AddScoped<TokenService>();

            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Scoped
            );

            // Add Identity services without cookie authentication
            builder.Services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; 
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Unauthorized - JWT token required\"}");
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Forbidden - Access denied\"}");
                };
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        RoleClaimType = ClaimTypes.Role,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };  
                });

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Your API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' followed by a space and the token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                // XML comments
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "StudentEmplacementApp.xml"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}