namespace RationalVote
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Infrastructure;

	using Models;

	public partial class RationalVoteContext : DbContext
	{
		public RationalVoteContext()
			: base( "RationalVoteContext" )
		{
		}

		protected override void OnModelCreating( DbModelBuilder modelBuilder )
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<RationalVoteContext, RationalVote.Migrations.Configuration>()); 
			base.OnModelCreating( modelBuilder );
		}

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Session> Sessions { get; set; }
		public virtual DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
		public virtual DbSet<Profile> Profiles { get; set; }
		public virtual DbSet<Debate> Debates { get; set; }
		public virtual DbSet<Tag> Tags { get; set; }
		public virtual DbSet<DebateTag> DebateTags { get; set; }
		public virtual DbSet<DebateLink> DebateLinks { get; set; }
		public virtual DbSet<DebateLinkVote> DebateLinkVotes { get; set; }
		public virtual DbSet<Permission> Permissions { get; set; }
	}
}
