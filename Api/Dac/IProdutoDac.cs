using Api.Models;
using System;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IProdutoDac
    {
        IEnumerable<Categoria> ListarCategorias();

        IEnumerable<Produto> ListarProdutos();

        Produto BuscarPorId(int produtoId);

        IEnumerable<Produto> ConsultarRanking();

        string BuscarPorContaId(int contaId);

        IEnumerable<Tuple<int, string>> ListarPedidosPendentes();
    }
}
