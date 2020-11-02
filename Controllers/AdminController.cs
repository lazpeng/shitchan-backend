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

        [HttpGet("{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            return Ok(await adminRepository.ValidateCode(code));
        }

        [HttpPut("{oldCode}")]
        public async Task<IActionResult> Update(string oldCode, [FromBody] string newCode)
        {
            var result = await adminRepository.UpdateCode(oldCode, newCode);

            if(result)
            {
                return Created(newCode, newCode);
            } else return NotFound();
        }
    }
}
