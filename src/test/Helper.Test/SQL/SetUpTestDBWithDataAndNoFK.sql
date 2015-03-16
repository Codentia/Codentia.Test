CREATE TABLE TestDataNotParent
(
	TestDataLoadParentId		INT NOT NULL IDENTITY(1,1),
	TestParentString			NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestParentInt				INT NOT NULL,
	TestParentGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

CREATE TABLE TestDataNotChild
(
	TestDataLoadChildId			INT NOT NULL IDENTITY(1,1),	
	TestChildString				NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestChildInt				INT NOT NULL,
	TestChildGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO


CREATE TABLE TestDataNotChild2
(
	TestDataLoadChildId			INT NOT NULL IDENTITY(1,1),	
	TestChildString				NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestChildInt				INT NOT NULL,
	TestChildGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

INSERT INTO TestDataNotParent (TestParentString, TestParentInt)
VALUES ('blahnoparent', 1)

INSERT INTO TestDataNotChild (TestChildString, TestChildInt)
VALUES ('blahnochild', 2)

INSERT INTO TestDataNotChild2 (TestChildString, TestChildInt)
VALUES ('blahnochild2', 3)


