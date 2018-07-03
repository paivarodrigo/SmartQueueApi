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
        [Route("ConsultarCardapio")]
        public IActionResult ConsultarCardapio()
        {
            try
            {
                // Busca lista de categorias
                IEnumerable<Categoria> categorias = _produtoDac.ListarCategorias();
                if (categorias == null)
                    return NotFound("Não há categorias para listar.");

                // Busca lista de produtos
                IEnumerable<Produto> produtos = _produtoDac.ListarProdutos();
                if (produtos == null)
                    return NotFound("Não há produtos para listar.");

                return Ok(new { Categorias = categorias, Produtos = produtos });
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosConsultarCardapio, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpGet]
        [Route("ConsultarRanking")]
        public IActionResult ConsultarRanking()
        {
            try
            {
                IEnumerable<Produto> produtos = _produtoDac.ConsultarRanking();
                if (produtos == null)
                    return NotFound("Não há dados para gerar o ranking.");

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosConsultarRanking, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}