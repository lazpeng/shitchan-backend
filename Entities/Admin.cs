using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shitchan.Entities
{
    public class Admin
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long RegisteredTimestamp { get; set; }
    }
}
