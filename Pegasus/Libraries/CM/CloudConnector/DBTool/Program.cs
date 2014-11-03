using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Sage.Connector.DBTool.Internal;
using Sage.Connector.DBTool.Migration;
using Sage.Connector.Utilities;

namespace Sage.Connector.DBTool
{
    public sealed class Program
    {
        [STAThread]
        private static void Main(String[] args)
        {
            ExitCode commandSpecificExitCode = ExitCode.OneOrMoreFailuresOccurred;
            System.Environment.ExitCode = (Int32)commandSpecificExitCode;
            
            try
            {
                Options options = new Options();
                // parse options out of the arguments
                ParseOptions(options, args);

                if (_printHelp || !IsValidOptionsSpecified(options))
                {
                    PrintHelp();
                    Environment.ExitCode = (Int32)ExitCode.OneOrMoreFailuresOccurred;
                }
                else
                {
                    PrintBanner();

                    _outputFileLocation = options.OutputFileLocation;
                    if (!String.IsNullOrEmpty(_outputFileLocation))
                    {
                        using (var file = File.Create(_outputFileLocation))
                        { }
                    }

                    Boolean success = false;
                    

                    DateTime startTime = DateTime.Now;

                    Program.WriteLine("Processing: " + options.Operation.ToString(), !_silent);

                    switch(options.Operation)
                    {
                        case Operation.SchemaUpgradeAndDataMigrate:
                            commandSpecificExitCode = DoSchemaUpgradeAndDataMigrate(options);
                            success = (commandSpecificExitCode == ExitCode.Success);
                            break;

                        case Operation.HardCorruptDatabase:
                            success = DoHardDatabaseCorruption(options);
                            break;

                        case Operation.RepairDatabase:
                            success = DoDatabaseRepair(options);
                            break;

                        case Operation.TestConnection:
                            success = DoTestConnection(options);
                            break;

                        default:
                            TraceWriteLine("Unknown operation: " + options.Operation.ToString());
                            throw new NotImplementedException("Unknown operation: " + options.Operation.ToString());
                    }

                    if (success)
                    {
                        System.Environment.ExitCode = (Int32)ExitCode.Success;
                    }
                    else
                    {
                        //possibly update the exit code with fresh data;
                        System.Environment.ExitCode = (Int32)commandSpecificExitCode;
                    }

                    Program.WriteLine(String.Format("{0} operation completed; success={1}; elapsed time {2}", options.Operation.ToString(), success, DateTime.Now - startTime), !_silent);
                }
            }
            catch (Exception ex)
            {

                try
                {
                    if ((String.IsNullOrEmpty(_outputFileLocation) || File.Exists(_outputFileLocation)) && !_silent)
                    {
                        foreach (var trace in _traces)
                        {
                            Console.WriteLine(trace);
                        }

                        Console.WriteLine("Exception encountered:");
                        Console.WriteLine(ex.ToString());
                    }
                    else if (!String.IsNullOrEmpty(_outputFileLocation) && File.Exists(_outputFileLocation))
                    {
                        File.AppendAllLines(_outputFileLocation, _traces);
                        File.AppendAllText(_outputFileLocation, "Exception encountered:\n");
                        File.AppendAllText(_outputFileLocation, ex.ToString() + "\n");
                    }
                }
                catch (Exception ex2)
                {
                    TraceWriteLine("Exception encountered trying to write an exception:");
                    TraceWriteLine(ex2.ToString());
                    TraceWriteLine("Original exception:");
                    TraceWriteLine(ex.ToString());
                }
            }

            Program.WriteLine(String.Format("Environment.ExitCode={0}", System.Environment.ExitCode), !_silent);
        }

