namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tag
    {
        public Tag()
        {
            this.Tags = new HashSet<DebateTag>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<DebateTag> Tags { get; set; }
    }
}
