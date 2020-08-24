﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shitchan.Repositories;

namespace shitchan.Controllers
{
    [ApiController]
    [Route("api/boards")]
    public class BoardController : Controller
    {
        private readonly IBoardRepository boardRepository;

        public BoardController(IBoardRepository repo)
        {
            boardRepository = repo;
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
    }
}
