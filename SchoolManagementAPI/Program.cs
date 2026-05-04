//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using OfficeOpenXml;
//using QuestPDF.Infrastructure;
//using SchoolManagementAPI.DB;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Controllers
//builder.Services.AddControllers();

//// CORS (lock this down in prod)
//var allowedOrigins = builder.Configuration
//    .GetSection("WebUrl:AllowedOrigins")
//    .Get<string[]>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularClient", policy =>
//    {
//        policy.WithOrigins(allowedOrigins!)
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});
////builder.Services.AddCors(options =>
////{
////    options.AddPolicy("AllowAngularClient", policy =>
////    {
////        //policy.WithOrigins("http://localhost:4200") // change in prod
////        //      .AllowAnyHeader()
////        //      .AllowAnyMethod();

////        policy.WithOrigins("https://smartschoolserp.com", "http://localhost:4200")
////              .AllowAnyHeader()
////              .AllowAnyMethod();

////    });
////});

//// Licenses
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//QuestPDF.Settings.License = LicenseType.Community;

//// Database
//builder.Services.AddDbContext<SchoolManagementDBContext>(options =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    options.UseMySql(
//        connectionString,
//        new MySqlServerVersion(new Version(8, 0, 44))
//    );
//});

//// 🔐 Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = true; // 🔥 PROD SAFE
//    options.SaveToken = false;

//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],

//        IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
//        ),

//        ClockSkew = TimeSpan.FromSeconds(30) // small tolerance
//    };

//    // 🔍 Logging (safe)
//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = context =>
//        {
//            context.NoResult();
//            context.Response.StatusCode = 401;
//            return Task.CompletedTask;
//        }
//    };
//});

//// Authorization
//builder.Services.AddAuthorization();

//// Swagger (dev only)
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "School Management API",
//        Version = "v1"
//    });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey,
//        In = ParameterLocation.Header,
//        Description = "Bearer {token}"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//builder.Services.AddEndpointsApiExplorer();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseCors("AllowAngularClient");

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.Run();



using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using QuestPDF.Infrastructure;
using SchoolManagementAPI.DB;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// CORS
var allowedOrigins = builder.Configuration
    .GetSection("WebUrl:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Licenses
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
QuestPDF.Settings.License = LicenseType.Community;

// Database (Primary DbContext)
builder.Services.AddDbContext<SchoolManagementDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 44)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()
    );
});

// ADD THIS (Factory for Export / Batch Operations)
// Normal DbContext (used everywhere)
builder.Services.AddDbContext<SchoolManagementDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 44)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()
    );
});

// DbContext Factory (used for Export only)
builder.Services.AddDbContextFactory<SchoolManagementDBContext>(
    options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        options.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 44)),
            mysqlOptions => mysqlOptions.EnableRetryOnFailure()
        );
    },
    ServiceLifetime.Scoped
);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        ),
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School Management API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<FileService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
