using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Core.Mappings;
using Store.Core.Services;
using Store.Infraestructure.Data;
using Store.Infraestructure.Repositories;
using Store.Infraestructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductPriceLogRepository, ProductPriceLogRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService>(sp =>
    new AuthService(
        sp.GetRequiredService<IConfiguration>(),
        sp.GetRequiredService<IUserRepository>(),
        sp.GetRequiredService<ILogger<AuthService>>(),
        builder.Environment.EnvironmentName // Se pasa el environment
    )
);



// Configurar DbContext con SQL Server
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("str_connection")));

// Servicios de controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region configuracion JWT
// Configurar JWT (si no se encuentra en el archivo de configuracion, se buscara en las variables de entorno)
var environment = builder.Environment.EnvironmentName;

string? jwtKey = null;

if (environment == "Development")
{
    // Si esta en desarrollo, lee del appsettings o de una variable de entorno de usuario (local)
    jwtKey = builder.Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("API_KEY", EnvironmentVariableTarget.User);
}
else
{
    // Si esta en produccion (Azure u otro hosting)
    jwtKey = builder.Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("API_KEY");
}


// Validar que no sea nulo
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key is not configured.");
}

// Configurar auth JWT
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Agregando logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Fallo la autenticacion: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validado correctamente.");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("Challenge lanzado: " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };

    });

#endregion

// Configuracion de CORS (opcional, util para conectar desde un cliente)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Autenticacion y autorizacion (placeholder, luego agregar  JWT)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS si es necesario
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Para auth/roles en el futuro
app.UseAuthorization();

app.MapControllers();

app.Run();
