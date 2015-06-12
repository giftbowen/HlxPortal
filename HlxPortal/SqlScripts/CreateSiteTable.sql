CREATE TABLE [dbo].[Site]
(
    [SiteId] TINYINT PRIMARY KEY NOT NULL, 
	[Name] NVARCHAR(128) NOT NULL,
	[Location] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Longitude] FLOAT NOT NULL,
	[Latitude] FLOAT NOT NULL,
)

insert into site values(1, 'Gaohua1', 'Liaoning', 'Liaoning Gaohua shoufeizhan', '123.376579','42.132367')
insert into site values(2, 'Changxinan2', 'Jiangxi', 'Jiangxi Changxinan shoufeizhan', '116.459195','28.668137')
insert into site values(3, 'Qingyuan3', 'Guangdong', 'Guangdong qingyuan shoufeizhan', '113.292323','23.685397')
insert into site values(4, 'Hangzhou4', 'Zhejiang', 'Zhejiang hangzhou shoufeizhan', '120.396217','30.18211')