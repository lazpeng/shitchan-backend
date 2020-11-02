using Dapper;
using System.Threading.Tasks;

namespace shitchan.Repositories.PostgreSQL
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        public AdminRepository(ConnectionStringProvider provider) : base(provider)
        {
        }

        public async Task<bool> ValidateCode(string code)
        {
            using var conn = await GetConnection();

            return (await conn.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM CODES WHERE CODE = @code", new { code })) > 0;
        }

        public async Task<bool> UpdateCode(string oldCode, string newCode)
        {
            using var conn = await GetConnection();

            return (await conn.ExecuteAsync("UPDATE CODES SET CODE = @newCode WHERE CODE = @oldCode", new { oldCode, newCode })) > 0;
        }
    }
}
