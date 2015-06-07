CREATE TABLE [dbo].[RadiationData]
(
	[Date] DATETIME NOT NULL,
	[TimeStamp] DATETIME NOT NULL, 
    [SiteId] TINYINT NOT NULL, 
	[Flame] TINYINT NOT NULL, 
	[Shutter] TINYINT NOT NULL, 
	[Position] TINYINT NOT NULL, 
	[Gate] TINYINT NOT NULL, 
    [Temperature] REAL NOT NULL, 
	[Humidity] REAL NOT NULL,
    [CameraImage] NCHAR(128) NOT NULL,
	[Dose1] REAL NOT NULL,
	[Dose2] REAL NOT NULL,
	[Dose3] REAL NOT NULL,
	[Dose4] REAL NOT NULL,
	[Dose5] REAL NOT NULL,
)
GO
Create Clustered Index Inx_Date on [dbo].[RadiationData] (Date)
GO
Create NonClustered Index Inx_SiteId on [dbo].[RadiationData] (SiteId)
GO
