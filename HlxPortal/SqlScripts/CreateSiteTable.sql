CREATE TABLE [dbo].[Site]
(
    [SiteId] TINYINT PRIMARY KEY NOT NULL, 
	[Name] NCHAR(128) NOT NULL,
	[Location] NCHAR(128) NOT NULL,
	[Description] NCHAR(128) NOT NULL,
)
