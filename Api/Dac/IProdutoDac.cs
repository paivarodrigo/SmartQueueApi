using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IProdutoDac
    {
        IEnumerable<Categoria> ListarCategorias();

        IEnumerable<Produto> ListarProdutos();

        Produto BuscarPorId(int id);

        IEnumerable<Produto> ConsultarRanking();

        string BuscarPorContaId(int contaId);
    }
}
