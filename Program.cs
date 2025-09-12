var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 👉 CORS debe ir ANTES del Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors("AllowVue");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("API is running ✅"));

app.Run();
