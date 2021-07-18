using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shitchan.Repositories
{
    public class ConnectionStringProvider
    {
        public string ConnectionString { get; private set; }

        private string FromHerokuConnectionString(string Conn)
        {
            var databaseUri = new Uri(Conn);
            var userInfo = databaseUri.UserInfo.Split(':');

            return new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                Pooling = true,
                MinPoolSize = 1,
                MaxPoolSize = 20,
                SslMode = SslMode.Require,
            }.ToString();
        }

        public ConnectionStringProvider(string ConnectionString)
        {
            if (ConnectionString.StartsWith("postgres://"))
            {
                ConnectionString = FromHerokuConnectionString(ConnectionString);
            }
            this.ConnectionString = ConnectionString;
        }
    }
}
