using Api.Dac;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Api.Utils;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioDac _usuarioDac;
        private readonly ILoggerDac _logger;

        public UsuariosController(IUsuarioDac usuarioDac, ILoggerDac logger)
        {
            _usuarioDac = usuarioDac;
            _logger = logger;
        }

        [HttpPost]
        [Route("CadastrarCliente")]
        public IActionResult CadastrarCliente([FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest("Entradas inválidas para cadastro de usuário.");

                Usuario _usuario = _usuarioDac.BuscarPorCPF(usuario.Cpf);
                if (_usuario != null)
                    return BadRequest("Este CPF já está cadastrado.");

                _usuario = _usuarioDac.BuscarPorEmail(usuario.Email);
                if (_usuario != null)
                    return BadRequest("Este email já está cadastrado.");

                // Criptografa a senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                usuario.Id = _usuarioDac.CadastrarCliente(usuario);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.UsuariosCadastrarCliente, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("Logar")]
        public IActionResult Logar([FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null || usuario.Email == null || usuario.Senha == null)
                    return BadRequest("Email e/ou senha estão incorretos.");

                Usuario _usuario = _usuarioDac.BuscarPorEmail(usuario.Email);
                if (_usuario == null)
                    return BadRequest("Email e/ou senha estão incorretos.");

                // Criptografa a senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                if (usuario.Senha != _usuario.Senha)
                    return BadRequest("Email e/ou senha estão incorretos.");

                return Ok(_usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.UsuariosLogar, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("RecuperarSenhaPorCpf/{cpf}")]
        public IActionResult RecuperarSenhaPorCpf(string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return BadRequest("O CPF não pode ser nulo.");

                Usuario usuario = _usuarioDac.BuscarPorCPF(cpf);
                if (usuario == null)
                    return NotFound("O usuário não foi encontrado.");

                usuario.Senha = Gerador.GerarSenhaUsuario();
                Email.EnviarNovaSenha(usuario.Email, usuario.Nome + " " + usuario.Sobrenome, usuario.Senha);

                // Criptografa a nova senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                _usuarioDac.AlterarSenha(usuario.Id, usuario.Senha);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.UsuariosRecuperarSenhaPorCpf, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("RecuperarSenhaPorEmail/{email}")]
        public IActionResult RecuperarSenhaPorEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest("O email não pode ser nulo.");

                Usuario usuario = _usuarioDac.BuscarPorEmail(email);
                if (usuario == null)
                    return NotFound("O usuário não foi encontrado.");

                usuario.Senha = Gerador.GerarSenhaUsuario();
                Email.EnviarNovaSenha(usuario.Email, usuario.Nome + " " + usuario.Sobrenome, usuario.Senha);

                // Criptografa a nova senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                _usuarioDac.AlterarSenha(usuario.Id, usuario.Senha);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.UsuariosRecuperarSenhaPorEmail, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("AlterarSenha")]
        public IActionResult AlterarSenha([FromBody] UsuarioNovaSenha model)
        {
            try
            {
                if (model == null || model.UsuarioAtual == null || model.NovaSenha == null)
                    return BadRequest("Entradas inválidas.");

                Usuario _usuario = _usuarioDac.BuscarPorId(model.UsuarioAtual.Id);
                if (_usuario == null)
                    return NotFound("O usuário não foi encontrado.");

                // Validar senha atual criptografada
                if (model.UsuarioAtual.Senha != _usuario.Senha)
                    return BadRequest("Falha de autenticação.");

                // Criptografa a nova senha
                model.NovaSenha = Gerador.HashMd5(model.NovaSenha);

                _usuarioDac.AlterarSenha(model.UsuarioAtual.Id, model.NovaSenha);
                _usuario.Senha = model.NovaSenha;
                return Ok(_usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.UsuariosAlterarSenha, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("Sair")]
        public IActionResult Sair([FromBody] Usuario usuario)
        {
            try
            {
                return Ok(new Usuario());
            }
            catch (Exception ex)
            {
                _logger.LogError(EventosLog.UsuariosSair, ex, ex.Message);
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}