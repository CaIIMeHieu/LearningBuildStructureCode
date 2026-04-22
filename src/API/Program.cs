using Application.DependencyInjection.Extentions;
using Application.Middlewares;
using Domain.Entities;
using Infrastructure.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity;
using Persistance.DBContext;
using Persistance.DependencyInjection.Extentions;
using Persistance.DependencyInjection.Options;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddApplicationPart(Presentation.AssemblyReference.Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration().ReadFrom
                 .Configuration(builder.Configuration)
                 .CreateLogger();

builder.Logging.ClearProviders().AddSerilog();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddApplicationConfiguration();
builder.Services.AddSqlServerConfiguration();
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddConfigurationAutoMapper();
builder.Services.AddRepositoryBaseConfiguration();
builder.Services.AddRedisConfiguration(builder.Configuration);
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddMemoryCache();

builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
         
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
    });
});

var app = builder.Build();


using ( var scope = app.Services.CreateScope() )
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    foreach( var role in new[] {"Admin","User"} )
    {
        if( !await roleManager.RoleExistsAsync(role) )
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }    
}

// trường hợp không cấu hình handle global exception middleware thì app sẽ trả về 500
// kèm rất nhiều thông tin chi tiết về lỗi ( stack trace )

app.UseMiddleware<ExceptionHandlingMiddleware>();
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
