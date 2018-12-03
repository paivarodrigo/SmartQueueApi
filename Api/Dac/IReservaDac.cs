using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IReservaDac
    {
        List<Reserva> BuscarReservasNaFila();

        Reserva BuscarUltimaFinalizadaDoUsuario(int usuarioId);

        Conta BuscarConta(int reservaId);

        IEnumerable<Historico> ConsultarHistorico(int usuarioId);

        Reserva AdicionarReserva(Reserva reserva);

        Conta AtivarReserva(Reserva reserva, string senhaDaMesa);

        int BuscarReservaIDPorStatus(int usuarioId, string reservaStatus);

        bool CancelarReserva(int reservaID);

        bool VerificarReservaUsuario(int usuarioId);

        bool VerificarLotacaoMesas();

        void AtualizarTempos(List<Reserva> reservas);

        int ConsultarTempo(int reservaId);
    }
}
