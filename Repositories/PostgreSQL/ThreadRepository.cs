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

            var query = "INSERT INTO POSTS (TITLE, BOARD, AUTHOR, POSTED, CONTENT, AUTHORHASH, PARENTPOSTID, PICTUREFILENAME, FILEDATA) VALUES (@Title, @Board, @Author, @Posted, @Content, @AuthorHash, @ParentPostId, @PictureFilename, decode(@PictureBase64, 'base64')) RETURNING ID";

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

            var query = @"
                        (SELECT t.Board,
                                 t.Id as ParentPostId,
                                 t.POSTED as TimestampCreated,
                                 COALESCE((select max(POSTED) from POSTS p where p.PARENTPOSTID = t.ID), t.POSTED) as TimestampUpdated,
                                 (select count(*) from POSTS where PARENTPOSTID = t.ID) as NumberOfPosts,
                                 STICKIED
                        from POSTS t where Board = @Board AND PARENTPOSTID IS NULL AND STICKIED = TRUE AND HIDDEN = FALSE ORDER BY 4 DESC)
                        UNION ALL
                        (SELECT t.Board,
                                 t.Id as ParentPostId,
                                 t.POSTED as TimestampCreated,
                                 COALESCE((select max(POSTED) from POSTS p where p.PARENTPOSTID = t.ID), t.POSTED) as TimestampUpdated,
                                 (select count(*) from POSTS where PARENTPOSTID = t.ID) as NumberOfPosts,
                                 STICKIED
                        from POSTS t where Board = @Board AND PARENTPOSTID IS NULL AND STICKIED = FALSE AND HIDDEN = FALSE ORDER BY 4 DESC)";

            var threads = await conn.QueryAsync<Thread>(query, new { Board });

            var latestPostsQuery = @"
            (SELECT Id, Title, Author, POSTED, Board, AuthorHash, Content, ParentPostId, PictureFilename, encode(FILEDATA, 'base64') as PictureBase64, Stickied
            FROM POSTS
            WHERE PARENTPOSTID = @ThreadParentId
            ORDER BY POSTED DESC
            LIMIT 3)
            UNION ALL
            (SELECT Id, Title, Author, POSTED, Board, AuthorHash, Content, ParentPostId, PictureFilename, encode(FILEDATA, 'base64') as PictureBase64, Stickied
            FROM POSTS
            WHERE ID = @ThreadParentId)
            ";

            foreach (var thread in threads)
            {
                thread.Children = (await conn.QueryAsync<Post>(latestPostsQuery, new { ThreadParentId = thread.ParentPostId })).Reverse().ToList();
            }

            return threads.ToList();
        }

        public async Task<Thread> Get(long postId)
        {
            using var conn = await GetConnection();

            var query = "SELECT Board, ID as ParentPostId, POSTED as TimestampCreated, (select count(*) from POSTS where PARENTPOSTID = @Parent) as NumberOfPosts, (select max(POSTED) from POSTS where PARENTPOSTID = @Parent) as TimestampUpdated from POSTS where ID = @Parent";

            var thread = await conn.QuerySingleAsync<Thread>(query, new { Parent = postId });
            thread.Children = await RefreshThread(postId, -1);

            return thread;
        }

        public async Task<List<Post>> RefreshThread(long ThreadParentId, long LastNumber)
        {
            using var conn = await GetConnection();

            var query = "SELECT Id, Title, Author, POSTED, Board, AuthorHash, Content, ParentPostId, PictureFilename, encode(FILEDATA, 'base64') as PictureBase64, Stickied FROM POSTS WHERE (PARENTPOSTID = @ThreadParentId OR ID = @ThreadParentId) AND ID > @LastNumber";

            return (await conn.QueryAsync<Post>(query, new { ThreadParentId, LastNumber })).ToList();
        }

        public async Task DeletePost(long Id)
        {
            using var conn = await GetConnection();

            var query = "DELETE FROM POSTS WHERE ID = @Id";

            await conn.ExecuteAsync(query, new { Id });
        }

        public async Task StickThread(long threadNumber, bool stickied)
        {
            using var conn = await GetConnection();

            var query = "UPDATE POSTS SET STICKIED = @stickied WHERE ID = @threadNumber";

            await conn.ExecuteAsync(query, new { stickied, threadNumber });
        }

        public async Task ReportPost(long postId)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE POSTS SET HIDDEN = TRUR WHERE ID = @postId", new { postId });
        }
    }
}
