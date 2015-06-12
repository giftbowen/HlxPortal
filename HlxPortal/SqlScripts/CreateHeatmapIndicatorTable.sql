CREATE TABLE [dbo].[HeatmapIndicator]
(
    [Id] INT PRIMARY KEY NOT NULL, 
	[PropertyName] NVARCHAR(MAX) NOT NULL,
	[Name] NVARCHAR(MAX) NOT NULL,
	[Threshold] NVARCHAR(MAX) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Green] NVARCHAR(MAX) NOT NULL,
	[Red] NVARCHAR(MAX) NOT NULL
)

insert into HeatmapIndicator values(1, 'Dose1', N'剂量探头1', '50', N'最近24小时是否超过 50 u/h', N'均未超过50 u/h', N'至少一次超过50 u/h')
insert into HeatmapIndicator values(2, 'Flame', N'火焰', '0', N'最近24小时是否发生过火焰', N'从未发生', N'至少发生一次')
insert into HeatmapIndicator values(3, 'Dose2', N'剂量探头2', '50', N'最近24小时是否超过 50 u/h', N'均未超过50 u/h', N'至少一次超过50 u/h')