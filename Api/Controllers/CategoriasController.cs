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
    public class CategoriasController : Controller
    {
        private readonly ICategoriaDac _categoriaDac;

        public CategoriasController(ICategoriaDac categoriaDac)
        {
            _categoriaDac = categoriaDac;
        }

        [HttpGet]
        [Route("Listar")]
        public IEnumerable<Categoria> Listar()
        {
            try
            {
                return _categoriaDac.Listar();
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