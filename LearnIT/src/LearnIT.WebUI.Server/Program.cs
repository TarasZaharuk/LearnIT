using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Profiles;
using LearnIT.Application.Services;
using LearnIT.Infrastructure.Persistence;
using LearnIT.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
