using ETicaretAPI.API.Extensions;
using ETicaretAPI.API.mssqlLogs;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infastructure;
using ETicaretAPI.Infastructure.Filters;
using ETicaretAPI.Infastructure.Services.Storage.Azure;
using ETicaretAPI.Infastructure.Services.Storage.Local;
using ETicaretAPI.Persistance;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Text;
using ETicaretAPI.SignelR;
using ETicaretAPI.SignelR.Hubs;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

builder.Services.AddPersistanceServices();

builder.Services.AddInfrastructureServices();


builder.Services.AddApplicationServices();
builder.Services.AddSignelRServices();

//builder.Services.AddStorage(StorageType.Local);
//builder.Services.AddStorage<LocalStorage>();
builder.Services.AddStorage<AzureStorage>();
builder.Services.AddCors(options=> options.AddDefaultPolicy(policy=>
  policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));

/////////////////////////////////////////////////////////////////////////////////////////////////////////

//Logger log = new LoggerConfiguration()
//   .WriteTo.Console()
//   .WriteTo.File("logs/log.txt")
//   .WriteTo.MSSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ETicaretAPIDb2;Trusted_Connection=True;",
//    sinkOptions: new MSSqlServerSinkOptions
//    {
//       AutoCreateSqlTable = true,
//        TableName = "logs",
//        ColumnOptions: new ColumnOptions
//        {

//        }


//    }, null, null, LogEventLevel.Warning, null, null, null, null)
//   .CreateLogger();

//builder.Host.UseSerilog(log);


SqlColumn sqlColumn = new SqlColumn();
sqlColumn.ColumnName = "UserName";
sqlColumn.DataType = System.Data.SqlDbType.NVarChar;
sqlColumn.PropertyName = "UserName";
sqlColumn.DataLength = 50;
sqlColumn.AllowNull = true;
ColumnOptions columnOpt = new ColumnOptions();
columnOpt.Store.Remove(StandardColumn.Properties);
columnOpt.Store.Add(StandardColumn.LogEvent);
columnOpt.AdditionalColumns = new Collection<SqlColumn> { sqlColumn };

Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.MSSqlServer(
    "Server=(localdb)\\MSSQLLocalDB;Database=ETicaretAPIDb2;Trusted_Connection=True;",
     sinkOptions: new MSSqlServerSinkOptions
{
AutoCreateSqlTable = true,
TableName = "logs2",
},
     appConfiguration: null,
     columnOptions: columnOpt
    )
    .Enrich.FromLogContext()
    .Enrich.With<CustomUserNameColumn>()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog(log);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

///////////////////////////////////////////////////////////



//////////////////////////////////////////////////////////////////////////




builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilter>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("admin",options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidAudience = "www.bilmemne.com",
            ValidIssuer = "www.myapi.com",

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sebebsiz bos yere ayrilacaksan...")),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            NameClaimType = ClaimTypes.Name

        };
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseHttpLogging();
app.UseCors();
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("UserName", username);
    await next();
});
app.MapControllers();
app.MapHubs();

app.Run();
