CREATE TABLE [dbo].[Site]
(
    [SiteId] TINYINT PRIMARY KEY NOT NULL, 
	[Name] NVARCHAR(128) NOT NULL,
	[Location] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[Longitude] FLOAT NOT NULL,
	[Latitude] FLOAT NOT NULL,
)

insert into site values(1, N'高花', N'辽宁', N'辽宁省高花收费站', '123.376579','42.132367')
insert into site values(2, N'昌西南', N'江西', N'江西省昌西南收费站', '116.459195','28.668137')
insert into site values(3, N'清远', N'广东', N'广东省清远收费站', '113.292323','23.685397')
insert into site values(4, N'杭州', N'浙江', N'浙江省杭州收费站', '120.396217','30.18211')