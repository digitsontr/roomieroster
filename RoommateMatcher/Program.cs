using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Configuration;
using RoommateMatcher.Localization;
using RoommateMatcher.Models;
using RoommateMatcher.Services;
using RoommateMatcher.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvyzqxw1234567890._";

    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
})
.AddUserValidator<UserValidator>()
.AddPasswordValidator<PasswordValidator>()
.AddErrorDescriber<LocalizationIdentityErrorDescriber>()
.AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddAutoMapper(typeof(Program));


var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero,
    };
    opts.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            return Task.CompletedTask;
        }
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("corsapp");

app.UseAuthorization();

app.MapControllers();

app.Run();

