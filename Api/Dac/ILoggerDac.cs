using System;

namespace Api.Dac
{
    public interface ILoggerDac
    {
        void LogDebug(int eventoId, string mensagem);

        void LogError(int eventoId, Exception exception, string message);
    }
}
