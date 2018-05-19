using Api.Models;

namespace Api.Dac
{
    public interface IContaDac
    {
        Historico ConsultarPorReservaId(int reservaId);
    }
}
