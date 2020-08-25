using Dapper;
using shitchan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace shitchan.Repositories.PostgreSQL
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        public AdminRepository(ConnectionStringProvider provider) : base(provider)
        {
        }

        public async Task<Admin> Get(long Id)
        {
            using var conn = await GetConnection();

            var query = "SELECT ID, USER, DATEREGISTERED as RegisteredTimestamp FROM ADMINS";

            return await conn.QuerySingleAsync<Admin>(query, new { Id });
        }

        private string HashPassword(string password, string salt)
        {
            var hasher = SHA256.Create();
            return Encoding.UTF8.GetString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password + salt)));
        }

        private string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<string> Login(Admin Target)
        {
            using var conn = await GetConnection();

            string Token = null;

            string PasswordHash = "", PasswordSalt = "";

            var query = "SELECT PASSWORDHASH, PASSWORDSALT FROM ADMINS WHERE ID = @Id";

            var reader = await conn.ExecuteReaderAsync(query, new { Target.Id });
            if(reader.Read())
            {
                PasswordHash = reader.GetString(reader.GetOrdinal("PASSWORDHASH"));
                PasswordSalt = reader.GetString(reader.GetOrdinal("PASSWORDSALT"));

                reader.Close();

                var ComputedHash = HashPassword(Target.Password, PasswordSalt);

                if(PasswordHash == ComputedHash)
                {
                    query = "DELETE FROM SESSIONS WHERE ADMIN = @Id";

                    await conn.ExecuteAsync(query, new { Target.Id });

                    Token = GenerateUUID();

                    query = "INSERT INTO SESSIONS (TOKEN, ADMIN) VALUES (@Token, @Id)";

                    await conn.ExecuteAsync(query, new { Token, Target.Id });
                }
            }

            return Token;
        }

        public async Task<Admin> Register(string Invite, Admin Target)
        {
            using var conn = await GetConnection();

            var Salt = GenerateUUID();
            var Hash = HashPassword(Target.Password, Salt);
            var Registered = DateTimeOffset.Now.ToUnixTimeSeconds();

            var query = "INSERT INTO ADMINS (USER, PASSWORDHASH, PASSWORDSALT, DATEREGISTERED) VALUES (@User, @Hash, @Salt, @Registered) RETURNING ID";

            var id = await conn.ExecuteScalarAsync<long>(query, new { Target.User, Hash, Salt, Registered });

            return await Get(id);
        }

        public async Task<Admin> ValidateToken(string Token)
        {
            using var conn = await GetConnection();

            var query = "SELECT ADMIN FROM SESSIONS WHERE TOKEN = @Token";

            var id = await conn.ExecuteScalarAsync<long?>(query, new { Token });

            if(id != null)
            {
                return await Get(id.Value);
            } else
            {
                return null;
            }
        }
    }
}
