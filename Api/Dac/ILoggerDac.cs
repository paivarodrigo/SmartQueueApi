using System;

namespace Api.Dac
{
    public interface ILoggerDac
    {
        void LogError(int eventoId, Exception exception, string message);
    }
}
