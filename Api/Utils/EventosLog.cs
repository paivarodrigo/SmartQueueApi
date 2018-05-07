namespace Api.Utils
{
    public class EventosLog
    {
        // Usuarios
        public const int Usuarios = 1000;
        public const int UsuariosCriar = 1001;
        public const int UsuariosLogar = 1002;
        public const int UsuariosRecuperarSenhaPorCpf = 1003;
        public const int UsuariosRecuperarSenhaPorEmail = 1004;
        public const int UsuariosAlterarSenha = 1005;
        public const int UsuariosSair = 1006;

        // Produtos
        public const int Produtos = 2000;
        public const int ProdutosBuscarPorId = 2001;
        public const int ProdutosListar = 2002;
        public const int ProdutosRanking = 2003;

        // Categorias
        public const int Categorias = 3000;
        public const int CategoriasListar = 3001;

        // Reservas
        public const int Reservas = 4000;
        public const int ReservasHistorico = 4001;

        // Contas
        public const int Contas = 5000;
        public const int ContasConsultar = 5001;
    }
}