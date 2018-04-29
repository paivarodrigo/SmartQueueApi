using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface ICategoriaDac
    {
        IEnumerable<Categoria> Listar();
    }
}
