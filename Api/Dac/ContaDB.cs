
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

        public Historico ConsultarAtual(int usuarioId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("UsuarioID", usuarioId);
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Historico>("Contas.ConsultarAtual", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
