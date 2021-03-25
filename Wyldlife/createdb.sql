CREATE TABLE dbo.Locations(
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
	uploaded DATETIME
		DEFAULT CURRENT_TIMESTAMP NOT NULL,
	PRIMARY KEY (locationId, imageId),
	FOREIGN KEY (locationId)
		REFERENCES dbo.Locations(id)
);