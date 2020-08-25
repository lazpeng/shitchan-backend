using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shitchan.Entities;
using shitchan.Repositories;

namespace shitchan.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository adminRepository;

        public AdminController(IAdminRepository repository)
        {
            adminRepository = repository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Admin admin)
        {
            var result = await adminRepository.Login(admin);

            if(string.IsNullOrEmpty(result))
            {
                return NotFound();
            } else
            {
                return Ok(result);
            }
        }

        [HttpPost("{invite}")]
        public async Task<IActionResult> Register(string invite, [FromBody] Admin admin)
        {
            var result = await adminRepository.Register(invite, admin);

            if(result != null)
            {
                return Ok(result);
            } else
            {
                return Forbid();
            }
        }
    }
}
