using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shitchan.Entities;

namespace shitchan.Repositories.PostgreSQL
{
    public class ThreadRepository : BaseRepository, IThreadRepository
    {
        public ThreadRepository(ConnectionStringProvider provider) : base(provider)
        {
        }

        public async Task<Post> CreatePost(Post content)
        {
            using var conn = await GetConnection();

            var query = "INSERT INTO POSTS (TITLE, BOARD, AUTHOR, POSTED, CONTENT, HASH, PARENT, FILENAME, FILEDATA) VALUES (@Title, @Board, @Author, @Timestamp, @Content, @AuthorHash, @ParentPostId, @PictureFilename, decode(@PictureBase64, 'base64')) RETURNING ID";

            content.Id = conn.ExecuteScalar<long>(query, content);
            return content;
        }

        public async Task<Thread> CreateThread(Post ParentPost)
        {
            var created = await CreatePost(ParentPost);

            return await Get(created.Id);
        }

        public async Task<List<Thread>> GetThreads(string Board)
        {
            using var conn = await GetConnection();

            var query = "SELECT Board, Id as ParentPostId, POSTED as TimestampCreated, (select max(POSTED) from POSTS p where p.PARENT = ID OR p.ID = ID) as TimestampUpdated from POSTS where Board = @Board AND PARENT IS NULL ORDER BY TimestampUpdated DESC";

            var threads = await conn.QueryAsync<Thread>(query, new { Board });
            foreach(var thread in threads)
            {
                thread.Children = await RefreshThread(thread.ParentPostId, -1);
            }

            return threads.ToList();
        }

        public async Task<Thread> Get(long postId)
        {
            using var conn = await GetConnection();

            var query = "SELECT Board, Id as ParentPostId, POSTED as TimestampCreated, (select max(POSTED) from POSTS where PARENT = @Parent) as TimestampUpdated from POSTS where ID = @Parent";

            var thread = await conn.QuerySingleAsync<Thread>(query, new { Parent = postId });
            thread.Children = await RefreshThread(postId, -1);

            return thread;
        }

        public async Task<List<Post>> RefreshThread(long ThreadParentId, long LastNumber)
        {
            using var conn = await GetConnection();

            var query = "SELECT Id, Title, Author, POSTED as Timestamp, Board, HASH as AuthorHash, Content, PARENT as ParentPostId, FILENAME as PictureFilename, encode(FILEDATA, 'base64') as PictureBase64 FROM POSTS WHERE (PARENT = @ThreadParentId OR ID = @ThreadParentId) AND ID > @LastNumber";

            return (await conn.QueryAsync<Post>(query, new { ThreadParentId, LastNumber })).ToList();
        }

        public async Task DeletePost(long Id)
        {
            using var conn = await GetConnection();

            var query = "DELETE FROM POSTS WHERE ID = @Id";

            await conn.ExecuteAsync(query, new { Id });
        }
    }
}
