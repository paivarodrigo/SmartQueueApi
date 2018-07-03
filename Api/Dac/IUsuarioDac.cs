using Api.Models;

namespace Api.Dac
{
    public interface IUsuarioDac
    {
        void CadastrarCliente(Usuario usuario);

        void AlterarSenha(int id, string senha);

        Usuario BuscarPorId(int id);

        Usuario BuscarPorEmail(string email);

        Usuario BuscarPorCPF(string cpf);
    }
}
