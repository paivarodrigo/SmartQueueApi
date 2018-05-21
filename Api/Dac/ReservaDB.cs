using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Api.Dac
{
    public class ReservaDB : IReservaDac
    {
        private readonly IConfiguration Configuration;

        public ReservaDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Reserva AtivarReserva(Reserva reserva, int numeroDaMesa)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("SenhaDaReserva", reserva.SenhaCheckIn);
            parametros.Add("NumeroDaMesa", numeroDaMesa);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Reserva>("Reservas.AtivarReserva", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Conta BuscarConta(int reservaId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("ReservaID", reservaId);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Conta>("Reservas.BuscarConta", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<Historico> BuscarHistorico(int usuarioId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("UsuarioID", usuarioId);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Historico>("Reservas.BuscarHistorico", parametros, commandType: CommandType.StoredProcedure);
        }

        public string BuscarMesasDaReserva(int reservaId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("ReservaID", reservaId);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<string>("Reservas.BuscarMesasDaReserva", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Reserva BuscarUltimaFinalizadaDoUsuario(int usuarioId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("UsuarioID", usuarioId);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Reserva>("Reservas.BuscarFinalizadasDoUsuario", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public Reserva SolicitarReserva(Reserva reserva)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("UsuarioID", reserva.UsuarioId);
            parametros.Add("QuantidadePessoas", reserva.QuantidadePessoas);
            parametros.Add("SenhaDaReserva", reserva.SenhaCheckIn);
            parametros.Add("TempoDeEspera", reserva.TempoDeEspera);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Reserva>("Reservas.SolicitarReserva", parametros, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
