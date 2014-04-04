namespace RationalVote.Models
{
    using System;
    using System.Collections.Generic;
    
    public class Permission
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public int? Value { get; set; }
    
        public virtual User User { get; set; }
    }
}