        #region Private Methods
        private static void ParseOptions(Options engineOptions, String[] args)
        {
            Dictionary<String, OptionHandler> optionHandlers = new Dictionary<String, OptionHandler>();
            optionHandlers["/help"] = new OptionHandler(HelpOptionHandler);
            optionHandlers["/?"] = new OptionHandler(HelpOptionHandler);
            optionHandlers["/nologo"] = new OptionHandler(NoLogoOptionHandler);
            optionHandlers["/silent"] = new OptionHandler(SilentOptionHandler);
            optionHandlers["/s"] = new OptionHandler(SilentOptionHandler);
            optionHandlers["/operation:"] = new OptionHandler(OperationOptionHandler);
            optionHandlers["/o:"] = new OptionHandler(OperationOptionHandler);
            optionHandlers["/instanceDataDir:"] = new OptionHandler(InstanceDataDirOptionHandler);
            optionHandlers["/idd:"] = new OptionHandler(InstanceDataDirOptionHandler);
            optionHandlers["/databaseFileName"] = new OptionHandler(DatabaseFileNameOptionHandler);
            optionHandlers["/df:"] = new OptionHandler(DatabaseFileNameOptionHandler);
            optionHandlers["/priorVersionBackupDir:"] = new OptionHandler(PriorVersionBackupDirOptionHandler);
            optionHandlers["/pvbd:"] = new OptionHandler(PriorVersionBackupDirOptionHandler);
            optionHandlers["/priorVersion:"] = new OptionHandler(PriorVersionOptionHandler);
            optionHandlers["/pv:"] = new OptionHandler(PriorVersionOptionHandler);
            optionHandlers["/currentVersion:"] = new OptionHandler(CurrentVersionOptionHandler);
            optionHandlers["/cv:"] = new OptionHandler(CurrentVersionOptionHandler);
            optionHandlers["/output:"] = new OptionHandler(OutputOptionHandler);
            optionHandlers["/out:"] = new OptionHandler(OutputOptionHandler);

            foreach (String arg in args)
            {
                TraceWriteLine(arg);

                Int32 nPos = arg.IndexOf(':');
                String param = arg.Substring(nPos + 1);
                String option = (nPos == -1 ? arg : arg.Substring(0, nPos + 1));

                if (optionHandlers.ContainsKey(option))
                {
                    optionHandlers[option](engineOptions, param);
                }
            }
        }

        private delegate void OptionHandler(Options options, String param);

        private static Boolean IsValidOptionsSpecified(Options options)
        {
            switch (options.Operation)
            {
                case Operation.SchemaUpgradeAndDataMigrate:
                    return
                        !String.IsNullOrEmpty(options.InstanceDataDir) &&
                        !String.IsNullOrEmpty(options.PriorVersionBackupDir) &&
                        !String.IsNullOrEmpty(options.PriorVersion) &&
                        !String.IsNullOrEmpty(options.CurrentVersion);
                
                case Operation.HardCorruptDatabase:
                case Operation.RepairDatabase:
                    return
                        !String.IsNullOrEmpty(options.InstanceDataDir) &&
                        !String.IsNullOrEmpty(options.DatabaseFileName) &&
                        !String.IsNullOrEmpty(options.OutputFileLocation);

                case Operation.TestConnection:
                    return
                        !String.IsNullOrEmpty(options.InstanceDataDir) &&
                        !String.IsNullOrEmpty(options.DatabaseFileName);
                
                case Operation.None:
                default:
                    return false;
            }
        }

        private static void HelpOptionHandler(Options options, String param)
        { _printHelp = true; }

        private static void NoLogoOptionHandler(Options options, String param)
        { _noLogo = true; }

        private static void SilentOptionHandler(Options options, String param)
        { _silent = true; }

        private static void OperationOptionHandler(Options options, String param)
        { options.Operation = (Operation)Enum.Parse(typeof(Operation), param); }

        private static void InstanceDataDirOptionHandler(Options options, String param)
        { options.InstanceDataDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(param)); }

