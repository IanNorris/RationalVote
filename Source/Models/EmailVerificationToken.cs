namespace RationalVote.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public class EmailVerificationToken
    {
        [Key]
        public long Id { get; set; }
        public System.DateTime Created { get; set; }
    
        [Required]
        [ForeignKey("Id")]
        public virtual User User { get; set; }
    }
}
