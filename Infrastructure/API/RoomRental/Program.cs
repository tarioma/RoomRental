using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Abstraction.Security;
using RoomRental.Application.UseCases.Services;
using RoomRental.Contracts.Responses;
using RoomRental.Dal;
using RoomRental.ExceptionHandling;
using RoomRental.Infrastructure;
using RoomRental.Infrastructure.Repositories;
using RoomRental.Infrastructure.Security;
using RoomRental.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options
        .UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatabaseContext>());
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer не задан.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience не задан.")
    .Validate(
        o => (o.Key?.Length ?? 0) >= JwtOptions.MinKeyLength,
        $"Jwt:Key должен быть не короче {JwtOptions.MinKeyLength} символов.")
    .Validate(o => o.LifetimeMinutes > 0, "Jwt:LifetimeMinutes должен быть положительным.")
    .ValidateOnStart();

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<DatabaseSeeder>();

builder.Services
    .AddOptions<SeedOptions>()
    .Bind(builder.Configuration.GetSection(SeedOptions.SectionName))
    .Validate(o => !o.Enabled || !string.IsNullOrWhiteSpace(o.AdminEmail), "Seed:AdminEmail не задан.")
    .Validate(
        o => !o.Enabled || (o.AdminPassword?.Length ?? 0) >= SeedOptions.MinAdminPasswordLength,
        $"Seed:AdminPassword должен быть не короче {SeedOptions.MinAdminPasswordLength} символов.")
    .ValidateOnStart();

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Описания эндпоинтов, полей запросов и ответов берутся из XML-комментариев
    // двух сборок: контроллеров и контрактов.
    string[] documentationFiles =
    [
        Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"),
        Path.Combine(AppContext.BaseDirectory, $"{typeof(CreatedResponse).Assembly.GetName().Name}.xml"),
    ];

    foreach (var documentationFile in documentationFiles.Where(File.Exists))
    {
        options.IncludeXmlComments(documentationFile, includeControllerXmlComments: true);
    }

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Токен из POST /api/auth/login",
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = [],
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var currentAssembly = typeof(SearchServicesCase).Assembly;
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssembly));

var app = builder.Build();

// Схема и начальные данные накатываются на старте, чтобы проект поднимался одной командой.
// В проде миграции обычно выносят в отдельный шаг деплоя, а сид выключают через Seed:Enabled.
await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.MigrateAsync();

    if (scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value.Enabled)
    {
        await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
    }
}

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();