using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Serilog;
using Serilog.Formatting.Compact;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.Common.Behaviors;
using enterprise_travel_and_expense_management_system.Features.Common.Middleware;
using enterprise_travel_and_expense_management_system.Services;
using enterprise_travel_and_expense_management_system.Infrastructure.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(new CompactJsonFormatter())
    .WriteTo.File(
        new CompactJsonFormatter(),
        path: "logs/app-.json",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "TravelExpenseManagement")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Travel & Expense Management API",
        Version = "v1",
        Description = "Enterprise system for managing corporate travel requests and expense tracking",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Support Team",
            Email = "support@example.com"
        }
    });

    // Include XML documentation
    var xmlFile = $"enterprise-travel-and-expense-management-system.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT security definition
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Add HTTP context accessor for AuthorizerBehavior
builder.Services.AddHttpContextAccessor();

// Add DbContext - Use SQL Server in production, SQLite in development
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (builder.Environment.IsProduction())
    {
        // Use SQL Server in production (Docker)
        options.UseSqlServer(connectionString
            ?? throw new InvalidOperationException("DefaultConnection not configured"));
    }
    else
    {
        // Use SQLite in development
        options.UseSqlite(connectionString ?? "Data Source=TravelExpenseManagement.db");
    }
});

// Register File Service for receipt handling
builder.Services.AddScoped<IFileService, FileService>();

// Register JWT Token Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "your-super-secret-key-min-32-characters-long!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TravelExpenseApp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TravelExpenseAppUsers";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Register AutoMapper for DTO mappings
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add in-memory caching
builder.Services.AddMemoryCache();

// Add MediatR with pipeline behaviors
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register pipeline behaviors (order matters - first registered runs LAST in pipeline, so LAST executes FIRST)
// 1. CachingBehavior - checks cache first (registered last, runs first)
// 2. PerformanceBehavior - measures total execution time
// 3. AuthorizerBehavior - checks role-based authorization
// 4. ValidationBehavior - validates request data (registered first, runs last)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizerBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

var app = builder.Build();

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable static files to serve receipts from wwwroot
app.UseStaticFiles();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapControllers();

// Apply migrations and seed database on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        if (dbContext.Database.IsSqlServer())
        {
            // Current migration snapshot is SQLite-based, so for SQL Server we bootstrap schema directly.
            logger.LogInformation("Ensuring SQL Server database is created...");
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("SQL Server database is ready");
        }
        else
        {
            logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations completed");
        }

        // Seed database with initial data
        await DatabaseSeeder.SeedDatabaseAsync(dbContext, logger);
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred during database migration or seeding");
    throw;
}

app.Run();
