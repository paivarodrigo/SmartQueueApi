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
    public class CategoriasController : Controller
    {
        private readonly ICategoriaDac _categoriaDac;
        private readonly ILogger _logger;

        public CategoriasController(ICategoriaDac categoriaDac, ILogger<CategoriasController> logger)
        {
            _categoriaDac = categoriaDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("Listar")]
        public IActionResult Listar()
        {
            try
            {
                IEnumerable<Categoria> categorias = _categoriaDac.Listar();
                if (categorias == null)
                    return NotFound("Não há categorias para listar.");

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.CategoriasListar, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}