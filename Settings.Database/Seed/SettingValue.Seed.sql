SET IDENTITY_INSERT [dbo].[SettingValue] ON

INSERT INTO [dbo].[SettingValue]([Id], [SettingFk], [ValidFrom], [ValidTo], [Value])
VALUES(1, 1, '2022/01/01', NULL, 50),
	  (2, 2, '2021/01/01', NULL, 100)

SET IDENTITY_INSERT [dbo].[SettingValue] OFF