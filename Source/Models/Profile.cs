namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class Profile
    {
        public Profile()
        {
            this.Experience = 0;
        }
    
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string RealName { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public System.DateTime Joined { get; set; }
        public long Experience { get; set; }
    
        public virtual User User { get; set; }
    }
}
