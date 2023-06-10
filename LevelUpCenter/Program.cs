using LevelUpCenter.LookUrClimb.Services;
using LevelUpCenter.LookUrClimb.Domain.Repositories;
using LevelUpCenter.LookUrClimb.Domain.Services;
using LevelUpCenter.LookUrClimb.Persistence.Repositories;
using LevelUpCenter.Security.Authorization.Handlers.Implementations;
using LevelUpCenter.Security.Authorization.Handlers.Interfaces;
using LevelUpCenter.Security.Authorization.Middleware;
using LevelUpCenter.Security.Authorization.Settings;
using LevelUpCenter.Security.Domain.Repositories;
using LevelUpCenter.Security.Domain.Services;
using LevelUpCenter.Security.Persistence.Repositories;
using LevelUpCenter.Security.Services;
using LevelUpCenter.Shared.Persistence.Contexts;
using LevelUpCenter.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add API Documentation Information
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "LookUrClimb API",
        Description = "LookUrClimb RESTful API",
        TermsOfService = new Uri("https://lookurclimb.com/tos"),
        Contact = new OpenApiContact
        {
            Name = "LookUrClimb.studio",
            Url = new Uri("https://lookurclimb.studio")
        },
        License = new OpenApiLicense
        {
            Name = "LookUrClimb Resources License",
            Url = new Uri("https://lookurclimb.com/license")
        }
    });
});

// Add CORS
builder.Services.AddCors();

// AppSettings Configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


// Add database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseMySQL(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

// Add lowercase routes
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Dependency injection configuration
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IPublicationRepository, PublicationRepository>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IPublicationService, PublicationService>();

// Security Injection Configuration
builder.Services.AddScoped<IJwtHandler, JwtHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


// AutoMapper Configuration
builder.Services.AddAutoMapper(
 
    typeof(LevelUpCenter.LookUrClimb.Mapping.ModelToResourceProfile),
 
    typeof(LevelUpCenter.Security.Mapping.ModelToResourceProfile),
 
    typeof(LevelUpCenter.LookUrClimb.Mapping.ResourceToModelProfile),
 
    typeof(LevelUpCenter.Security.Mapping.ResourceToModelProfile)
    );

var app = builder.Build();

// Validation for ensuring database objects are created
using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<AppDbContext>())
{
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure CORS 
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// Configure Error Handler Middleware
app.UseMiddleware<ErrorHandlerMiddleware>();
// Configure JWT Handling
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();