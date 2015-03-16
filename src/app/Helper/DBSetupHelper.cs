using System;
using System.IO;
using Codentia.Common.Data;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// Helper class for Common DL unit tests
    /// Note: this is included here as its presence in UnitTestHelper (Common Utils libraries) would require
    /// a circular dependency.
    /// </summary>
    public static class DBSetupHelper
    {
        private static object _lockObject = new object();
        
        /// <summary>
        /// Ensure that the testing database exists
        /// Should this database grow significantly in the future, this logic should be replaced with a full database build
        /// </summary>
        /// <param name="masterConnection">Name of master database connection</param>
        /// <param name="databaseConnection">Name of database connection</param>
        /// <param name="dbName">db Name</param>
        /// <param name="setupScriptPath">if null or empty then does create db only</param>
        /// <param name="alwaysRebuild">alwway rebuild</param>
        public static void EnsureTestDatabaseExists(string masterConnection, string databaseConnection, string dbName, string setupScriptPath, bool alwaysRebuild)
        {            
            lock (_lockObject)
            {
                if (DbSystem.DatabaseExists(masterConnection, dbName))
                {
                    // if alwaysRebuild is false, exit
                    if (!alwaysRebuild)
                    {
                        Console.WriteLine("db: {0} already exists - script not run", dbName);
                        return;
                    }

                    // drop database
                    DbSystem.DropDatabase(masterConnection, dbName);
                    Console.Out.WriteLine(string.Format("Drop Database: {0}", dbName));
                }

                // create database
                DbSystem.CreateDatabase(masterConnection, dbName);
                Console.Out.WriteLine(string.Format("Create Database: {0}", dbName));
                
                // use new database
                if (!string.IsNullOrEmpty(setupScriptPath))
                {
                    StreamReader sr = File.OpenText(setupScriptPath);
                    string commands = sr.ReadToEnd();
                    sr.Close();

                    string[] delims = { "GO\r\n" };
                    string[] commandSplit = commands.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    
                    //// Console.Out.WriteLine(commands);

                    for (int i = 0; i < commandSplit.Length; i++)
                    {
                        //// Console.Out.WriteLine(commandSplit[i]);

                        try
                        {
                            DbInterface.ExecuteQueryNoReturn(databaseConnection, commandSplit[i], null);
                        }
                        catch (Exception)
                        {
                            // some wierd problems with failures because we are creating/dropping dbs - so auto retry (once)
                            DbInterface.ExecuteQueryNoReturn(databaseConnection, commandSplit[i], null);
                        }
                    }
                }
            }            
        }
    }
}
