CREATE TABLE [dbo].[Permissions] (
    [Id]      BIGINT   IDENTITY (1, 1) NOT NULL,
    [Type]    SMALLINT NOT NULL,
    [Value]   SMALLINT NULL,
    [User_Id] BIGINT   NULL,
    CONSTRAINT [PK_dbo.Permissions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Permissions_dbo.Users_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[Users] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[Permissions]([User_Id] ASC);

