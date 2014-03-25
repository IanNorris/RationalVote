CREATE TABLE [dbo].[DebateLinkVotes] (
    [Id]       BIGINT   IDENTITY (1, 1) NOT NULL,
    [Vote]     SMALLINT NOT NULL,
    [Link_Id]  BIGINT   NULL,
    [Owner_Id] BIGINT   NULL,
    CONSTRAINT [PK_dbo.DebateLinkVotes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DebateLinkVotes_dbo.DebateLinks_Link_Id] FOREIGN KEY ([Link_Id]) REFERENCES [dbo].[DebateLinks] ([Id]),
    CONSTRAINT [FK_dbo.DebateLinkVotes_dbo.Users_Owner_Id] FOREIGN KEY ([Owner_Id]) REFERENCES [dbo].[Users] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Link_Id]
    ON [dbo].[DebateLinkVotes]([Link_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Owner_Id]
    ON [dbo].[DebateLinkVotes]([Owner_Id] ASC);

