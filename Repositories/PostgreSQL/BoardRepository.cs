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

        public async Task<Board> Create(Board board, long CreatedBy)
        {
            using var conn = await GetConnection();

            var query = "INSERT INTO BOARDS (ROUTE, TITLE, DESCRIPTION, CREATEDBY) VALUES (@Route, @Title, @Description, @CreatedBy)";

            await conn.ExecuteAsync(query, new { board.Route, board.Title, board.Description, CreatedBy });

            return await Get(board.Route);
        }

        public async Task<Board> Get(string Url)
        {
            using var conn = await GetConnection();

            var query = "SELECT ROUTE, TITLE, DESCRIPTION, a.USERNAME AS CreatedBy FROM BOARDS INNER JOIN ADMINS a ON a.ID = CREATEDBY WHERE ROUTE = @URL";

            return await conn.QuerySingleAsync<Board>(query, new { URL = Url });
        }

        public async Task<List<Board>> GetBoards()
        {
            using var conn = await GetConnection();

            var query = "SELECT ROUTE, TITLE, DESCRIPTION, a.USERNAME AS CreatedBy FROM BOARDS INNER JOIN ADMINS a ON a.ID = CREATEDBY";

            return (await conn.QueryAsync<Board>(query)).ToList();
        }
    }
}
