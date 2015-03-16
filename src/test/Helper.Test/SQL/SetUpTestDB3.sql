CREATE TABLE TestDataLoadParent
(
	TestDataLoadParentId		INT NOT NULL IDENTITY(1,1),
	TestParentString			NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestParentInt				INT NOT NULL,
	TestParentGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

CREATE TABLE TestDataLoadChild
(
	TestDataLoadChildId			INT NOT NULL IDENTITY(1,1),
	TestDataLoadParentId		INT NOT NULL DEFAULT 1,
	TestChildString				NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	TestChildInt				INT NOT NULL,
	TestChildGuid				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

CREATE PROCEDURE dbo.TestDataLoadParent_Insert
	@TestParentString		NVARCHAR(100),
	@TestParentInt			INT,
	@TestParentGuid			UNIQUEIDENTIFIER,
	@TestDataLoadParentId	INT OUTPUT
AS
	BEGIN
		SET NOCOUNT ON
	
		INSERT INTO dbo.TestDataLoadParent(TestParentString, TestParentInt, TestParentGuid)
		VALUES (@TestParentString, @TestParentInt, @TestParentGuid)
		
		SET @TestDataLoadParentId=SCOPE_IDENTITY()
	
	END
GO

CREATE PROCEDURE dbo.TestDataLoadChild_Insert
	@TestDataLoadParentId	INT,
	@TestChildString		NVARCHAR(100),
	@TestChildInt			INT,
	@TestChildGuid			UNIQUEIDENTIFIER,
	@TestDataLoadChildId	INT OUTPUT
AS
	BEGIN
		SET NOCOUNT ON
	
		INSERT INTO dbo.TestDataLoadChild(TestDataLoadParentId, TestChildString, TestChildInt, TestChildGuid)
		VALUES (@TestDataLoadParentId, @TestChildString, @TestChildInt, @TestChildGuid)
		
		SET @TestDataLoadChildId=SCOPE_IDENTITY()
	
	END
GO

ALTER TABLE dbo.TestDataLoadParent ADD CONSTRAINT PK_TestDataLoadParent PRIMARY KEY NONCLUSTERED (TestDataLoadParentId)
GO

ALTER TABLE dbo.TestDataLoadChild ADD CONSTRAINT FK_TestDataLoadParent_TestDataLoadParentId FOREIGN KEY (TestDataLoadParentId) REFERENCES dbo.TestDataLoadParent (TestDataLoadParentId)
GO



