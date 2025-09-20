using Microsoft.EntityFrameworkCore;
using parcial2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<dbparcialContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ParcialDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ParcialDb"))));


// Add services to the container.
//Cors ANY


builder.Services.AddCors(options =>
{
    options.AddPolicy("ANY",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// CORS restringido 

builder.Services.AddCors(options =>
{
    options.AddPolicy("private",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // solo React local
                  .WithHeaders("Content-Type", "Authorization") // solo estos headers
                  .WithMethods("GET", "POST"); // solo GET y POST
        });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ANY");

app.UseAuthorization();

app.MapControllers();

app.Run();
