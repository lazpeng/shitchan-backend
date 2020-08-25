using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shitchan.Entities;
using shitchan.Repositories;

namespace shitchan.Controllers
{
    [ApiController]
    [Route("api/boards")]
    public class BoardController : Controller
    {
        private readonly IBoardRepository boardRepository;
        private readonly IAdminRepository adminRepository;

        public BoardController(IBoardRepository boardRepo, IAdminRepository adminRepo)
        {
            boardRepository = boardRepo;
            adminRepository = adminRepo;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return Ok(await boardRepository.GetBoards());
        }

        [HttpGet("{url}")]
        public async Task<IActionResult> Get(string url)
        {
            var result = await boardRepository.Get(url);

            if(result != null)
            {
                return Ok(result);
            } else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BoardRegister board)
        {
            var admin = await adminRepository.ValidateToken(board.Token);

            if(admin == null)
            {
                return Forbid();
            }

            var result = await boardRepository.Create(board, admin.Id);
            return Created(result.Route, result);
        }
    }
}
