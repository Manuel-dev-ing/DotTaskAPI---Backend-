using DotTaskAPI.Entidades;
using DotTaskAPI.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Servicios
builder.Services.AddControllers();

builder.Services.AddTransient<IRepositorioProyectos, RepositorioProyectos>();
builder.Services.AddTransient<IRepositorioTareas, RepositorioTareas>();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer("name=DefaultConnection");

});


var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
