using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Api.Dac
{
    public class CategoriaDB : ICategoriaDac
    {
        private readonly IConfiguration Configuration;

        public CategoriaDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IEnumerable<Categoria> Listar()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Categoria>("Categorias.Listar", null, commandType: CommandType.StoredProcedure);
        }
    }
}
