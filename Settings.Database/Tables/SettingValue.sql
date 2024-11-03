CREATE TABLE [dbo].[SettingValue]
(
	[Id]                                 INT                     IDENTITY (1, 1) NOT NULL,
    [SettingFk]                          INT                                     NOT NULL,    
    [Value]                              DECIMAL(19,4)                           NOT NULL,
    [ValidFrom]                          DATETIME2                               NOT NULL,
    [ValidTo]                            DATETIME2                               NULL,
    CONSTRAINT [PK_SettingValue] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SettingValue_ToSetting] FOREIGN KEY ([SettingFk]) REFERENCES [dbo].[Setting]([Id]),
)