using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IReservaDac
    {
        Reserva BuscarUltimaFinalizadaDoUsuario(int usuarioId);

        string BuscarMesasDaReserva(int reservaId);

        Conta BuscarConta(int reservaId);

        IEnumerable<Historico> BuscarHistorico(int usuarioId);
    }
}
