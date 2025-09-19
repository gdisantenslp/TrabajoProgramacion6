using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrabajoProyecto.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrabajoProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocioController : ControllerBase
    {
        public IConfiguration _config;
        public string connectionString;

        public SocioController(IConfiguration config)
        {
            _config = config;
            connectionString = _config["ConnectionStrings:DefaultConnection"];

        }

        private SqlConnection OpenConn(string connectionString)
        {
            var cn = new SqlConnection(connectionString);
            cn.Open();
            return cn;
        }


        // GET: api/<SocioController>
        [HttpGet]
        public List<Socio> Get()
        {
            const string query = "SELECT SocioId  ,ClubId  ,Nombre  ,Apellido  ,FechaNacimiento  ,FechaAsociado  ,Dni  ,CantidadAsistencias  FROM [ClubDB].[dbo].[Socio]";
            using var cn = OpenConn(connectionString);
            var cmd = new SqlCommand(query, cn);

            var rd = cmd.ExecuteReader();

            List<Socio> listaSocios = new List<Socio>();

            while (rd.Read()) {
                listaSocios.Add(new Socio
                {
                    SocioId = rd.GetInt32(0),
                    ClubId = rd.GetInt32(1),
                    Nombre = rd.GetString(2),
                    Apellido = rd.GetString(3),
                    FechaNacimiento = rd.GetDateTime(4),
                    FechaAsociado = rd.GetDateTime(5),
                    Dni = rd.GetInt32(6),
                    CantidadAsistencias = rd.GetInt32(7)


                });

            }

            return listaSocios;
        }

        // GET api/<SocioController>/5
        [HttpGet("{dni}")]
        public ActionResult<Socio> Get(int dni)
        {
            const string query = "SELECT SocioId  ,ClubId  ,Nombre  ,Apellido  ,FechaNacimiento  ,FechaAsociado  ,Dni  ,CantidadAsistencias  FROM [ClubDB].[dbo].[Socio] where Dni = @dni";
            using var cn = OpenConn(connectionString);
            var cmd = new SqlCommand(query, cn);
            cmd.Parameters.Add("@dni", System.Data.SqlDbType.Int).Value = dni;

            var rd = cmd.ExecuteReader();
            Socio socio = new Socio();

            while (rd.Read())
            {

                socio.SocioId = rd.GetInt32(0);
                socio.ClubId = rd.GetInt32(1);
                socio.Nombre = rd.GetString(2);
                socio.Apellido = rd.GetString(3);
                socio.FechaNacimiento = rd.GetDateTime(4);
                socio.FechaAsociado = rd.GetDateTime(5);
                socio.Dni = rd.GetInt32(6);
                socio.CantidadAsistencias = rd.GetInt32(7);


            }

            if (socio.Apellido == null)
            {
                return NotFound(new ErrorResponse { ErrorCode = "404", Message = "No se encontro ningun socio con ese DNI" });
            }
            else {
                return socio;
            }
        }

        // POST api/<SocioController>
        [HttpPost]
        public ActionResult<Socio> CrearSocio([FromBody] Socio socio)
        {
            const string query = @"INSERT INTO [dbo].[Socio]
           ([ClubId]
           ,[Nombre]
           ,[Apellido]
           ,[FechaNacimiento]
           ,[FechaAsociado]
           ,[Dni]
           ,[CantidadAsistencias])
     VALUES
           (@clubid,@nombre,@apellido,@fechanacimiento,@fechaasociado,@dni,@cantidadasistencias);";
            try
            {
                using var cn = OpenConn(connectionString);
            var cmd = new SqlCommand(query, cn);
            cmd.Parameters.Add("@clubid", System.Data.SqlDbType.Int).Value = socio.ClubId;
            cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = socio.Nombre;
            cmd.Parameters.Add("@apellido", System.Data.SqlDbType.VarChar).Value = socio.Apellido;
            cmd.Parameters.Add("@fechanacimiento", System.Data.SqlDbType.DateTime).Value = socio.FechaNacimiento;
            cmd.Parameters.Add("@fechaasociado", System.Data.SqlDbType.DateTime).Value = socio.FechaAsociado;
            cmd.Parameters.Add("@dni", System.Data.SqlDbType.Int).Value = socio.Dni;
            cmd.Parameters.Add("@cantidadasistencias", System.Data.SqlDbType.Int).Value = socio.CantidadAsistencias;

            int result = Convert.ToInt32(cmd.ExecuteScalar());
            
            if (result == 0)
            {
                return Ok(new Responses { Code = "200", Message = "Se inserto correctamente el socio " });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno");

            }

            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno");

            }







        }

        // PUT api/<SocioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SocioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
