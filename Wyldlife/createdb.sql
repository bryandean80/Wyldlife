CREATE TABLE dbo.Locations(
	id uniqueidentifier
		DEFAULT newid() PRIMARY KEY,
	title varchar(140) NOT NULL,
	lat numeric NOT NULL,
	long numeric NOT NULL,
	descrip varchar(140),
	note varchar(140)
);
