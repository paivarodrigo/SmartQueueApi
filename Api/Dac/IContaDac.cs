using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IContaDac
    {
        Historico ConsultarPorReservaId(int reservaId);

        Pedido AdicionarPedido(int contaId, IEnumerable<ItemPedido> itensPedido);

        bool CancelarPedido(int pedidoId);

        //bool ProcessarPedido(int pedidoId);

        bool FinalizarPedido(int pedidoId);

        Conta FecharConta(int reservaId);
    }
}
