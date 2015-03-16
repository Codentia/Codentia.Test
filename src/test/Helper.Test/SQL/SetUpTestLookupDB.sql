CREATE TABLE TestLookup
(
	TestLookupId		INT NOT NULL,
	TestLookupCode		NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
)
GO

CREATE TABLE TestLookupEmpty
(
	TestLookupEmptyId			INT NOT NULL,
	TestLookupEmptyCode			NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
)
GO

INSERT INTO TestLookup (TestLookupId, TestLookupCode)
SELECT 1, 'LookupVal 1'
UNION
SELECT 2, 'LookupVal 2'

CREATE TABLE MyTable
(
	MyTableId				INT NOT NULL,
	MyTableString			NVARCHAR(100) NOT NULL DEFAULT 'ABCDEFGH',
	MyTableYesNo			BIT
)

INSERT INTO MyTable (MyTableId, MyTableString, MyTableYesNo)
SELECT 1, 'I am Spartacus', 1
UNION
SELECT 2, 'I most definitely am not Spartacus', 0