using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Profiles;
using LearnIT.Application.Services;
using LearnIT.Infrastructure.Persistence;
using LearnIT.Infrastructure.TokenService;
using LearnIT.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LearnIT.Application.Interfaces;
using System.Net.Mail;
using LearnIT.Infrastructure.EmailService;
using LearnIT.Application.Interfaces.Services.UsersEmailService;

namespace LearnIT.WebUI.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(options => options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                }
                ));

            // Add JWT Authentication for Swagger UI
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "FitnessClub API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            string connectionString = builder.Configuration["LearnITDbConnectionString"] ?? throw new Exception("DBConectionString is null");
            builder.Services.AddDbContext<LearnITDBContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
            builder.Services.AddTransient<IUsersRepository, UsersRepository>();
            builder.Services.AddTransient<ITutorsRepository, TutorsRepository>();
            builder.Services.AddTransient<ISkillsRepository, SkillsRepository>();
            builder.Services.AddTransient<IGendersRepository, GendersRepository>();
            builder.Services.AddTransient<IConsultationsRepository, ConsultationsRepository>();
            builder.Services.AddTransient<IUsersService, UsersService>();
            builder.Services.AddTransient<ITutorsService, TutorsService>();
            builder.Services.AddTransient<IConsultationsService, ConsultationsService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddTransient<IEmailConfirmationService, EmailConfirmationService>();
            
            // Add JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? throw new Exception("JwtSettings:SecretKey is not configured");
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true
                };
            });
            
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ElevatedRights", policy =>
                      policy.RequireRole("Admin", "User", "Tutor"));
            });

            builder.Services.AddSingleton(
                new SmtpClient
                {
                    Credentials = new System.Net.NetworkCredential(builder.Configuration["SmtpClientSettings:UserName"], builder.Configuration["SmtpClientSettings:Password"]),
                    Host = builder.Configuration["SmtpClientSettings:Host"]!,
                    Port = int.Parse(builder.Configuration["SmtpClientSettings:Port"]!),
                    EnableSsl = true,
                    Timeout = 30000
                });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");
            app.MapControllers();
            app.Run();
        }
    }
}
