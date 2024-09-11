using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Hubs;

namespace ToDoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // SQL database connection
            builder.Services.AddDbContext<ToDoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Loading Redis connection string from appsettings.json
            var redisConnectionString = builder.Configuration.GetSection("Redis")["ConnectionString"];

            // Redis cache service
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "ToDoApp_";
            });

            // SignalR
            builder.Services.AddSignalR();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // SignalR Hub
            app.MapHub<NotificationHub>("/notifications");

            app.MapControllers();

            app.Run();
        }
    }
}
