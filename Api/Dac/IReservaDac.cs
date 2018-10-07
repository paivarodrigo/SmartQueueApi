using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IReservaDac
    {
        Reserva BuscarUltimaFinalizadaDoUsuario(int usuarioId);

        Conta BuscarConta(int reservaId);

        IEnumerable<Historico> ConsultarHistorico(int usuarioId);

        Reserva SolicitarReserva(Reserva reserva);

        Conta AtivarReserva(Reserva reserva, string senhaDaMesa);

        int BuscarReservaIDPorStatus(int usuarioId, string reservaStatus);

        bool CancelarReserva(int reservaID);
    }
}
