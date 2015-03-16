using System;
using System.Data;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{
    /// <summary>
    /// This class is the unit testing fixture for the static class ExcelHelper
    /// </summary>
    [TestFixture]
    public class ExcelHelperTest
    {
        /// <summary>
        /// Scenario: XLS worksheet loaded, no header rows returned
        /// Expected: Data matching known contents of sheets
        /// </summary>
        [Test]
        public void _001_LoadXLSSheetToDataTable_NoHeader()
        {
            DataTable dt1 = ExcelHelper.LoadXLSSheetToDataTable(@"Helper.Test\TestData\TestXLS1.xls", "TestSheet1_1", false);
            Assert.That(dt1, Is.Not.Null, "DataTable was null");
            Assert.That(dt1.Rows.Count, Is.EqualTo(7), "Expected 7 rows");

            Assert.That(Convert.ToString(dt1.Rows[0][0]), Is.EqualTo("Col1"), "Row 0, column 0 mismatch");
            Assert.That(Convert.ToString(dt1.Rows[0][1]), Is.EqualTo("Col2"), "Row 0, column 1 mismatch");
            Assert.That(Convert.ToString(dt1.Rows[0][2]), Is.EqualTo("Col3"), "Row 0, column 2 mismatch");

            int a = 1;
            int b = 2;
            int c = 3;

            for (int i = 1; i < dt1.Rows.Count; i++)
            {
                Assert.That(Convert.ToInt32(dt1.Rows[i][0]), Is.EqualTo(a), "column 0 mismatch");
                Assert.That(Convert.ToInt32(dt1.Rows[i][1]), Is.EqualTo(b), "column 1 mismatch");
                Assert.That(Convert.ToInt32(dt1.Rows[i][2]), Is.EqualTo(c), "column 2 mismatch");

                a = a * 2;
                b = b * 2;
                c = c * 2;
            }
        }

        /// <summary>
        /// Scenario: XLS worksheet loaded, header rows returned
        /// Expected: Data matching known contents of sheets
        /// </summary>
        [Test]
        public void _002_LoadXLSSheetToDataTable_Header()
        {
            DataTable dt1 = ExcelHelper.LoadXLSSheetToDataTable(@"Helper.Test\TestData\TestXLS1.xls", "TestSheet1_1", true);
            Assert.That(dt1, Is.Not.Null, "DataTable was null");
            Assert.That(dt1.Rows.Count, Is.EqualTo(6), "Expected 6 rows");
            
            Assert.That(Convert.ToString(dt1.Columns[0].ColumnName), Is.EqualTo("Col1"), "column 0 mismatch");
            Assert.That(Convert.ToString(dt1.Columns[1].ColumnName), Is.EqualTo("Col2"), "column 1 mismatch");
            Assert.That(Convert.ToString(dt1.Columns[2].ColumnName), Is.EqualTo("Col3"), "column 2 mismatch");

            int a = 1;
            int b = 2;
            int c = 3;

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                Assert.That(Convert.ToInt32(dt1.Rows[i][0]), Is.EqualTo(a), "column 0 mismatch");
                Assert.That(Convert.ToInt32(dt1.Rows[i][1]), Is.EqualTo(b), "column 1 mismatch");
                Assert.That(Convert.ToInt32(dt1.Rows[i][2]), Is.EqualTo(c), "column 2 mismatch");

                a = a * 2;
                b = b * 2;
                c = c * 2;
            }
        }
    }
}
