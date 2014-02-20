namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class DebateLinkVote
    {
        public long Id { get; set; }
        public short Vote { get; set; }
    
        public virtual DebateLink Link { get; set; }
        public virtual User Owner { get; set; }
    }
}
