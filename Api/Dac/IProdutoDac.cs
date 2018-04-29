using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IProdutoDac
    {
        IEnumerable<Produto> Listar();

        Produto BuscarPorId(int id);
    }
}
