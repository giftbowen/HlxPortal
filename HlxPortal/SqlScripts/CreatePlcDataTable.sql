﻿CREATE TABLE [dbo].[PlcData]
(
    [SiteId] TINYINT PRIMARY KEY NOT NULL,
    [TimeStamp] DATETIME NOT NULL, 
    [KeyValueData] NVARCHAR(MAX) NOT NULL,
)
GO