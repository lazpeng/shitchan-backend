using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shitchan.Entities;
using shitchan.Repositories;

namespace shitchan.Controllers
{
    [ApiController]
    [Route("api/threads")]
    public class ThreadController : Controller
    {
        private readonly IThreadRepository threadRepository;

        public ThreadController(IThreadRepository repo)
        {
            threadRepository = repo;
        }

        [HttpGet("board/{board}")]
        public async Task<IActionResult> GetThreads(string board)
        {
            return Ok(await threadRepository.GetThreads(board));
        }

        [HttpGet("refresh/{threadId}")]
        public async Task<IActionResult> Refresh(long threadId, [FromQuery] long? lastNumber)
        {
            return Ok(await threadRepository.RefreshThread(threadId, lastNumber ?? 0));
        }

        [HttpGet("{threadId}")]
        public async Task<IActionResult> Get(long threadId)
        {
            return Ok(await threadRepository.Get(threadId));
        }

        [HttpPost("{board}")]
        public async Task<IActionResult> NewThread(string board, [FromBody] Post parent)
        {
            var created = await threadRepository.CreateThread(board, parent);

            return Created(created.ParentPostId.ToString(), created);
        }

        [HttpPost]
        public async Task<IActionResult> NewReply([FromBody] Post reply)
        {
            var created = await threadRepository.CreatePost(reply);

            return Created(created.Id.ToString(), created);
        }
    }
}
