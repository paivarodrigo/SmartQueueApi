using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Api.Dac
{
    public class ProdutoDB : IProdutoDac
    {
        private readonly IConfiguration Configuration;

        public ProdutoDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Produto BuscarPorId(int id)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("ID", id);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Produto>("Produtos.BuscarPorId", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<Produto> Listar()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Produto>("Produtos.Listar", null, commandType: CommandType.StoredProcedure);
        }
    }
}
