using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shitchan.Entities;

namespace shitchan.Repositories
{
    public interface IThreadRepository
    {
        Task<List<Thread>> GetThreads(string Board);
        Task<Post> CreatePost(Post content);
        Task<List<Post>> RefreshThread(long ThreadParentId, long LastNumber);
        Task<Thread> Get(long ParentPostId);
        Task<Thread> CreateThread(string Board, Post ParentPost);
    }
}
