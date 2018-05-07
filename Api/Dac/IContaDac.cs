using Api.Models;

namespace Api.Dac
{
    public interface IContaDac
    {
        Historico ConsultarAtual(int usuarioId);
    }
}
