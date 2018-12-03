using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EstudosController : Controller
    {
        private readonly ILoggerDac _loggerDac;
        private readonly IEstudoDac _estudoDac;

        public EstudosController(IEstudoDac estudoDac, ILoggerDac loggerDac)
        {
            _estudoDac = estudoDac;
            _loggerDac = loggerDac;
        }

        [HttpGet]
        [Route("BuscarDadosTreinoIA")]
        public IActionResult BuscarDadosTreinoIA()
        {
            try
            {
                IEnumerable<DadosTreinoIA> dadosTreinoIAs = _estudoDac.BuscarDadosTreinoIA();
                if (dadosTreinoIAs == null)
                {
                    return BadRequest("Não há dados para treino.");
                }

                return Ok(dadosTreinoIAs);
            }
            catch (Exception ex)
            {
                _loggerDac.LogError(EventosLog.EstudosBuscarDadosTreinoIA, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao buscar os dados de treino da IA.");
            }
        }

        [HttpGet]
        [Route("BuscarDadosTesteIA")]
        public IActionResult BuscarDadosTesteIA()
        {
            try
            {
                IEnumerable<DadosTreinoIA> dadosTesteIAs = _estudoDac.BuscarDadosTesteIA();
                if (dadosTesteIAs == null)
                {
                    return BadRequest("Não há dados para teste.");
                }

                return Ok(dadosTesteIAs);
            }
            catch (Exception ex)
            {
                _loggerDac.LogError(EventosLog.EstudosBuscarDadosTreinoIA, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao buscar os dados de teste da IA.");
            }
        }

        [HttpGet]
        [Route("BuscarDadosExecucao")]
        public IActionResult BuscarDadosExecucao()
        {
            try
            {
                List<DadosExecucao> dadosExecucao = _estudoDac.BuscarDadosExecucao();

                return Ok(dadosExecucao);
            }
            catch (Exception ex)
            {
                _loggerDac.LogError(EventosLog.EstudosBuscarDadosExecucao, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao buscar os dados de execução.");
            }
        }
    }
}