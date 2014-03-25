CREATE TABLE [dbo].[Profiles] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [DisplayName] NVARCHAR (MAX) NULL,
    [RealName]    NVARCHAR (MAX) NULL,
    [Occupation]  NVARCHAR (MAX) NULL,
    [Location]    NVARCHAR (MAX) NULL,
    [Joined]      DATETIME       NOT NULL,
    [Experience]  BIGINT         NOT NULL,
    [User]        BIGINT         NOT NULL,
    CONSTRAINT [PK_Profile_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Profile_User] FOREIGN KEY ([User]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Id]
    ON [dbo].[Profiles]([Id] ASC);

