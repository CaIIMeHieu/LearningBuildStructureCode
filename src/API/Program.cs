using Application.DependencyInjection.Extentions;
using Persistance.DependencyInjection.Extentions;
using Persistance.DependencyInjection.Options;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
