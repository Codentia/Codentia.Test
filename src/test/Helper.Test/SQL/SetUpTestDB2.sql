CREATE TABLE TestTable1
(
	TestTable1Id	INT IDENTITY(1,1),
	TestInt			INT NOT NULL DEFAULT 1,
	TestString		NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime	DATETIME NOT NULL DEFAULT GETDATE(),
	TestDecimal		DECIMAL(20,10) NOT NULL,
	TestBit			BIT NOT NULL,
	TestGuid		UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

INSERT INTO dbo.TestTable1
(
	TestInt,
	TestString,
	TestDateTime,
	TestDecimal,
	TestBit
)
VALUES
(
	3,
	'ABCDEFGHIJ',
	'2007-06-01',
	17.5,
	1
)
GO

CREATE TABLE TestTable2
(
	TestTable2Id	INT IDENTITY(1,1),
	TestInt			INT NOT NULL DEFAULT 1,
	TestString		NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime	DATETIME NOT NULL DEFAULT GETDATE()
)
GO

CREATE TABLE TestTable3
(
	TestTable3Id	INT IDENTITY(1,1),
	TestInt			INT NOT NULL DEFAULT 1,
	TestString		NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime	DATETIME NOT NULL DEFAULT GETDATE()
)
GO

CREATE TABLE Test_Table4
(
	TestTable4Id	INT IDENTITY(1,1),
	TestInt			INT NOT NULL DEFAULT 1,
	TestString		NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime	DATETIME NOT NULL DEFAULT GETDATE(),
	ExtraIntColumn	INT
)
GO

CREATE TABLE Test_TableDifferent
(
	TableDifferentId	INT IDENTITY(1,1),
	TestInt				INT NOT NULL DEFAULT 1,
	TestString			NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime		DATETIME NOT NULL DEFAULT GETDATE(),
	ExtraIntColumn		INT,
	DifferentColumn		INT
)
GO

CREATE TABLE Test_TableDifferentNoStringCompare
(
	TestTableDifferentNoStringCompareId		INT IDENTITY(1,1),
	TestInt									INT NOT NULL DEFAULT 1,
	TestString								NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime							DATETIME NOT NULL DEFAULT GETDATE(),
	ExtraIntColumn							INT
)
GO

CREATE TABLE Test_TableSameNoStringCompare
(
	TestTableSameNoStringCompareId	INT IDENTITY(1,1),
	TestInt							INT NOT NULL DEFAULT 1,
	TestString						NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime					DATETIME NOT NULL DEFAULT GETDATE()
)
GO

CREATE TABLE TestTableNoIdent
(
	TestTableNoIdentId			INT NOT NULL DEFAULT 1,
	TestString					NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime				DATETIME NOT NULL DEFAULT GETDATE(),
	TestDecimal					DECIMAL(20,10) NOT NULL,
	TestBit						BIT NOT NULL,
	TestGuid					UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

INSERT INTO dbo.TestTable3
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	3,
	'ABCDEFGHIJ',
	'2007-06-01'
)
GO

INSERT INTO dbo.TestTable3
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	6,
	'JIHGFEDCBA',
	'2007-06-02'
)
GO

INSERT INTO dbo.Test_Table4
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	199999999,
	'BLAHBLER',
	'2015-07-01'
)
GO

INSERT INTO dbo.Test_Table4
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	2000,
	'BLERGGGHH',
	'2005-07-01'
)
GO

INSERT INTO dbo.Test_TableDifferent
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	2000,
	'BLERGGGHH',
	'2005-07-01'
)
GO

INSERT INTO dbo.Test_TableDifferent
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	2001,
	'BLERGGGH2',
	'2005-07-01'
)
GO


INSERT INTO dbo.Test_TableDifferentNoStringCompare
(
	TestInt,
	TestString,
	TestDateTime,
	ExtraIntColumn
)
VALUES
(
	2000,
	'DIFFERENT',
	'2005-07-01',
	8
)
GO

INSERT INTO dbo.Test_TableSameNoStringCompare
(
	TestInt,
	TestString,
	TestDateTime
)
VALUES
(
	2000,
	'DIFFERENT',
	'2005-07-01'
)
GO



CREATE TABLE NewTableForThisDB
(
	NewTableForThisDBId			INT NOT NULL DEFAULT 1,
	TestString					NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime				DATETIME NOT NULL DEFAULT GETDATE(),
	TestDecimal					DECIMAL(20,10) NOT NULL,
	TestBit						BIT NOT NULL,
	TestGuid					UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO

CREATE TABLE NewTableForThisDB2
(
	NewTableForThisDBId			INT NOT NULL DEFAULT 1,
	TestString					NVARCHAR(10) NOT NULL DEFAULT 'ABCDEFGH',
	TestDateTime				DATETIME NOT NULL DEFAULT GETDATE(),
	TestDecimal					DECIMAL(20,10) NOT NULL,
	TestBit						BIT NOT NULL,
	TestGuid					UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
)
GO



CREATE PROCEDURE dbo.TestProc1
AS
	BEGIN
		SET NOCOUNT ON
	END
GO

CREATE PROCEDURE dbo.TestProc2
	@param1		INT
AS
	BEGIN
		SET NOCOUNT ON
	END
GO

CREATE PROCEDURE dbo.TestProc3
	@param1		INT OUTPUT
AS
	BEGIN
		SET NOCOUNT ON

		SET @param1 = 2
	END
GO

CREATE PROCEDURE dbo.TestProc4
AS
	BEGIN
		SET NOCOUNT ON

		SELECT 1, 2, 3, 4
	END
GO

CREATE PROCEDURE dbo.TestProc5
	@param1		INT
AS
	BEGIN
		SET NOCOUNT ON

		SELECT @param1, 2, 3, 4
	END
GO

CREATE PROCEDURE dbo.TestProc6
	@param1		INT OUTPUT
AS
	BEGIN
		SET NOCOUNT ON

		SET @param1 = 2

		SELECT @param1, 2, 3, 4
	END
GO


