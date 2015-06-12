CREATE TABLE [dbo].[CpfData]
(
	[Date] DATETIME NOT NULL,
	[SN] NVARCHAR(128) NOT NULL, 
    [SiteId] INT NOT NULL,
	[DeviceId] INT NOT NULL,
	[PlateNumber] NVARCHAR(128) NOT NULL,
	[VehicleType] NVARCHAR(128) NOT NULL,
	[Comments] NVARCHAR(MAX) NOT NULL,
	[Goods] NVARCHAR(MAX) NOT NULL,
)
GO
Create Clustered Index Inx_Date on [dbo].[CpfData] (Date)
GO
