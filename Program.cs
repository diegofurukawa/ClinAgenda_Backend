// Program.cs - com adições para autenticação

using System.Text;
using ClinAgenda.src.Application.AppointmentUseCase;
using ClinAgenda.src.Application.AuthUseCase;
using ClinAgenda.src.Application.DoctorUseCase;
using ClinAgenda.src.Application.PatientUseCase;
using ClinAgenda.src.Application.Services;
using ClinAgenda.src.Application.SpecialtyUseCase;
using ClinAgenda.src.Application.StatusUseCase;
using ClinAgenda.src.Core.Interfaces;
using ClinAgenda.src.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Configurações do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured")))
    };

    // Para pegar o token dos cabeçalhos dos requests HTTP
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
            }
            return Task.CompletedTask;
        }
    };
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Configuração do Swagger para incluir autenticação JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClinAgenda API",
        Version = "v1",
        Description = "API para o sistema de agendamento ClinAgenda"
    });

    // Configuração para autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' [espaço] seu token. Exemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configuração da conexão com MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<MySqlConnection>(_ => new MySqlConnection(connectionString));

// Registrar JwtService
builder.Services.AddScoped<JwtService>();

// Novos repositórios para autenticação
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Repositórios
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorSpecialtyRepository, DoctorSpecialtyRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();


// Novo caso de uso para autenticação
builder.Services.AddScoped<AuthUseCase>();

// Casos de uso
builder.Services.AddScoped<StatusUseCase>();
builder.Services.AddScoped<SpecialtyUseCase>();
builder.Services.AddScoped<PatientUseCase>();
builder.Services.AddScoped<DoctorUseCase>();
builder.Services.AddScoped<AppointmentUseCase>();


// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:8080") // Adicione a origem do seu front-end
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowSpecificOrigin");

// Adicionar Authentication e Authorization ao pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();