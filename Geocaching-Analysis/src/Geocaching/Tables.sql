DROP TABLE IF EXISTS Logs;
DROP TABLE IF EXISTS Geocaches;
DROP TABLE IF EXISTS PocketQueries;
      
CREATE TABLE Geocaches (
	Name				NVARCHAR	(MAX)	NULL,
	CacheID				VARCHAR		(10)	NULL,
	Country				VARCHAR		(MAX)	NULL,
	[Description]		NVARCHAR	(MAX)	NULL,
	Difficulty			REAL				NOT NULL,
	LongDescription		NVARCHAR	(MAX)	NULL,
	[Owner]				NVARCHAR	(MAX)	NULL,
	ShortDescription	NVARCHAR	(MAX)	NULL,
	Size				NVARCHAR	(MAX)	NULL,
	[State]				NVARCHAR	(MAX)	NULL,
	StatusArchived		BIT					NOT NULL,
	StatusAvailable		BIT					NOT NULL,
	Symbol				NVARCHAR	(MAX)	NULL,
	SymbolType			NVARCHAR	(MAX)	NULL,
	Terrain				REAL				NOT NULL,
	[Time]				NVARCHAR	(MAX)	NULL,
	[Type]				NVARCHAR	(MAX)	NULL,
	[URL]				NVARCHAR	(MAX)	NULL,
	URLName				NVARCHAR	(MAX)	NULL,
	Latitude			FLOAT (53)		    NOT NULL,
	Longitude			FLOAT (53)		    NOT NULL,
	Altitude			FLOAT (53)		    NOT NULL,
	LastChanged			DATETIME			DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
	GeocacheID			NVARCHAR (128)		DEFAULT ('') NOT NULL,
	CONSTRAINT [PK_dbo.Geocaches] PRIMARY KEY (GeocacheID ASC),
);

CREATE TABLE Logs (
	ID	BIGINT NOT NULL,
	GeocacheID NVARCHAR (128) NULL,
	Date DATETIME not null,
	type nvarchar (max) null,
	author nvarchar (max) null,
	text nvarchar (max) null,
	textencoded bit not null,
	lastchanged datetime DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
	constraint [PK_dbo.Logs] PRIMARY KEY (ID ASC),
	constraint [fk_dbo.Logs] FOREIGN KEY (GeocacheID) REFERENCES Geocaches (GeocacheID)
);

CREATE TABLE PocketQueries (
	Name nvarchar (128) not null,
	DateGenerated DATETIME not null,
	EntryCount SMALLINT not null,
	FileSize nvarchar (max) not null,
	Url nvarchar (max) not null,
	constraint [pk_dbo.PocketQueries] PRIMARY KEY (Name ASC)
);