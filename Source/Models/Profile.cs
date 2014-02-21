namespace RationalVote.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public class Profile
    {
        public Profile()
        {
            this.Experience = 0;
        }
    
        [Key]
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string RealName { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public System.DateTime Joined { get; set; }
        public long Experience { get; set; }
    
        [Required]
        [ForeignKey("Id")]
        public virtual User User { get; set; }
    }
}
