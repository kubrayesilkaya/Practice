using Microsoft.EntityFrameworkCore;
using Practice.Model.DBContext;
using Practice.Services;
using Practice.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    options.AddPolicy("Local", p => p.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());
});

// Add services to the container.
#region Dependency Injection Management
services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddScoped<IAuthService, AuthService>();
#endregion

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

app.UseHttpsRedirection();

app.UseCors("Local");

app.UseAuthorization();

app.MapControllers();

app.Run();
