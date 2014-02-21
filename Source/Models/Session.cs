namespace RationalVote.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Session
    {
        public Session()
        {
            this.KeepLoggedIn = false;
        }
    
        public long Id { get; set; }
        public string Token { get; set; }
        public string IP { get; set; }
        public bool KeepLoggedIn { get; set; }
        public System.DateTime LastSeen { get; set; }
        public System.DateTimeOffset Life { get; set; }
    
        public virtual User User { get; set; }
    }
}
