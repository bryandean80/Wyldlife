﻿CREATE TABLE dbo.Locations(
	id uniqueidentifier
		DEFAULT newid() NOT NULL,
	title varchar(140) NOT NULL,
	author VARCHAR(140),
	lat float NOT NULL,
	long float NOT NULL,
	descrip varchar(140),
	note varchar(140),
	PRIMARY KEY (id)
);

CREATE TABLE dbo.Reviews(
	locationId uniqueidentifier  NOT NULL,
	author VARCHAR(140) NOT NULL,
	rating TINYINT NOT NULL,
	reviewText VARCHAR(140)
	PRIMARY KEY (author, locationId),
	FOREIGN KEY (locationId)
		REFERENCES dbo.Locations(id)
);

CREATE TABLE dbo.Images(
	locationId uniqueidentifier NOT NULL,
	imageId uniqueidentifier 
		DEFAULT newid() NOT NULL,
	author VARCHAR(140) NOT NULL,
	img VARBINARY(max),
	isStory BIT
		DEFAULT 0 NOT NULL,
	uploaded DATETIME
		DEFAULT CURRENT_TIMESTAMP NOT NULL,
	PRIMARY KEY (locationId, imageId),
	FOREIGN KEY (locationId)
		REFERENCES dbo.Locations(id)
);

CREATE TABLE dbo.Maps(
	locationId uniqueidentifier NOT NULL,
	satellite VARBINARY(max) NOT NULL,
	terrain VARBINARY(max) NOT NULL,
	PRIMARY KEY (locationId),
	FOREIGN KEY (locationId)
		REFERENCES dbo.Locations(id)
);

CREATE TABLE dbo.Weather(
	locationId uniqueidentifier NOT NULL,
	weather VARCHAR(4000) NOT NULL,
	updated DATETIME
		DEFAULT CURRENT_TIMESTAMP NOT NULL,
	FOREIGN KEY (locationId)
		REFERENCES dbo.Locations(id)
);