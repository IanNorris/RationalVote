CREATE TABLE [dbo].[Users] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Email]                NVARCHAR (254) NOT NULL,
    [PasswordSalt]         VARBINARY (64) NULL,
    [PasswordHash]         VARBINARY (64) NULL,
    [AuthenticationMethod] TINYINT        NOT NULL,
    [Verified]             BIT            NOT NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [EmailUnique]
    ON [dbo].[Users]([Email] ASC);

