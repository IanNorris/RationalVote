CREATE TABLE [dbo].[Tags] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Tags] PRIMARY KEY CLUSTERED ([Id] ASC)
);

