DROP TABLE dbo.Locations;
CREATE TABLE dbo.Locations(
	id uniqueidentifier
		DEFAULT newid() PRIMARY KEY,
	title varchar(140) NOT NULL,
	lat float NOT NULL,
	long float NOT NULL,
	descrip varchar(140),
	note varchar(140)
);
