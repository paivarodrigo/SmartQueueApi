using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IContaDac
    {
        Historico ConsultarPorReservaId(int reservaId);

        Pedido AdicionarPedido(int contaId, IEnumerable<ItemPedido> itensPedido);

        bool EncerrarPedido(int pedidoId);
    }
}
