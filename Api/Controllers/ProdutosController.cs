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
    public class ProdutosController : Controller
    {
        private readonly IProdutoDac _produtoDac;
        private readonly ILogger _logger;

        public ProdutosController(IProdutoDac produtoDac, ILogger<ProdutosController> logger)
        {
            _produtoDac = produtoDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("BuscarPorId/{id}")]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                Produto produto = _produtoDac.BuscarPorId(id);

                if (produto == null)
                    return NotFound("O produto não foi encontrado.");

                return Ok(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosBuscarPorId, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpGet]
        [Route("Listar")]
        public IActionResult Listar()
        {
            try
            {
                IEnumerable<Produto> produtos = _produtoDac.Listar();
                if (produtos == null)
                    return NotFound("Não há produtos para listar.");

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosListar, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpGet]
        [Route("Ranking")]
        public IActionResult Ranking()
        {
            try
            {
                IEnumerable<Produto> produtos = _produtoDac.Ranking();
                if (produtos == null)
                    return NotFound("Não há dados para gerar o ranking.");

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosRanking, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}