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
	id uniqueidentifier
		DEFAULT newid() NOT NULL,
	locationId uniqueidentifier  NOT NULL,
	author VARCHAR(140) NOT NULL,
	rating TINYINT NOT NULL,
	reviewText VARCHAR(140)
	PRIMARY KEY (id, locationId),
	FOREIGN KEY (locationId)
		REFERENCES dbo.Locations(id)
);