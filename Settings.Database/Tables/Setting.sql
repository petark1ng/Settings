CREATE TABLE [dbo].[Setting]
(
	[Id]                                 INT                     IDENTITY (1, 1) NOT NULL,
    [Name]                               NVARCHAR (255)          NOT NULL,    
    CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED ([Id] ASC),
)