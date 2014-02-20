namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailVerificationToken
    {
        public long Id { get; set; }
        public System.DateTime Created { get; set; }
    
        public virtual User User { get; set; }
    }
}
