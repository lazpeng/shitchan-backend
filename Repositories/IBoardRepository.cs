using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shitchan.Entities;

namespace shitchan.Repositories
{
    public interface IBoardRepository
    {
        Task<List<Board>> GetBoards();
        Task<Board> Get(string Url);
        Task<Board> Create(Board board);
    }
}
