using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace shitchan.Entities
{
    public class Post
    {
        public long Id { get; set; }
        [MaxLength(128)]
        public string Title { get; set; }
        [MaxLength(4)]
        public string Board { get; set; }
        [MaxLength(128)]
        public string Author { get; set; }
        public long Timestamp { get; set; }
        [MaxLength(256)]
        public string Content { get; set; }
        [MaxLength(32)]
        public string AuthorHash { get; set; }
        public long? ParentPostId { get; set; }
    }
}
