namespace RationalVote.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DebateTag
    {
        public long Id { get; set; }
    
        public virtual Tag Tag { get; set; }
        public virtual Debate Debate { get; set; }
    }
}
