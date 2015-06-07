CREATE TABLE [dbo].[CpfData]
(
	[Date] DATETIME NOT NULL,
	[SN] NCHAR(128) NOT NULL, 
        [SiteId] INT NOT NULL,
	[DeviceId] INT NOT NULL,
	[PlateNumber] NCHAR(32) NOT NULL,
	[VehicleType] NCHAR(32) NOT NULL,
	[Comments] NVARCHAR(MAX) NOT NULL,
	[Goods] NCHAR(128) NOT NULL,
)
GO
Create Clustered Index Inx_Date on [dbo].[CpfData] (Date)
GO
