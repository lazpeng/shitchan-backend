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

        public async Task<Board> Create(Board board)
        {
            using var conn = await GetConnection();

            var query = "INSERT INTO BOARDS (ROUTE, TITLE, DESCRIPTION) VALUES (@Route, @Title, @Description)";

            await conn.ExecuteAsync(query, new { board.Route, board.Title, board.Description });

            return await Get(board.Route);
        }

        public async Task<Board> Get(string Url)
        {
            using var conn = await GetConnection();

            var query = "SELECT ROUTE, TITLE, DESCRIPTION FROM BOARDS WHERE ROUTE = @URL";

            return await conn.QuerySingleAsync<Board>(query, new { URL = Url });
        }

        public async Task<List<Board>> GetBoards()
        {
            using var conn = await GetConnection();

            var query = "SELECT ROUTE, TITLE, DESCRIPTION FROM BOARDS";

            return (await conn.QueryAsync<Board>(query)).ToList();
        }
    }
}
