using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ContasController : Controller
    {
        private readonly IContaDac _contaDac;
        private readonly ILogger _logger;

        public ContasController(IContaDac contaDac, ILogger<ContasController> logger)
        {
            _contaDac = contaDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("ConsultarAtual/{usuarioId}")]
        public IActionResult ConsultarAtual(int usuarioId)
        {
            try
            {
                Historico historicoDeHoje = _contaDac.ConsultarAtual(usuarioId);
                if (historicoDeHoje == null)
                    return NotFound("Não há uma conta em aberto.");

                return Ok(historicoDeHoje);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ContasConsultar, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}