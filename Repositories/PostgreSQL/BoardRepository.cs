using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shitchan.Entities;

namespace shitchan.Repositories.PostgreSQL
{
    public class BoardRepository : BaseRepository, IBoardRepository
    {
        public BoardRepository(ConnectionStringProvider provider) : base(provider)
        {
        }

        public async Task<Board> Get(string Url)
        {
            using var conn = await GetConnection();

            var query = "SELECT * FROM BOARDS WHERE ROUTE = @URL";

            return await conn.QuerySingleAsync<Board>(query, new { URL = Url });
        }

        public async Task<List<Board>> GetBoards()
        {
            using var conn = await GetConnection();

            var query = "SELECT * FROM BOARDS";

            return (await conn.QueryAsync<Board>(query)).ToList();
        }
    }
}
