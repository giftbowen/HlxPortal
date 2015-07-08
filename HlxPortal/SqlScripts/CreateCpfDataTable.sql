CREATE TABLE [dbo].[CpfData]
(
	[Date] DATETIME NOT NULL,
	[SN] NVARCHAR(128) NOT NULL, 
    [SiteId] INT NOT NULL,
	[DeviceId] INT NOT NULL,
	[PlateNumber] NVARCHAR(128) NULL,
	[VehicleType] NVARCHAR(128) NULL,
	[Comments] NVARCHAR(MAX) NULL,
	[Goods] NVARCHAR(MAX) NULL,
)
GO
Create Clustered Index Inx_Date on [dbo].[CpfData] (Date)
GO
