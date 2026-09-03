using System.Text.Json.Serialization;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Infrastructure.Data;
using EducationalCenter.Infrastructure.Repositories;
using EducationalCenter.Infrastructure.Services;
using EducationalCenter.web.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repositories & Unit of Work (The missing pieces)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 3. Business Logic Services
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. AutoMapper (Perfectly configured to scan the Web assembly)
builder.Services.AddAutoMapper(config => { }, typeof(Program).Assembly);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();