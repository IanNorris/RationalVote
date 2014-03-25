CREATE TABLE [dbo].[Debates] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [Status]   SMALLINT       DEFAULT ((0)) NOT NULL,
    [Title]    NVARCHAR (250) NOT NULL,
    [Posted]   DATETIME       NOT NULL,
    [Updated]  DATETIME       NULL,
    [Owner_Id] BIGINT         NULL,
    CONSTRAINT [PK_dbo.Debates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Debates_dbo.Users_Owner_Id] FOREIGN KEY ([Owner_Id]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ui_Id]
    ON [dbo].[Debates]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Owner_Id]
    ON [dbo].[Debates]([Owner_Id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [ui_Title]
    ON [dbo].[Debates]([Title] ASC);

