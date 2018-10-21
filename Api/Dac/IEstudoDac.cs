using Api.Models;
using System.Collections.Generic;

namespace Api.Dac
{
    public interface IEstudoDac
    {
        List<DadosTreinoIA> BuscarDadosTreinoIA();

        List<DadosTreinoIA> BuscarDadosTesteIA();

        List<DadosExecucao> BuscarDadosExecucao();
    }
}
