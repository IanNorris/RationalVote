namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class Debate
    {
        public Debate()
        {
            this.Status = 0;
            this.DebateTags = new HashSet<DebateTag>();
            this.LinkParents = new HashSet<DebateLink>();
            this.LinkChildren = new HashSet<DebateLink>();
        }
    
        public long Id { get; set; }
        public short Status { get; set; }
        public string Title { get; set; }
        public System.DateTime Posted { get; set; }
        public System.DateTime Updated { get; set; }
    
        public virtual User Owner { get; set; }
        public virtual ICollection<DebateTag> DebateTags { get; set; }
        public virtual ICollection<DebateLink> LinkParents { get; set; }
        public virtual ICollection<DebateLink> LinkChildren { get; set; }
    }
}
