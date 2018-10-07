using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace Api.Dac
{
    public class LoggerDB : ILoggerDac
    {
        private readonly IConfiguration Configuration;

        public LoggerDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void LogError(int eventoId, Exception exception, string message)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                con.Execute("INSERT INTO Logs VALUES (@Level, @Evento, @Exception, @Message, GETDATE())", new { Level = "Erro", Evento = eventoId, Exception = exception.ToString(), Message = message });
        }
    }
}
