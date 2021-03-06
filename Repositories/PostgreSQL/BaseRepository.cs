﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
using System.IO;

namespace shitchan.Repositories.PostgreSQL
{
    public abstract class BaseRepository
    {
        protected readonly ConnectionStringProvider connectionStringProvider;

        public BaseRepository(ConnectionStringProvider provider)
        {
            connectionStringProvider = provider;
            PerformUpgrade().Wait();
        }

        public long LatestDatabaseVersion { get; } = 1;

        private string GetUpgradeScript(long version)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = $"{assembly.GetName().Name}.Repositories.PostgreSQL.Scripts.v{version}.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine($"Failed to load upgrade script: \"{resourceName}\"");
                return null;
            }

            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        private async Task UpdateDbVersion(long current)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("INSERT INTO DbUpgrade (Version, DateInstalled) VALUES (@current, CURRENT_DATE)", new { current });
        }

        public async Task PerformUpgrade()
        {
            long currentVersion = await GetDatabaseVersion();

            try
            {
                while (currentVersion < LatestDatabaseVersion)
                {
                    var script = GetUpgradeScript(currentVersion);
                    if (script == null)
                    {
                        return;
                    }

                    using var conn = await GetConnection();
                    await conn.ExecuteAsync(script);

                    await UpdateDbVersion(currentVersion + 1);
                    currentVersion = await GetDatabaseVersion();

                    Console.WriteLine($"Successfully performed upgrade to database version {currentVersion}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred when performing database upgrade: {e.Message}\n{e.StackTrace}");
            }
        }

        public async Task<long> GetDatabaseVersion()
        {
            long version;

            try
            {
                using var conn = await GetConnection();
                version = await conn.QuerySingleAsync<long>("SELECT COALESCE(MAX(Version), 0) FROM DbUpgrade");
            }
            catch (Exception e)
            {
                if (!(e is NpgsqlException))
                {
                    Console.WriteLine($"Something wrong occurred when checking database version: {e.Message}");
                }
                version = 0;
            }

            return version;
        }

        public async Task<IDbConnection> GetConnection()
        {
            var conn = new NpgsqlConnection(connectionStringProvider.ConnectionString);
            await conn.OpenAsync();

            return conn;
        }
    }
}
