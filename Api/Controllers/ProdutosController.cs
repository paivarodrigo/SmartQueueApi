using System;
using System.Collections.Generic;
using System.Net;
using Api.Dac;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        private readonly IProdutoDac _produtoDac;

        public ProdutosController(IProdutoDac produtoDac)
        {
            _produtoDac = produtoDac;
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
                //Logar exception
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpGet]
        [Route("Listar")]
        public IEnumerable<Produto> Listar()
        {
            try
            {
                return _produtoDac.Listar();
            }
            catch (Exception ex)
            {
                //Logar exception
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return null;
            }
        }
    }
}