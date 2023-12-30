using JWTTEST.Models;
using JWTTEST.service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult>RegisterAsync([FromBody]RegstirModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterAsynce(model);
            if (!result.IsAuthenticated)
                BadRequest(result.Massege);

            return Ok(result);
         
        }
        [HttpGet("token")]
        public async Task<IActionResult>GetToken([FromBody]TokenReguestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.GetTokenAsynce(model);
            if(!result.IsAuthenticated)
            {
                return BadRequest(result.Massege);
            }
            return Ok(result);
        }

    }
}
