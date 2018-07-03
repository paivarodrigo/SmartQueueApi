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

        Conta AtivarReserva(Reserva reserva, int numeroDaMesa);

        int BuscarReservaIDPorSenha(int usuarioId, string senhaCheckIn);

        bool CancelarReserva(int reservaID);
    }
}
