using System;
using System.IO;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{
    /// <summary>
    /// Testing fixture for FileHelper class. This intentionally includes positive tests only, as this class is
    /// part of the unit testing framework. Any failures arising due to exceptions/etc are therefore desirable as
    /// factors causing test failure.
    /// </summary>
    [TestFixture]
    public class FileHelperTest
    {
        /// <summary>
        /// Scenario: Create a set of new text files, with varying parameters.
        /// Expected: Successful file creation. Contents matches input.
        /// </summary>
        [Test]
        public void _001_CreateTextFile_DoesNotExist()
        {
            if (Directory.Exists("FileHelperData"))
            {
                Directory.Delete("FileHelperData", true);
            }

            Directory.CreateDirectory("FileHelperData");

            FileHelper.CreateTextFile("FileHelperData/testfile1.txt", string.Empty);
            Assert.That(File.Exists("FileHelperData/testfile1.txt"), Is.True, "File was not created");

            FileHelper.CreateTextFile("FileHelperData/testfile2.txt", "This is the content for the second test file.");
            Assert.That(File.Exists("FileHelperData/testfile2.txt"), Is.True, "File was not created");
        }

        /// <summary>
        /// Scenario: Attempt to create a set of text files which already exist.
        /// Expected: Exception(FileHelper: File 'x' already exists)
        /// </summary>
        [Test]
        public void _002_CreateTextFile_DoesExist()
        {
            // if output files from first test do not exist, run the first test now to create them
            if (!Directory.Exists("FileHelperData") || !File.Exists("FileHelperData/testfile1.txt") || !File.Exists("FileHeleprData/testfile2.txt"))
            {
                _001_CreateTextFile_DoesNotExist();
            }

            Assert.That(Directory.Exists("FileHelperData"), Is.True, "Could not find test data folder");
            Assert.That(File.Exists("FileHelperData/testfile1.txt"), Is.True, "Could not find test data file (1)");
            Assert.That(File.Exists("FileHelperData/testfile2.txt"), Is.True, "Could not find test data file (2)");

            Assert.That(delegate { FileHelper.CreateTextFile("FileHelperData/testfile1.txt", string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("File 'FileHelperData/testfile1.txt' already exists"));
            Assert.That(delegate { FileHelper.CreateTextFile("FileHelperData/testfile2.txt", string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("File 'FileHelperData/testfile2.txt' already exists"));
        }

        /// <summary>
        /// Scenario: ReadTextFile used to read files with known contents
        /// Expected: Value read back will exactly match value put in by previous tests
        /// </summary>
        [Test]
        public void _003_ReadTextFile()
        {
            // if output files from first test do not exist, run the first test now to create them
            if (!Directory.Exists("FileHelperData") || !File.Exists("FileHelperData/testfile1.txt") || !File.Exists("FileHeleprData/testfile2.txt"))
            {
                _001_CreateTextFile_DoesNotExist();
            }

            Assert.That(FileHelper.ReadTextFile("FileHelperData/testfile1.txt"), Is.EqualTo(string.Empty), "Contents differ for testfile1.txt");
            Assert.That(FileHelper.ReadTextFile("FileHelperData/testfile2.txt"), Is.EqualTo("This is the content for the second test file."), "Contents differ for testfile2.txt");
        }

        /// <summary>
        /// Scenario: CompareFiles called on mismatching files
        /// Expected: false
        /// </summary>
        [Test]
        public void _004_CompareFiles_Different()
        {
            Directory.CreateDirectory("FileHelperData/Test004");

            FileHelper.CreateTextFile("FileHelperData/Test004/1.txt", "This is file one");
            FileHelper.CreateTextFile("FileHelperData/Test004/2.txt", "This is file two");

            Assert.That(FileHelper.CompareFiles("FileHelperData/Test004/1.txt", "FileHelperData/Test004/2.txt"), Is.False);
        }

        /// <summary>
        /// Scenario: CompareFiles called where one file does not exist
        /// Expected: false
        /// </summary>
        [Test]
        public void _005_CompareFiles_OneDoesNotExist()
        {
            Directory.CreateDirectory("FileHelperData/Test005");
            FileHelper.CreateTextFile("FileHelperData/Test005/1.txt", "This is file one");
            Assert.That(FileHelper.CompareFiles("FileHelperData/Test005/1.txt", "FileHelperData/Test005/2.txt"), Is.False);
        }

        /// <summary>
        /// Scenario: CompareFiles called on matching files
        /// Expected: true
        /// </summary>
        [Test]
        public void _006_CompareFiles_Same()
        {
            Directory.CreateDirectory("FileHelperData/Test006");
            FileHelper.CreateTextFile("FileHelperData/Test006/1.txt", "This is file one");
            Assert.That(FileHelper.CompareFiles("FileHelperData/Test006/1.txt", "FileHelperData/Test006/1.txt"), Is.True);
        }

        /// <summary>
        /// Scenario: CompareDirectories called on empty folders
        /// Expected: true
        /// </summary>
        [Test]
        public void _007_CompareDirectories_BothEmpty()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test007";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", false), Is.True, "Expected true");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders with different file contents
        /// Expected: false
        /// </summary>
        [Test]
        public void _008_CompareDirectories_ContentsDiffer()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test008";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            FileHelper.CreateTextFile(baseDirectory + "/A/1.txt", "This is file one in folder A");
            FileHelper.CreateTextFile(baseDirectory + "/B/2.txt", "This is file two in folder B");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", false), Is.False, "Expected false");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders with matching file contents (differences inside files)
        /// Expected: false
        /// </summary>
        [Test]
        public void _009_CompareDirectories_SameContents_FilesDiffer()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test009";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            FileHelper.CreateTextFile(baseDirectory + "/A/1.txt", "This is file one in folder A");
            FileHelper.CreateTextFile(baseDirectory + "/B/1.txt", "This is file one in folder B");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", false), Is.False, "Expected false");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders with matching file contents
        /// Expected: true
        /// </summary>
        [Test]
        public void _010_CompareDirectories_FileContentsMatch()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test010";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            FileHelper.CreateTextFile(baseDirectory + "/A/1.txt", "This is file one in folder");
            FileHelper.CreateTextFile(baseDirectory + "/B/1.txt", "This is file one in folder");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", false), Is.True, "Expected true");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders with mismatching child directories
        /// Expected: false
        /// </summary>
        [Test]
        public void _011_CompareDirectories_ChildDirectoriesMismatch()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test011";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            Directory.CreateDirectory(baseDirectory + "/A/Child1");
            Directory.CreateDirectory(baseDirectory + "/B/Child2");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", true), Is.False, "Expected false");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders (only lhs exists)
        /// Expected: false
        /// </summary>
        [Test]
        public void _012_CompareDirectories_OnlyLeftExists()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test012";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", true), Is.False, "Expected false");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders (only rhs exists)
        /// Expected: false
        /// </summary>
        [Test]
        public void _013_CompareDirectories_OnlyRightExists()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test013";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/B");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", true), Is.False, "Expected false");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders where child files match
        /// Expected: true
        /// </summary>
        [Test]
        public void _014_CompareDirectories_ChildFilesMatch()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test014";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            Directory.CreateDirectory(baseDirectory + "/A/Child1");
            Directory.CreateDirectory(baseDirectory + "/B/Child1");

            FileHelper.CreateTextFile(baseDirectory + "/A/Child1/1.txt", "This is file one");
            FileHelper.CreateTextFile(baseDirectory + "/B/Child1/1.txt", "This is file one");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", true), Is.True, "Expected true");
        }

        /// <summary>
        /// Scenario: CompareDirectories called on folders where child files mismatch
        /// Expected: false
        /// </summary>
        [Test]
        public void _015_CompareDirectories_ChildFilesMismatch()
        {
            // create test data
            string baseDirectory = "FileHelperData/Test015";
            Directory.CreateDirectory(baseDirectory);
            Directory.CreateDirectory(baseDirectory + "/A");
            Directory.CreateDirectory(baseDirectory + "/B");

            Directory.CreateDirectory(baseDirectory + "/A/Child1");
            Directory.CreateDirectory(baseDirectory + "/B/Child1");

            FileHelper.CreateTextFile(baseDirectory + "/A/Child1/1.txt", "This is file one");
            FileHelper.CreateTextFile(baseDirectory + "/B/Child1/1.txt", "This is file one but different");

            // perform test
            Assert.That(FileHelper.CompareDirectories(baseDirectory + "/A", baseDirectory + "/B", true), Is.False, "Expected true");
        }

         /// <summary>
        /// Scenario: Create a file in C:\Windows\Temp and use DoesWindowsTempFileExist to check existence
        /// Expected: File gets created
        /// </summary>
        [Test]
        public void _016_CreateWindowsTempFile()
        {
            Guid g = Guid.NewGuid();           
            string fileName = string.Format("{0}.txt", g.ToString());
            string fullFileName = string.Format(@"C:\Windows\Temp\{0}.chk", fileName);

            Assert.That(FileHelper.DoesWindowsTempFileExist(fileName, g.ToString()), Is.False);
            
            FileHelper.CreateWindowsTempFile(fileName, g.ToString());            

            Assert.That(FileHelper.DoesWindowsTempFileExist(fileName, g.ToString()), Is.True);

            Assert.That(FileHelper.ReadTextFile(fullFileName) == g.ToString(), Is.True);
           
            // Create same file with new Guid
            Guid g2 = Guid.NewGuid();
            FileHelper.CreateWindowsTempFile(fileName, g2.ToString());
            
            Assert.That(FileHelper.ReadTextFile(fullFileName) == g2.ToString(), Is.True);
            Assert.That(FileHelper.DoesWindowsTempFileExist(fileName, g2.ToString()), Is.True);
            Assert.That(FileHelper.DoesWindowsTempFileExist(fileName, g.ToString()), Is.False);
        }
    }
}
