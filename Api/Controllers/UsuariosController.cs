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

        public UsuariosController(IUsuarioDac usuarioDac)
        {
            _usuarioDac = usuarioDac;
        }

        [HttpPost]
        [Route("Criar")]
        public IActionResult Criar([FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest();

                Usuario _usuario;
                _usuario = _usuarioDac.BuscarPorCPF(usuario.Cpf);
                if (_usuario != null)
                    return BadRequest("Este CPF já é cadastrado.");

                _usuario = _usuarioDac.BuscarPorEmail(usuario.Email);
                if (_usuario != null)
                    return BadRequest("Este e-mail já é cadastrado.");

                // Criptografa a senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                _usuarioDac.Criar(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                //Logar exception
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
                //Logar exception
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

                usuario.Senha = Gerador.NovaSenha();
                Email.EnviarNovaSenha(usuario.Email, usuario.Nome + " " + usuario.Sobrenome, usuario.Senha);

                // Criptografa a nova senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                _usuarioDac.AlterarSenha(usuario.Id, usuario.Senha);
                return Ok();
            }
            catch (Exception ex)
            {
                //Logar exception
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

                usuario.Senha = Gerador.NovaSenha();
                Email.EnviarNovaSenha(usuario.Email, usuario.Nome + " " + usuario.Sobrenome, usuario.Senha);

                // Criptografa a nova senha
                usuario.Senha = Gerador.HashMd5(usuario.Senha);

                _usuarioDac.AlterarSenha(usuario.Id, usuario.Senha);
                return Ok();
            }
            catch (Exception ex)
            {
                //Logar exception
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost()]
        [Route("AlterarSenha")]
        public IActionResult AlterarSenha([FromBody] UsuarioNovaSenha model)
        {
            try
            {
                if (model == null || model.UsuarioAtual == null || model.NovaSenha == null)
                    return BadRequest(new { Mensagem = "Entrada inválida." });

                Usuario _usuario = _usuarioDac.BuscarPorId(model.UsuarioAtual.Id);
                if (_usuario == null)
                    return NotFound(new { Mensagem = "O usuário não foi encontrado." });

                // Validar senha atual criptografada
                if (Gerador.HashMd5(model.UsuarioAtual.Senha) != _usuario.Senha)
                    return BadRequest(new { Mensagem = "Falha de autenticação." });

                // Criptografa a nova senha
                model.NovaSenha = Gerador.HashMd5(model.NovaSenha);

                _usuarioDac.AlterarSenha(model.UsuarioAtual.Id, model.NovaSenha);
                return Ok();
            }
            catch (Exception ex)
            {
                //Logar exception
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }

        [HttpPost]
        [Route("Sair")]
        public IActionResult Sair([FromBody] Usuario usuario)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                //Logar exception
                return StatusCode(500, "Erro desconhecido. Por favor, contate o suporte.");
            }
        }
    }
}