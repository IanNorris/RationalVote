CREATE TABLE [dbo].[Sessions] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [Token]        VARCHAR (64)  NULL,
    [IP]           VARCHAR (254) NULL,
    [KeepLoggedIn] BIT           NOT NULL,
    [LastSeen]     DATETIME      NOT NULL,
    [Life]         INT           NOT NULL,
    [User]         BIGINT        NULL,
    CONSTRAINT [PK_dbo.Sessions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Session_User] FOREIGN KEY ([User]) REFERENCES [dbo].[Users] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[Sessions]([User] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Session_Token]
    ON [dbo].[Sessions]([Id] ASC, [Token] ASC);

