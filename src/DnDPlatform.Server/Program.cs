using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Implementations;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Implementations;
using DnDPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DnDDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ICharacterRepository, EfCharacterRepository>();
builder.Services.AddScoped<ICharacterSheetRepository, EfCharacterSheetRepository>();
builder.Services.AddScoped<ITemplateRepository, EfTemplateRepository>();
builder.Services.AddScoped<IAuditLogRepository, EfAuditLogRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICharacterCreationService, CharacterCreationService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IVersionSnapshotManager, VersionSnapshotManager>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// event bus for the observer pattern
builder.Services.AddSingleton<IEventBus, InProcessEventBus>();

// enabling caching
builder.Services.AddMemoryCache();

// HTTP Client for D&D 5e API
builder.Services.AddHttpClient("DnD5e", client =>
{
    client.BaseAddress = new Uri("https://www.dnd5eapi.co");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// JWT Authentication for the JWT token
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// CORS implmentation to work with distributed app
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// wire up Observer/Event subscriptions
var eventBus = app.Services.GetRequiredService<IEventBus>();
var auditScope = app.Services.CreateScope();
var auditLogService = auditScope.ServiceProvider.GetRequiredService<IAuditLogService>();

eventBus.Subscribe<CharacterSheetSavedEvent>(e =>
    auditLogService.LogAsync(
        e.UserId, e.Username,
        e.IsSnapshot ? AuditAction.Snapshot : AuditAction.Save,
        ResourceType.CharacterSheet, e.SheetId,
        new { e.CharacterId, e.VersionNumber }));

eventBus.Subscribe<CharacterCreatedEvent>(e =>
    auditLogService.LogAsync(
        e.UserId, e.Username,
        AuditAction.Create,
        ResourceType.Character, e.CharacterId,
        new { e.CharacterName }));

eventBus.Subscribe<CharacterDeletedEvent>(e =>
    auditLogService.LogAsync(
        e.UserId, e.Username,
        AuditAction.Delete,
        ResourceType.Character, e.CharacterId,
        new { e.CharacterName }));

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// apply migrations in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DnDDbContext>();
    db.Database.Migrate();
}

app.Run();
