using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Api.Dac
{
    public class UsuarioDB : IUsuarioDac
    {
        private readonly IConfiguration Configuration;

        public UsuarioDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public int CadastrarCliente(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.ExecuteScalar<int>(@"
                    INSERT INTO dbo.Usuarios (Nome, Sobrenome, DataNascimento, CPF, Email, CidadeNatal, Senha)
	                VALUES (@Nome, @Sobrenome, @DataNascimento, @Cpf, @Email, @CidadeNatal, @Senha);

                    SELECT @@IDENTITY;",
                    new
                    {
                        Nome = usuario.Nome,
                        Sobrenome = usuario.Sobrenome,
                        DataNascimento = usuario.DataNascimento,
                        Cpf = usuario.Cpf,
                        Email = usuario.Email,
                        CidadeNatal = usuario.CidadeNatal,
                        Senha = usuario.Senha
                    });
            }
        }

        public void AlterarSenha(int id, string senha)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                con.ExecuteScalar(@"UPDATE Usuarios SET Senha = @Senha WHERE ID = @ID;",
                    new
                    {
                        Senha = senha,
                        ID = id
                    });
            }
        }

        public Usuario BuscarPorId(int id)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Usuario>(@"
                    SELECT ID,
		                   Nome,
		                   Sobrenome,
		                   DataNascimento,
		                   CPF,
		                   Email,
		                   CidadeNatal,
		                   Senha
	                  FROM Usuarios WHERE ID = @ID;",
                      new
                      {
                          ID = id
                      });
            }
        }

        public Usuario BuscarPorEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Usuario>(@"
                    SELECT ID,
		                   Nome,
		                   Sobrenome,
		                   DataNascimento,
		                   CPF,
		                   Email,
		                   CidadeNatal,
		                   Senha
	                  FROM Usuarios WHERE Email = @Email;",
                      new
                      {
                          Email = email
                      });
            }
        }

        public Usuario BuscarPorCPF(string cpf)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Usuario>(@"
                    SELECT ID,
		                   Nome,
		                   Sobrenome,
		                   DataNascimento,
		                   CPF,
		                   Email,
		                   CidadeNatal,
		                   Senha
	                  FROM Usuarios WHERE CPF = @CPF;",
                      new
                      {
                          CPF = cpf
                      });
            }
        }
    }
}
