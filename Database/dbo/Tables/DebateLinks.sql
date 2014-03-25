CREATE TABLE [dbo].[DebateLinks] (
    [Id]         BIGINT  IDENTITY (1, 1) NOT NULL,
    [Type]       TINYINT NOT NULL,
    [Weight]     BIGINT  NOT NULL,
    [Debate_Id]  BIGINT  NULL,
    [Debate_Id1] BIGINT  NULL,
    [Child_Id]   BIGINT  NULL,
    [Parent_Id]  BIGINT  NULL,
    CONSTRAINT [PK_dbo.DebateLinks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DebateLinks_dbo.Debates_Child_Id] FOREIGN KEY ([Child_Id]) REFERENCES [dbo].[Debates] ([Id]),
    CONSTRAINT [FK_dbo.DebateLinks_dbo.Debates_Debate_Id] FOREIGN KEY ([Debate_Id]) REFERENCES [dbo].[Debates] ([Id]),
    CONSTRAINT [FK_dbo.DebateLinks_dbo.Debates_Debate_Id1] FOREIGN KEY ([Debate_Id1]) REFERENCES [dbo].[Debates] ([Id]),
    CONSTRAINT [FK_dbo.DebateLinks_dbo.Debates_Parent_Id] FOREIGN KEY ([Parent_Id]) REFERENCES [dbo].[Debates] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Debate_Id]
    ON [dbo].[DebateLinks]([Debate_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Debate_Id1]
    ON [dbo].[DebateLinks]([Debate_Id1] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Child_Id]
    ON [dbo].[DebateLinks]([Child_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Parent_Id]
    ON [dbo].[DebateLinks]([Parent_Id] ASC);

