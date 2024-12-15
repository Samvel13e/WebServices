using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace LoggingService
{
    public static class SerilogLogger
    {
        public static void AddSerilogLogging(this IServiceCollection services, string connectionString)
        {

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "ServiceName", DataType = System.Data.SqlDbType.NVarChar, DataLength = 50 }
            }
            };

            Log.Logger = new LoggerConfiguration()
              .WriteTo.MSSqlServer(
                  connectionString: connectionString,
                  tableName: "Logs",
                  autoCreateSqlTable: true,
                  columnOptions: columnOptions,
                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
              )
              .Enrich.WithProperty("ServiceName", AppDomain.CurrentDomain.FriendlyName)
              .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });
        }
    }
}
