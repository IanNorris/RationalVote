CREATE TABLE [dbo].[EmailVerificationTokens] (
    [Id]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [Created] DATETIME      NOT NULL,
    [Token]   NVARCHAR (64) NOT NULL,
    [User]    BIGINT        NOT NULL,
    CONSTRAINT [PK_EmailVerificationTokens_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailVerificationTokens_User] FOREIGN KEY ([User]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Token]
    ON [dbo].[EmailVerificationTokens]([Token] ASC);

