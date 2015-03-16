using System;
using System.IO;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// This class contains a set of static methods designed to ease file manipulation when writing unit tests.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Pattern - @"C:\Windows\Temp\{0}"
        /// </summary>
        public const string TempFilePattern = @"C:\Windows\Temp\{0}.chk";

        /// <summary>
        /// Create a new text file with the name/path and content specified.
        /// This method will fail if the file already exists.
        /// </summary>
        /// <param name="fileName">Full filename (including path) of the file to be created</param>
        /// <param name="contents">Contents to be placed in the file</param>
        public static void CreateTextFile(string fileName, string contents)
        {
            if (File.Exists(fileName))
            {
                throw new ArgumentException(string.Format("File '{0}' already exists", fileName));
            }
            else
            {
                StreamWriter fileStream = File.CreateText(fileName);
                fileStream.Write(contents);
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Read the data within the specified text file and return it as a single string.
        /// </summary>
        /// <param name="fileName">Full filename (including path) of the file to be read</param>
        /// <returns>string of the text file</returns>
        public static string ReadTextFile(string fileName)
        {
            StreamReader fileStream = File.OpenText(fileName);
            string contents = fileStream.ReadToEnd();

            fileStream.Close();
            fileStream.Dispose();

            return contents;
        }

        /// <summary>
        /// Compare two files (compare the size, and then contents).
        /// </summary>
        /// <param name="filenameA">First file</param>
        /// <param name="filenameB">Second file</param>
        /// <returns>bool - true if files are the same, otherwise false</returns>
        public static bool CompareFiles(string filenameA, string filenameB)
        {
            bool match = true;

            if (File.Exists(filenameA) && File.Exists(filenameB))
            {
                match = ReadTextFile(filenameA) == ReadTextFile(filenameB);
            }
            else
            {
                match = false;
            }

            return match;
        }

        /// <summary>
        /// Compare two directories to see if they contain the same files with the same name and the same contents.
        /// </summary>
        /// <param name="directoryA">First directory</param>
        /// <param name="directoryB">Second directory</param>
        /// <param name="recurse">Recurse through child directories?</param>
        /// <returns>bool - true if directories are the same, otherwise false</returns>
        public static bool CompareDirectories(string directoryA, string directoryB, bool recurse)
        {
            bool match = true;

            if (Directory.Exists(directoryA))
            {
                if (Directory.Exists(directoryB))
                {
                    string[] filesA = Directory.GetFiles(directoryA);
                    string[] filesB = Directory.GetFiles(directoryB);

                    string[] directoriesA = Directory.GetDirectories(directoryA);
                    string[] directoriesB = Directory.GetDirectories(directoryB);

                    // fail immediately if they have different amounts of files OR child-directories
                    match = (filesA.Length == filesB.Length) && (directoriesA.Length == directoriesB.Length);

                    if (match)
                    {
                        for (int i = 0; i < filesA.Length && match == true; i++)
                        {
                            FileInfo fileIA = new FileInfo(filesA[i]);
                            FileInfo fileIB = new FileInfo(filesB[i]);

                            // compare names
                            match = fileIA.Name == fileIB.Name;

                            if (match)
                            {
                                // compare sizes                            
                                match = fileIA.Length == fileIB.Length;

                                if (match)
                                {
                                    match = FileHelper.CompareFiles(filesA[i], filesB[i]);
                                }
                            }
                        }

                        if (match)
                        {
                            for (int i = 0; i < directoriesA.Length && match == true; i++)
                            {
                                // compare names
                                DirectoryInfo directoryIA = new DirectoryInfo(directoriesA[i]);
                                DirectoryInfo directoryIB = new DirectoryInfo(directoriesB[i]);
                                match = directoryIA.Name == directoryIB.Name;

                                if (match && recurse)
                                {
                                    match = FileHelper.CompareDirectories(directoriesA[i], directoriesB[i], true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // only directoryA exists - fail
                    match = false;
                }
            }
            else
            {
                if (Directory.Exists(directoryB))
                {
                    // only directoryB exists - fail
                    match = false;
                }
            }

            return match;
        }

        /// <summary>
        /// Create a file specifically in C:\Windows\Temp
        /// </summary>
        /// <param name="fileName">Just the name of the file (no path or extension)</param>
        /// <param name="contents">the content of the file</param>
        public static void CreateWindowsTempFile(string fileName, string contents)
        {
            string fullFileName = string.Format(TempFilePattern, fileName);
            
            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }

            CreateTextFile(fullFileName, contents);            
        }

        /// <summary>
        /// Check for existence of file and if it exists does it have the same contents as provided
        /// </summary>
        /// <param name="fileName">Just the name of the file (no path or extension)</param>
        /// <param name="contents">the content of the file</param>
        /// <returns>bool - if file exists and contents match</returns>
        public static bool DoesWindowsTempFileExist(string fileName, string contents)
        {
            string fullFileName = string.Format(TempFilePattern, fileName);
            bool retVal = false;
            if (File.Exists(fullFileName))
            {
                string checkFileContents = ReadTextFile(fullFileName);
                if (contents == checkFileContents)
                {
                    retVal = true;
                }
            }

            return retVal;
        }
    }
}
