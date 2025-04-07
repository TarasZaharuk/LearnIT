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
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
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
            builder.Services.AddTransient<IUsersService, UsersService>();
            builder.Services.AddTransient<ITutorsService, TutorsService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddTransient<IEmailConfirmationService, EmailConfirmationService>();
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
                    EnableSsl = true
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
