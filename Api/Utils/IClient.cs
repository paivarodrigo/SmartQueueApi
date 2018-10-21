using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Utils
{
    public interface IClient
    {
        Task<List<int>> BuscarTemposPrevistos(int quantidadeDeTempos);
    }
}
