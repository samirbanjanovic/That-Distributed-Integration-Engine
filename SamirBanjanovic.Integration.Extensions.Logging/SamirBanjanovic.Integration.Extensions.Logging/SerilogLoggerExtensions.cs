using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace OnTrac.Integration.Extensions.Logging
{
    public static class SerilogLoggerExtensions
    {

        public static LoggerConfiguration Configure(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            loggerConfiguration
                .WithOnTracDbSink(configuration.GetSection("Serilog").GetSection("MsSqlLog"))
                .ReadFrom
                .Configuration(configuration);
                    


            loggerConfiguration = EnvironmentLoggerConfigurationExtensions.WithMachineName(loggerConfiguration.Enrich);

            return loggerConfiguration;
        }

        private static LoggerConfiguration WithOnTracDbSink(this LoggerConfiguration loggerConfiguration, IConfigurationSection configuration)
        {
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            
            columnOptions.Store.Add(StandardColumn.LogEvent);

            columnOptions.AdditionalDataColumns = new Collection<DataColumn>
            {
                new DataColumn{DataType = typeof(string), ColumnName = "ProcessName"},
                new DataColumn{DataType = typeof(string), ColumnName = "SourceContext"},
                new DataColumn{DataType = typeof(string), ColumnName = "Name"},
                new DataColumn{DataType = typeof(string), ColumnName = "MachineName"},
                new DataColumn{DataType = typeof(Guid), ColumnName = "Correlation"},
                new DataColumn{DataType = typeof(string), ColumnName = "AssemblyVersion"},
                new DataColumn{DataType = typeof(string), ColumnName = "ObjectProperties"},
                new DataColumn{DataType = typeof(long), ColumnName = "ThreadId"},
                new DataColumn{DataType = typeof(string), ColumnName = "ComponentMessage"},
            };

            loggerConfiguration
                .WriteTo
                .MSSqlServer(configuration["connectionString"],
                             configuration["tableName"],
                             autoCreateSqlTable: bool.Parse(configuration["autoCreateTable"]),
                             columnOptions: columnOptions);

            return loggerConfiguration;
        }

    }
}
