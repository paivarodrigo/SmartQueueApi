using System;
using System.Collections.Generic;
using Api.Dac;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ReservasController : Controller
    {
        private readonly IReservaDac _reservaDac;
        private readonly IProdutoDac _produtoDac;
        private readonly ILogger _logger;

        public ReservasController(IReservaDac reservaDac, IProdutoDac produtoDac, ILogger<ReservasController> logger)
        {
            _reservaDac = reservaDac;
            _produtoDac = produtoDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("Historico/{usuarioId}")]
        public IActionResult Historico(int usuarioId)
        {
            try
            {
                IEnumerable<Historico> historicos = _reservaDac.BuscarHistorico(usuarioId);
                if (historicos == null)
                    return NotFound("Não há dados para gerar histórico.");

                return Ok(historicos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ReservasHistorico, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}