        private static void PriorVersionBackupDirOptionHandler(Options options, String param)
        { options.PriorVersionBackupDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(param)); }

        private static void PriorVersionOptionHandler(Options options, String param)
        { options.PriorVersion = param; }

        private static void CurrentVersionOptionHandler(Options options, String param)
        { options.CurrentVersion = param; }

        private static void DatabaseFileNameOptionHandler(Options options, String param)
        { options.DatabaseFileName = param; }

        private static void OutputOptionHandler(Options options, String param)
        { options.OutputFileLocation = Path.GetFullPath(Environment.ExpandEnvironmentVariables(param)); }

        private static void PrintBanner()
        {
            if (!_noLogo)
            {
                WriteLine("Sage Connector DB Tool.  Version " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
                WriteLine(String.Empty);
            }
        }

        private static void PrintHelp()
        {
            PrintBanner();

            WriteLine("Syntax: DBTool /operation:<operation> [Options]");
            WriteLine("");
            WriteLine("Options:");
            WriteLine("  /operation:<operation>, /o:<operation>");
            WriteLine("    'SchemaUpgradeAndDataMigrate' perform a schema and data upgrade from a");
            WriteLine("           prior version.");
            WriteLine("           also requires /idd:<path> /pvbd:<path> /cv:<version> /pv:<version>");
            WriteLine("    'HardDatabaseCorruption' hard corrupt a specific database");
            WriteLine("           also requires /idd:<path> /df:<filename> /out:<backupFilename>");
            WriteLine("    'RepairDatabase' attempt to repair a specific database");
            WriteLine("           also requires /idd:<path> /df:<filename> /out:<backupFilename>");
            WriteLine("    'TestConnection' attempt to connect to a specific database");
            WriteLine("           also requires /idd:<path> /df:<filename>");
            WriteLine("");
            WriteLine("  /instanceDataDir:<path>, /idd:<path>");
            WriteLine("    The location of <path> contains the database files that will be used");
            WriteLine("");
            WriteLine("  /priorVersionBackupDir:<path>, /pvbd:<path>");
            WriteLine("    The location of <path> contains the original 'prior version' database");
            WriteLine("    files that will be used");
            WriteLine("");
            WriteLine("  /currentVersion:<version>, /cv:<version>");
            WriteLine("    The current version number");
            WriteLine("");
            WriteLine("  /priorVersion:<version>, /pv:<version>");
            WriteLine("    The prior version number");
            WriteLine("");
            WriteLine("  /databaseFileName:<fileName>, /df:<fileName>");
            WriteLine("    The database file name for corruption and repair operations");
            WriteLine("");
            WriteLine("  /output:<file>, /out:<file>");
            WriteLine("    Writes the resulting output to <file>");
            WriteLine("");
            WriteLine("  /silent");
            WriteLine("    Turns off the progress output to the console");
            WriteLine("");
            WriteLine("  /nologo");
            WriteLine("    Suppress display of the logo banner");
            WriteLine("");
            WriteLine("  /?, /help");
            WriteLine("    Displays this usage message");
        }

        internal static void TraceWriteLine(String line)
        {
            _traces.Add(line);
            System.Diagnostics.Trace.WriteLine(line); 
        }

        internal static void WriteLine(String line, Boolean forceToConsole)
        {
            TraceWriteLine(line);

            if (!String.IsNullOrEmpty(_outputFileLocation) && File.Exists(_outputFileLocation))
            {
                File.AppendAllLines(_outputFileLocation, new String[] { line });
            }

            if (forceToConsole || !_silent)
            {
                Console.WriteLine(line);
            }
        }

        internal static void WriteLine(String line)
        {
            WriteLine(line, false);
        }

        private static ExitCode DoSchemaUpgradeAndDataMigrate(Options options)
        {
            MigrationController migrationController = new MigrationController();
            ExitCode result = migrationController.Migrate(
                        options.PriorVersion, 
                        options.PriorVersionBackupDir, 
                        options.CurrentVersion, 
                        options.InstanceDataDir,
                        WriteLine);

            return result;
        }

        private static Boolean DoHardDatabaseCorruption(Options options)
        {
            string fullDatabaseFileName = Path.Combine(options.InstanceDataDir, options.DatabaseFileName);
            return DatabaseRepairUtils.HardCorruptDatabase(fullDatabaseFileName, options.OutputFileLocation);
        }

        private static Boolean DoDatabaseRepair(Options options)
        {
            try
            {
                string fullDatabaseFileName = Path.Combine(options.InstanceDataDir, options.DatabaseFileName);
                return DatabaseRepairUtils.RepairDatabase(
                    fullDatabaseFileName, 
                    options.OutputFileLocation,
                    null);
            }
            catch (SqlCeException)
            {
                // Any SQL CE exception here means that our repair did not work
                // Return false to avoid confusing results (e.g. "password specified is incorrect")
                return false;
            }
        }

        private static Boolean DoTestConnection(Options options)
        {
            string fullDatabaseFileName = Path.Combine(options.InstanceDataDir, options.DatabaseFileName);
            return DatabaseRepairUtils.TestConnection(fullDatabaseFileName);
        }

        #endregion

        #region Private fields
        private static readonly List<String> _traces = new List<String>();
        private static String _outputFileLocation;
        private static Boolean _silent;
        private static Boolean _printHelp;
        private static Boolean _noLogo;
        #endregion
    }
}
