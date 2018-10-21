using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Api.Dac
{
    public class EstudoDB : IEstudoDac
    {
        private readonly IConfiguration Configuration;

        public EstudoDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public List<DadosTreinoIA> BuscarDadosTreinoIA()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.Query<DadosTreinoIA>(@"
                    SELECT TOP 45000
                        hora_solicitacao AS HoraSolicitacao,
                        hora_checkin AS HoraCheckin,
                        qtd_pessoas AS QtdPessoas,
                        qtd_ent AS QtdEnt,
                        qtd_principal AS QtdPrincipal,
                        qtd_bebidas AS QtdBebidas,
                        qtd_sobremesa AS QtdSobremesas,
                        fds AS Fds,
                        noite AS Noite,
                        minutos_utilizados AS MinutosUtilizados
                    FROM BaseTreinoIA
                    ORDER BY 1 ASC;").AsList();
            }
        }

        public List<DadosTreinoIA> BuscarDadosTesteIA()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.Query<DadosTreinoIA>(@"
                    SELECT
                        hora_solicitacao AS HoraSolicitacao,
                        hora_checkin AS HoraCheckin,
                        qtd_pessoas AS QtdPessoas,
                        qtd_ent AS QtdEnt,
                        qtd_principal AS QtdPrincipal,
                        qtd_bebidas AS QtdBebidas,
                        qtd_sobremesa AS QtdSobremesas,
                        fds AS Fds,
                        noite AS Noite,
                        minutos_utilizados AS MinutosUtilizados
                    FROM BaseTreinoIA
                    ORDER BY 1 ASC
                    OFFSET 45000 ROWS
                    FETCH NEXT 5000 ROWS ONLY;").AsList();
            }
        }

        public List<DadosExecucao> BuscarDadosExecucao()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.Query<DadosExecucao>("Reservas.ColetarDadosParaMLP", commandType: CommandType.StoredProcedure).AsList();
            }
        }
    }
}
