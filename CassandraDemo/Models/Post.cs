using System;
using System.ComponentModel.DataAnnotations;

namespace CassandraDemo.Models
{
    public enum PostAction
    {
        Added,
        Updated,
        Deleted,
    }

    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public DateTime Timestamp { get; set; }
    }
}