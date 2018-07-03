﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public Conta AtivarReserva(Reserva reserva, int numeroDaMesa)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("SenhaDaReserva", reserva.SenhaCheckIn);
            parametros.Add("NumeroDaMesa", numeroDaMesa);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<Conta>("Reservas.AtivarReserva", parametros, commandType: CommandType.StoredProcedure);
        }

        public Conta BuscarConta(int reservaId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<Conta>(@"
                    SELECT C.ID,
		                   C.ReservaID,
		                   C.DataAbertura,
		                   C.DataFechamento,
		                   SUM(PR.Valor * IPE.Quantidade) AS Valor
	                  FROM CONTAS C
	                 INNER JOIN Pedidos PE ON PE.ContaID = C.ID
	                 INNER JOIN ItensPedidos IPE ON IPE.PedidoID = PE.ID
	                 INNER JOIN Produtos PR ON PR.ID = IPE.ProdutoID
	                 INNER JOIN PedidosStatus PS ON PS.ID = PE.StatusID
	                 WHERE ReservaID = @ReservaID
	                   AND PS.Nome IN('Em Fila', 'Processando', 'Finalizado')
	                 GROUP BY C.ID,
		                      C.ReservaID,
		                      C.DataAbertura,
		                      C.DataFechamento;", new { ReservaID = reservaId });
        }

        public IEnumerable<Historico> ConsultarHistorico(int usuarioId)
        {
            DynamicParameters parametros = new DynamicParameters();
            parametros.Add("UsuarioID", usuarioId);

            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.Query<Historico>("Reservas.BuscarHistorico", parametros, commandType: CommandType.StoredProcedure);
        }

        public int BuscarReservaIDPorSenha(int usuarioId, string senhaCheckIn)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<int>(@"
                    SELECT TOP 1 ID
	                  FROM Reservas
	                 WHERE UsuarioID = @UsuarioID
	                   AND SenhaCheckin = @SenhaCheckin
	                 ORDER BY ID DESC;", new { UsuarioID = usuarioId, SenhaCheckin = senhaCheckIn });
        }

        public Reserva BuscarUltimaFinalizadaDoUsuario(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<Reserva>(@"
                    SELECT R.ID,
		                   R.UsuarioID,
		                   R.MesaID,
		                   R.DataReserva,
		                   R.QuantidadePessoas,
		                   R.SenhaCheckin,
		                   R.DataCheckIn,
		                   R.DataCheckOut,
		                   R.TempoDeEspera,
		                   RS.Nome AS Status
	                  FROM Reservas R
	                 INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID
	                 WHERE R.UsuarioID = @UsuarioID
	                   AND RS.Nome = 'Finalizada'
	                 ORDER BY ID DESC;", new { UsuarioID = usuarioId });
        }

        public bool CancelarReserva(int reservaID)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.ExecuteScalar<int>(@"
                    IF EXISTS(SELECT * FROM Reservas R INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID WHERE R.ID = @ReservaID AND RS.Nome = 'Em Fila')
	                BEGIN
	                	UPDATE Reservas
	                	   SET StatusID = (SELECT ID FROM ReservasStatus WHERE Nome = 'Cancelada')
	                	 WHERE ID = @ReservaID;

	                	SELECT 1;
	                END
	                ELSE
	                BEGIN
	                	SELECT 0;
	                END", new { ReservaID = reservaID }) == 1;
        }

        public Reserva SolicitarReserva(Reserva reserva)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                return con.QueryFirstOrDefault<Reserva>(@"
                    IF NOT EXISTS(SELECT TOP 1 R.ID FROM Reservas R INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID WHERE UsuarioID = @UsuarioID AND RS.Nome IN ('Em Fila', 'Em Uso'))
	                BEGIN
	                	DECLARE @StatusID INT = (SELECT ID FROM ReservasStatus WHERE Nome = 'Em Fila');

	                	INSERT INTO Reservas (UsuarioID, MesaID, DataReserva, QuantidadePessoas, SenhaCheckIn, DataCheckIn, DataCheckOut, TempoDeEspera, StatusID)
	                	VALUES (@UsuarioID, NULL, GETDATE(), @QuantidadePessoas, @SenhaCheckIn, NULL, NULL, @TempoDeEspera, @StatusID);

	                	SELECT TOP 1 R.ID,
	                		   R.UsuarioID,
	                		   R.MesaID,
	                		   R.DataReserva,
	                		   R.QuantidadePessoas,
	                		   R.SenhaCheckIn,
	                		   R.DataCheckIn,
	                		   R.DataCheckOut,
	                		   R.TempoDeEspera,
	                		   RS.Nome AS Status
	                	  FROM Reservas R
	                	 INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID
	                	 WHERE UsuarioID = @UsuarioID
	                	   AND RS.Nome = 'Em Fila';
	                END", reserva);
        }
    }
}