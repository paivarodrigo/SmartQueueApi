
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Api.Dac
{
    public class ContaDB : IContaDac
    {
        private readonly IConfiguration Configuration;

        public ContaDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Historico ConsultarPorReservaId(int reservaId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("ReservaID", reservaId);
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Historico>("Contas.ConsultarPorReservaID", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
