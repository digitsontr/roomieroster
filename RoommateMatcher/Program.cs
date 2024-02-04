using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Configuration;
using RoommateMatcher.Hubs;
using RoommateMatcher.Localization;
using RoommateMatcher.Loggers;
using RoommateMatcher.Models;
using RoommateMatcher.Services;
using RoommateMatcher.Validations;
using Hangfire;
using RoommateMatcher.Tasks;
using HangfireBasicAuthenticationFilter;
using RoommateMatcher.Middlewares;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder
    .Configuration.GetConnectionString("SqlConnection")));


builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
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
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<RoommateMatcher.Models.UserOptions>(builder.Configuration.GetSection("UserOperations"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("localhost")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials()
       .SetIsOriginAllowed((host) => true);
}));
builder.Services.AddSignalR();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(3);
});
builder.Services.AddAutoMapper(typeof(Program));

var env = builder.Services.BuildServiceProvider().GetService<IWebHostEnvironment>();
string logDirectory = Path.Combine(env.ContentRootPath, "logs");
HubLogger logger = new HubLogger(logDirectory);
builder.Services.AddSingleton(logger);

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
        },
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chat")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "My Website",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter{
            User = builder.Configuration.GetSection("HangfireSettings:UserName").Value,
            Pass = builder.Configuration.GetSection("HangfireSettings:Password").Value
                }
            }
});


app.UseHangfireServer();

RecurringJob.AddOrUpdate<CheckUnreadMessagesTask>("CheckUnreadMessages",
    x => x.CheckUnreadMessages(), Cron.Hourly);
RecurringJob.AddOrUpdate<RemoveMessagesFromDbTask>("RemoveMessagesOlderThanTenDays",
    x => x.RemoveMessagesOlderThanTenDays(), Cron.Daily);

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Src")),
    RequestPath = "/Src"
});
app.UseCustomException();

app.UseAuthentication();

app.UseCors("corsapp");

app.UseAuthorization();

app.MapHub<ChatHub>("/chat");

app.MapControllers();

app.Run();

