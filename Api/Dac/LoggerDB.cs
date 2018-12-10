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

        public void LogDebug(int eventoId, string mensagem)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                con.Execute(@"
                    INSERT INTO Logs (Nivel, Evento, Excecao, Mensagem, Data)
                    VALUES (@Nivel, @Evento, @Excecao, @Mensagem, GETDATE() - '02:00:00')",
                    new
                    {
                        Nivel = "Debug",
                        Evento = eventoId,
                        Excecao = "",
                        Mensagem = mensagem
                    });
            }
        }

        public void LogError(int eventoId, Exception excecao, string mensagem)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                con.Execute(@"
                    INSERT INTO Logs (Nivel, Evento, Excecao, Mensagem, Data)
                    VALUES (@Nivel, @Evento, @Excecao, @Mensagem, GETDATE() - '02:00:00')",
                    new
                    {
                        Nivel = "Erro",
                        Evento = eventoId,
                        Excecao = excecao.ToString(),
                        Mensagem = mensagem
                    });
            }
        }
    }
}
