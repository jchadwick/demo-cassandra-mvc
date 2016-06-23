using Cassandra;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace CassandraDemo.Models
{
    public class PostsContext
    {
        static Lazy<ISession> DefaultSession = new Lazy<ISession>(Initialize);

        private readonly ISession _session;

        public PostsContext()
            : this(DefaultSession.Value)
        {
        }

        public PostsContext(ISession session)
        {
            _session = session;
        }

        public string Add(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            var id = TimeUuid.NewId();

            var statement = 
                _session
                    .Prepare("INSERT INTO posts (id, title, body, timestamp)" +
                             "            VALUES(?, ?, ?, toTimestamp(now()))")
                    .Bind(id, post.Title, post.Body);

            _session.Execute(statement);

            var newPost = GetPostByIdInternal(id);
            post.Id = newPost.Id;

            return post.Id;
        }

        public Post GetPostById(string id)
        {
            var uuid = Guid.Parse(id);
            return GetPostByIdInternal(uuid);
        }

        internal Post GetPostByIdInternal(Guid id)
        {
            var statement =
                _session
                    .Prepare("SELECT * FROM Posts WHERE id = ?;")
                    .Bind(id);

            return _session
                    .Execute(statement)
                    .GetRows()
                    .Select(MapPost)
                    .FirstOrDefault();
        }

        public void Delete(string id)
        {
            var statement = 
                _session
                    .Prepare("DELETE FROM POsts WHERE id = ?;")
                    .Bind(id);

            _session.Execute(statement);
        }

        public IEnumerable<Post> GetPosts()
        {
            return _session
                    .Execute("SELECT * FROM Posts;")
                    .GetRows()
                    .Select(MapPost)
                    .ToArray();
        }

        private static Post MapPost(Row row)
        {
            var timestamp = row.GetValue<DateTime?>("timestamp");

            return new Post
            {
                Id = row.GetValue<Guid>("id").ToString("n"),
                Body = row.GetValue<string>("body"),
                Title = row.GetValue<string>("title"),
                Timestamp = timestamp.GetValueOrDefault(DateTime.MinValue),
            };
        }

        private static ISession Initialize()
        {
            var contactPoint = ConfigurationManager.AppSettings["BlogDb.ContactPoint"];
            var port = ConfigurationManager.AppSettings["BlogDb.Port"];
            var keyspace = ConfigurationManager.AppSettings["BlogDb.Keyspace"];

            return Cluster
                    .Builder()
                    .AddContactPoint(contactPoint)
                        .WithPort(int.Parse(port))
                    .Build()
                    .Connect(keyspace);
        }
    }
}