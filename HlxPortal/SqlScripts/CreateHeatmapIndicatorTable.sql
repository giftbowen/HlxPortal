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

insert into HeatmapIndicator values(1, 'Flame', N'火焰', '0', N'最近24小时是否发生过火焰', N'从未发生', N'至少发生一次')
insert into HeatmapIndicator values(2, 'Dose1', N'剂量探头1', '50', N'最近24小时是否超过 50 uSv/h', N'均未超过50 uSv/h ', N'至少一次超过50 uSv/h ')
insert into HeatmapIndicator values(3, 'Dose2', N'剂量探头2', '50', N'最近24小时是否超过 50 uSv/h ', N'均未超过50 uSv/h ', N'至少一次超过50 uSv/h ')
insert into HeatmapIndicator values(4, 'Dose3', N'剂量探头3', '50', N'最近24小时是否超过 50 uSv/h ', N'均未超过50 uSv/h ', N'至少一次超过50 uSv/h ')
insert into HeatmapIndicator values(5, 'Dose4', N'剂量探头4', '50', N'最近24小时是否超过 50 uSv/h ', N'均未超过50 uSv/h ', N'至少一次超过50 uSv/h ')
insert into HeatmapIndicator values(6, 'Dose5', N'剂量探头5', '50', N'最近24小时是否超过 50 uSv/h ', N'均未超过50 uSv/h ', N'至少一次超过50 uSv/h ')
