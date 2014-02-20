namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class DebateLink
    {
        public DebateLink()
        {
            this.Votes = new HashSet<DebateLinkVote>();
        }
    
        public long Id { get; set; }
        public byte Type { get; set; }
        public long Weight { get; set; }
    
        public virtual Debate Parent { get; set; }
        public virtual Debate Child { get; set; }
        public virtual ICollection<DebateLinkVote> Votes { get; set; }
    }
}
