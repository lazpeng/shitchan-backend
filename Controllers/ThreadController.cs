using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shitchan.Entities;
using shitchan.Repositories;
using System.Buffers.Text;
using System.Text;

namespace shitchan.Controllers
{
    [ApiController]
    [Route("api/threads")]
    public class ThreadController : Controller
    {
        private readonly IThreadRepository threadRepository;
        private readonly IAdminRepository adminRepository;

        public ThreadController(IThreadRepository threadRepo, IAdminRepository adminRepo)
        {
            threadRepository = threadRepo;
            adminRepository = adminRepo;
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Post post)
        {
            Post created;

            if(!string.IsNullOrEmpty(post.PictureBase64))
            {
                var maxLength = 1024 * 1024 * 1024; // 1 MB
                if(Convert.FromBase64String(post.PictureBase64).Length > maxLength)
                {
                    return BadRequest("Picture max length exceeded");
                }
            }

            if(post.ParentPostId != null)
            {
                created = await threadRepository.CreatePost(post);
            } else
            {
                var thread = await threadRepository.CreateThread(post);
                created = thread.Children[0];
            }

            return Created(created.Id.ToString(), created);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete(long postId, [FromBody] string code)
        {
            if(!await adminRepository.ValidateCode(code))
            {
                return Forbid();
            }

            await threadRepository.DeletePost(postId);

            return Ok();
        }

        [HttpPut("sticky")]
        public async Task<IActionResult> SetStickied([FromBody] StickyThreadRequest request, [FromQuery] bool stickied = true)
        {
            if(!await adminRepository.ValidateCode(request.Code))
            {
                return BadRequest();
            }

            await threadRepository.StickThread(request.PostNumber, stickied);
            return Ok();
        }

        [HttpPost("report")]
        public async Task<IActionResult> Report([FromBody] long postId)
        {
            await threadRepository.ReportPost(postId);

            return Ok();
        }
    }
}
