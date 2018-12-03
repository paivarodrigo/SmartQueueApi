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
    public class ProdutosController : Controller
    {
        private readonly IProdutoDac _produtoDac;
        private readonly ILoggerDac _logger;

        public ProdutosController(IProdutoDac produtoDac, ILoggerDac logger)
        {
            _produtoDac = produtoDac;
            _logger = logger;
        }

        [HttpGet]
        [Route("ConsultarProduto/{produtoId}")]
        public IActionResult ConsultarProduto(int produtoId)
        {
            try
            {
                Produto produto = _produtoDac.BuscarPorId(produtoId);

                if (produto == null)
                {
                    return NotFound("O produto não foi encontrado.");
                }

                return Ok(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosBuscarPorId, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao consultar o produto.");
            }
        }

        [HttpGet]
        [Route("ListarCategorias")]
        public IActionResult ListarCategorias()
        {
            try
            {
                // Busca lista de categorias
                IEnumerable<Categoria> categorias = _produtoDac.ListarCategorias();
                if (categorias == null)
                {
                    return NotFound("Não há categorias para listar.");
                }

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosListarCategorias, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao listar as categorias.");
            }
        }

        [HttpGet]
        [Route("ListarProdutos")]
        public IActionResult ListarProdutos()
        {
            try
            {
                // Busca lista de produtos
                IEnumerable<Produto> produtos = _produtoDac.ListarProdutos();
                if (produtos == null)
                {
                    return NotFound("Não há produtos para listar.");
                }

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosListar, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao listar os produtos.");
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
                {
                    return NotFound("Não há dados para gerar o ranking.");
                }

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosConsultarRanking, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao consultar o ranking.");
            }
        }

        [HttpGet]
        [Route("ListarPedidosPendentes")]
        public IActionResult ListarPedidosPendentes()
        {
            try
            {
                IEnumerable<Tuple<int, string>> pedidosPendentes = _produtoDac.ListarPedidosPendentes();

                return Ok(pedidosPendentes);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.ProdutosListarPedidosPendentes, ex, ex.Message);
                return StatusCode(500, "Ops! Um erro ocorreu ao listar os pedidos pendentes.");
            }
        }
    }
}