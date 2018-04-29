using System;

namespace Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public DateTime DataNascimento { get; set; }

        public string Cpf { get; set; }

        public string Email { get; set; }

        public string CidadeNatal { get; set; }

        public string Senha { get; set; }

        public Reserva Reserva { get; set; }
    }
}
