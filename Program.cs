using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LimasHotel.Api.Data;
using LimasHotel.Api.Services;
using LimasHotel.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection(MongoSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(CloudinarySettings.SectionName));

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();

var cloudinarySettings = builder.Configuration.GetSection(CloudinarySettings.SectionName).Get<CloudinarySettings>()
                          ?? throw new InvalidOperationException("Seção 'Cloudinary' não configurada.");
if (string.IsNullOrWhiteSpace(cloudinarySettings.CloudName) ||
    string.IsNullOrWhiteSpace(cloudinarySettings.ApiKey) ||
    string.IsNullOrWhiteSpace(cloudinarySettings.ApiSecret))
    throw new InvalidOperationException("As credenciais do Cloudinary ('Cloudinary:CloudName', 'Cloudinary:ApiKey', 'Cloudinary:ApiSecret') devem ser configuradas.");

var cloudinaryAccount = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
builder.Services.AddSingleton(new Cloudinary(cloudinaryAccount));

var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
          ?? throw new InvalidOperationException("Seção 'Jwt' não configurada.");
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException("'Jwt:Key' deve ter ao menos 32 caracteres.");

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

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
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

                if (jti is null || await tokenService.IsRevokedAsync(jti))
                    context.Fail("Token revogado.");
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Limas Pet Hotel API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe apenas o token JWT."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
