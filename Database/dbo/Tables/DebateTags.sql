CREATE TABLE [dbo].[DebateTags] (
    [Id]        BIGINT IDENTITY (1, 1) NOT NULL,
    [Debate_Id] BIGINT NULL,
    [Tag_Id]    BIGINT NULL,
    CONSTRAINT [PK_dbo.DebateTags] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DebateTags_dbo.Debates_Debate_Id] FOREIGN KEY ([Debate_Id]) REFERENCES [dbo].[Debates] ([Id]),
    CONSTRAINT [FK_dbo.DebateTags_dbo.Tags_Tag_Id] FOREIGN KEY ([Tag_Id]) REFERENCES [dbo].[Tags] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Debate_Id]
    ON [dbo].[DebateTags]([Debate_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tag_Id]
    ON [dbo].[DebateTags]([Tag_Id] ASC);

