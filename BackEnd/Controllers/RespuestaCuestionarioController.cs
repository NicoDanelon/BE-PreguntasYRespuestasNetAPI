using BackEnd.Domain.IServices;
using BackEnd.Domain.Models;
using BackEnd.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RespuestaCuestionarioController : ControllerBase
    {
        private readonly IRespuestaCuestionarioService _respuestaCuestionarioService;
        private readonly ICuestionarioService _cuetionarioService;
        public RespuestaCuestionarioController(IRespuestaCuestionarioService respuestaCuestionarioService, ICuestionarioService cuestionarioService)
        {
            _respuestaCuestionarioService = respuestaCuestionarioService;
            _cuetionarioService = cuestionarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RespuestaCuestionario respuestaCuestionario)
        {
            try
            {
                await _respuestaCuestionarioService.SaveRespuestaCuestionario(respuestaCuestionario);
                return Ok(new { message = "Respuesta del Cuestionario enviada." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{idCuestionario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
        public async Task<IActionResult> Get(int idCuestionario)
        {
            try
            {
                var identy = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = JwtConfigurator.GetTokenIdUsuario(identy);

                var listRespuestasCuestionario = await _respuestaCuestionarioService.ListRespuestaCuestionario(idCuestionario, idUsuario);

                if(listRespuestasCuestionario == null)
                {
                    return BadRequest(new { message = "Error al buscar el listado de respuestas" });
                }
                else
                {
                    return Ok(listRespuestasCuestionario);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var identy = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = JwtConfigurator.GetTokenIdUsuario(identy);

                var respuestaCuestionario = await _respuestaCuestionarioService.BuscarRespuestaCuestionario(id, idUsuario);

                if(respuestaCuestionario == null)
                {
                    return BadRequest(new { message = "Error al buscar al respuesta al cuestionario." });
                }

                await _respuestaCuestionarioService.EliminarRespuestaCuestionario(respuestaCuestionario);
                return Ok(new { message = "La respuesta fue eliminada." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetCuestionarioByIdRespuesta/{idRespuesta}")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCuestionarioByIdRespuesta(int idRespuesta)
        {
            try
            {
                int idCuestionario = await _respuestaCuestionarioService.getIdCuestionarioByIdRespuesta(idRespuesta);

                var cuestionario = await _cuetionarioService.GetCuestionario(idCuestionario);

                var listRespuestas = await _respuestaCuestionarioService.GetListRespuestas(idRespuesta);

                return Ok(new { cuestionario = cuestionario, respuestas = listRespuestas });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
