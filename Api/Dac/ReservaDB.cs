using Api.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Api.Dac
{
    public class ReservaDB : IReservaDac
    {
        private readonly IConfiguration Configuration;

        public ReservaDB(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Conta AtivarReserva(Reserva reserva, string senhaDaMesa)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                con.Execute("Reservas.AtivarReserva",
                    new
                    {
                        ReservaID = reserva.Id,
                        NumeroDaMesa = reserva.MesaId,
                        SenhaDaMesa = senhaDaMesa
                    }, commandType: CommandType.StoredProcedure);

                return con.QueryFirstOrDefault<Conta>(@"
        		SELECT ID
		    	, ReservaID
			    , DataAbertura
			    , DataFechamento
		        FROM Contas
		        WHERE ReservaID = @ReservaID;",
                new { ReservaID = reserva.Id });
            }
        }

        public Conta BuscarConta(int reservaId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
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
		                      C.DataFechamento;",
                              new
                              {
                                  ReservaID = reservaId
                              });
            }
        }

        public IEnumerable<Historico> ConsultarHistorico(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.Query<Historico>("Reservas.BuscarHistorico",
                    new
                    {
                        UsuarioID = usuarioId
                    }, commandType: CommandType.StoredProcedure);
            }
        }

        public int BuscarReservaIDPorStatus(int usuarioId, string reservaStatus)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<int>(@"
                    SELECT TOP 1 R.ID FROM Reservas R
                    INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID
                    WHERE R.UsuarioID = @UsuarioID
                    AND RS.Nome = @Status
                    ORDER BY R.ID DESC;",
                    new
                    {
                        UsuarioID = usuarioId,
                        Status = reservaStatus
                    });
            }
        }

        public Reserva BuscarUltimaFinalizadaDoUsuario(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Reserva>(@"
                    SELECT R.ID,
		                   R.UsuarioID,
		                   R.MesaID,
		                   R.DataReserva,
		                   R.QuantidadePessoas,
		                   R.MinutosDeEspera,
		                   RS.Nome AS Status
	                  FROM Reservas R
	                 INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID
	                 WHERE R.UsuarioID = @UsuarioID
	                   AND RS.Nome = 'Finalizada'
	                 ORDER BY ID DESC;",
                     new
                     {
                         UsuarioID = usuarioId
                     });
            }
        }

        public void CancelarReserva(int reservaID)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                con.Execute(@"
	            UPDATE Reservas
	               SET StatusID = (SELECT ID FROM ReservasStatus WHERE Nome = 'Cancelada')
	             WHERE ID = @ReservaID;

                IF (SELECT DataDeLiberacao FROM ReservasFila WHERE ReservaID = @ReservaID) IS NOT NULL
                BEGIN
                    -- RETIRA RESERVA DA FILA
                    DELETE FROM ReservasFila WHERE ReservaID = @ReservaID;
                    -- LIBERA PRÓXIMA RESERVA NA FILA
                    EXEC Reservas.LiberarProximaReserva;
                END
                ELSE
                BEGIN
                    -- RETIRA RESERVA DA FILA
                    DELETE FROM ReservasFila WHERE ReservaID = @ReservaID;
                END;",
                new
                {
                    ReservaID = reservaID
                });
            }
        }

        public bool VerificarReservaUsuario(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirst<int>(@"
                    IF EXISTS(SELECT TOP 1 R.ID FROM Reservas R INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID WHERE UsuarioID = @UsuarioID AND RS.Nome IN ('Em Fila', 'Em Uso'))
                    BEGIN
                        SELECT 1;
                    END
                    ELSE
                    BEGIN
                        SELECT 0;
                    END",
                    new { UsuarioID = usuarioId }) == 1;
            }
        }

        public Reserva AdicionarReserva(Reserva reserva)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<Reserva>(@"                    
	                DECLARE @StatusID INT = (SELECT ID FROM ReservasStatus WHERE Nome = 'Em Fila');

	                INSERT INTO Reservas (UsuarioID, MesaID, DataReserva, QuantidadePessoas, MinutosDeEspera, StatusID)
	                VALUES (@UsuarioID, NULL, GETDATE() - '02:00:00', @QuantidadePessoas, 0, @StatusID);

                    DECLARE @ReservaID INT = @@IDENTITY;

                    DECLARE @Posicao INT = (SELECT COUNT(1) + 1 FROM ReservasFila);

                    INSERT INTO ReservasFila (ReservaID, Posicao, MinutosDeEspera, DataDeLiberacao)
                    VALUES (@ReservaID, @Posicao, @MinutosDeEspera, NULL);
                    
                    -- LIBERA PRÓXIMA RESERVA NA FILA
                    EXEC Reservas.LiberarProximaReserva;                    

	                SELECT R.ID,
	                	   R.UsuarioID,
	                	   R.MesaID,
	                	   R.DataReserva,
	                	   R.QuantidadePessoas,
	                	   RF.MinutosDeEspera,
	                	   RS.Nome AS Status
	                FROM Reservas R
	                INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID
                    INNER JOIN ReservasFila RF ON RF.ReservaID = R.ID
	                WHERE R.ID = @ReservaID;",
                    new
                    {
                        UsuarioID = reserva.UsuarioId,
                        QuantidadePessoas = reserva.QuantidadePessoas,
                        MinutosDeEspera = reserva.MinutosDeEspera
                    });
            }
        }

        public bool VerificarLotacaoMesas()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirst<int>(@"
                    DECLARE @ReservasAtivasELiberadas INT =   
                        (SELECT COUNT(1) FROM Reservas WHERE StatusID = 
                            (SELECT TOP 1 ID FROM ReservasStatus WHERE Nome = 'Em Uso')) +   
                        (SELECT COUNT(1) FROM ReservasFila WHERE DataDeLiberacao IS NOT NULL);  
                     
                    DECLARE @MesasTotal INT = (SELECT COUNT(1) FROM Mesas);  

                    IF (@ReservasAtivasELiberadas < @MesasTotal)
                    BEGIN
                        SELECT 0;
                    END
                    ELSE
                    BEGIN
                        SELECT 1;
                    END") == 1;
            }
        }

        public List<Reserva> BuscarReservasNaFila()
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.Query<Reserva>(@"
                    SELECT R.ID,
	                	R.UsuarioID,
	                	R.MesaID,
	                	R.DataReserva,
	                	R.QuantidadePessoas,
	                	RF.MinutosDeEspera,
	                	RS.Nome AS Status
	                FROM Reservas R
	                INNER JOIN ReservasStatus RS ON RS.ID = R.StatusID
                    INNER JOIN ReservasFila RF ON RF.ReservaID = R.ID
	                WHERE RS.Nome = 'Em Fila'
                    AND RF.DataDeLiberacao IS NULL
                    ORDER BY R.ID;").AsList();
            }
        }

        public void AtualizarTempos(List<Reserva> reservas)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                con.Execute(@"
                UPDATE ReservasFila 
                SET MinutosDeEspera = @Minutos 
                WHERE ReservaID = @ReservaID;",
                reservas.Select(x => new
                {
                    ReservaID = x.Id,
                    Minutos = x.MinutosDeEspera
                }));
            }
        }

        public int ConsultarTempo(int reservaId)
        {
            using (SqlConnection con = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                return con.QueryFirstOrDefault<int>(@"
                    SELECT
                    CASE
                        WHEN DataDeLiberacao IS NULL
                            THEN MinutosDeEspera
                        ELSE 0
                    END
                    FROM ReservasFila
                    WHERE ReservaID = @ReservaID",
                    new { ReservaID = reservaId });
            }
        }
    }
}
