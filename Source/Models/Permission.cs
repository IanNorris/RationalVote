namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class Permission
    {
        public long Id { get; set; }
        public short Type { get; set; }
        public Nullable<short> Value { get; set; }
    
        public virtual User User { get; set; }
    }
}
