using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoesApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace ShoesApp.Api
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmpleadosController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public EmpleadosController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Empleado entidad)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: entidad.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var e = contexto.Empleados.FirstOrDefault(x => x.Email == entidad.Email);
                if (e == null || e.Clave != hashed)
                {
                    return BadRequest("Datos Incorrectos");
                }
                else
                {
                    if (e.Estado == 0)
                    {
                        e.Estado = 1;
                        contexto.Empleados.Update(e);
                        contexto.SaveChanges();
                    }

                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, "Empleado"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(120),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Empleados
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(contexto.Empleados.FirstOrDefault(x => x.Email == usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Empleados/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(contexto.Empleados.SingleOrDefault(x => x.idEmpleado == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Empleados
        [HttpPost]
        public async Task<IActionResult> Post(Empleado entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                password: entidad.Clave,
                                                salt: System.Text.Encoding.ASCII.GetBytes("SALADA"),
                                                prf: KeyDerivationPrf.HMACSHA1,
                                                iterationCount: 1000,
                                                numBytesRequested: 256 / 8));
                    contexto.Empleados.Add(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.idEmpleado }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Empleados/5

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Empleado entidad)
        {
            try
            {

                if (ModelState.IsValid && contexto.Empleados.AsNoTracking().SingleOrDefault(e => e.idEmpleado == id) != null)
                {
                    //entidad.Id = id;
                    if (contexto.Empleados.Where(x => x.Clave != entidad.Clave).Count().Equals(1))
                    {
                        entidad.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                password: entidad.Clave,
                                                salt: System.Text.Encoding.ASCII.GetBytes("SALADA"),
                                                prf: KeyDerivationPrf.HMACSHA1,
                                                iterationCount: 1000,
                                                numBytesRequested: 256 / 8));
                    }

                    contexto.Empleados.Update(entidad);
                    contexto.SaveChanges();
                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //Aqui se da de baja al empleado que inicio sesion
                var entidad = contexto.Empleados.FirstOrDefault(e => e.idEmpleado == id && e.Email == User.Identity.Name);
                if (entidad != null)
                {
                    entidad.Estado = 0;
                    contexto.Empleados.Update(entidad);
                    contexto.SaveChanges();
                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}