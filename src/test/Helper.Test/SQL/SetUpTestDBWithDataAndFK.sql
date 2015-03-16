CREATE TABLE TestDataDelete1
(
	TestDataParentId			INT NOT NULL IDENTITY(1,1),
	TestParentString			NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestParentInt				INT NOT NULL,
	TestParentGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

CREATE TABLE TestDataDelete2
(
	TestDataChildId				INT NOT NULL IDENTITY(1,1),
	TestDataParentId			INT NOT NULL DEFAULT 1,
	TestChildString				NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestChildInt				INT NOT NULL,
	TestChildGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO


CREATE TABLE TestDataDelete3
(
	TestDataChildId				INT NOT NULL IDENTITY(1,1),
	TestChildString				NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestChildInt				INT NOT NULL,
	TestChildGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO



ALTER TABLE dbo.TestDataDelete1 ADD CONSTRAINT PK_TestDataDelete1 PRIMARY KEY NONCLUSTERED (TestDataParentId)
GO

ALTER TABLE dbo.TestDataDelete2 ADD CONSTRAINT FK_TestDataDelete2_TestDataDelete1 FOREIGN KEY (TestDataParentId) REFERENCES dbo.TestDataDelete1 (TestDataParentId)
GO


INSERT INTO TestDataDelete1 (TestParentString, TestParentInt)
VALUES ('parentblah', 1)

DECLARE @Id INT
SET @Id=SCOPE_IDENTITY()

INSERT INTO TestDataDelete2 (TestDataParentId, TestChildString, TestChildInt)
VALUES (@Id, 'childblah', 2)

INSERT INTO TestDataDelete3 (TestChildString, TestChildInt)
VALUES ('child2blah', 3)


