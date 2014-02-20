namespace RationalVote
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.AuthenticationMethod = 0;
            this.Verified = false;
            this.Sessions = new HashSet<Session>();
            this.Debates = new HashSet<Debate>();
            this.Votes = new HashSet<DebateLinkVote>();
            this.Permissions = new HashSet<Permission>();
        }
    
        public long Id { get; set; }
        public string Email { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public byte AuthenticationMethod { get; set; }
        public bool Verified { get; set; }
    
        public virtual ICollection<Session> Sessions { get; set; }
        public virtual Profile Profiles { get; set; }
        public virtual ICollection<Debate> Debates { get; set; }
        public virtual ICollection<DebateLinkVote> Votes { get; set; }
        public virtual EmailVerificationToken EmailVerificationTokens { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
