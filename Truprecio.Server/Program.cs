using Truprecio.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Truprecio.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // ? Conexión a BD
            builder.Services.AddDbContext<TruPreciosContext>(opciones =>
            {
                opciones.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL"));
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
