using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Api.Dac
{
    public class UsuarioDB : IUsuarioDac
    {
        private readonly IConfiguration Configuration;

        public UsuarioDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Criar(Usuario usuario)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Nome", usuario.Nome);
            parameters.Add("Sobrenome", usuario.Sobrenome);
            parameters.Add("DataNascimento", usuario.DataNascimento);
            parameters.Add("CPF", usuario.Cpf);
            parameters.Add("Email", usuario.Email);
            parameters.Add("CidadeNatal", usuario.CidadeNatal);
            parameters.Add("Senha", usuario.Senha);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                con.ExecuteScalar<int>("Usuarios.Criar", parameters, commandType: CommandType.StoredProcedure);
        }

        public void AlterarSenha(int id, string senha)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ID", id);
            parameters.Add("Senha", senha);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                con.ExecuteScalar("Usuarios.AlterarSenha", parameters, commandType: CommandType.StoredProcedure);
        }

        public Usuario BuscarPorId(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ID", id);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Usuario>("Usuarios.BuscarPorId", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Usuario BuscarPorEmail(string email)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Email", email);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Usuario>("Usuarios.BuscarPorEmail", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Usuario BuscarPorCPF(string cpf)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("CPF", cpf);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Usuario>("Usuarios.BuscarPorCpf", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
