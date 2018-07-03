namespace Api.Utils
{
    public class EventosLog
    {
        // Usuarios
        public const int Usuarios = 1000;
        public const int UsuariosCadastrarCliente = 1001;
        public const int UsuariosLogar = 1002;
        public const int UsuariosRecuperarSenhaPorCpf = 1003;
        public const int UsuariosRecuperarSenhaPorEmail = 1004;
        public const int UsuariosAlterarSenha = 1005;
        public const int UsuariosSair = 1006;

        // Produtos
        public const int Produtos = 2000;
        public const int ProdutosBuscarPorId = 2001;
        public const int ProdutosConsultarCardapio = 2002;
        public const int ProdutosConsultarRanking = 2003;

        // Categorias
        public const int Categorias = 3000;
        public const int CategoriasListar = 3001;

        // Reservas
        public const int Reservas = 4000;
        public const int ReservasConsultarHistorico = 4001;
        public const int ReservasConsultarTempo = 4002;
        public const int ReservasSolicitarMesa = 4003;
        public const int ReservasAtivarReserva = 4004;
        public const int ReservasCancelarMesa = 4005;

        // Contas
        public const int Contas = 5000;
        public const int ContasConsultarConta = 5001;
        public const int ContasRealizarPedido = 5002;
        public const int ContasEncerrarPedido = 5003;
    }
}