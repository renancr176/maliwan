using Maliwan.Service.Api.Extensions;
using Maliwan.Service.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSerilog();
builder.UseStartup<Startup>();