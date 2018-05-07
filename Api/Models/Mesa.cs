namespace Api.Models
{
    public class Mesa
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public int Numero { get; set; }

        public int QuantidadeAssentos { get; set; }

        public bool Disponivel { get; set; }
    }
}
