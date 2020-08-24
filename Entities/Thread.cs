using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shitchan.Entities
{
    public class Thread
    {
        public string Board { get; set; }
        public long ParentPostId { get; set; }
        public long TimestampCreated { get; set; }
        public long TimestampUpdated { get; set; }
        public List<Post> Children { get; set; }
    }
}
