using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.XPath;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - N/A
namespace Sage.Sandbox.Tools.LinkedSource
{
    /// <summary>
    /// Exception thrown when a schema error is found with a folder redirect file
    /// </summary>
    [Serializable]
    internal sealed class FolderRedirectSchemaException : Exception
    {
        #region Construction

        /// <summary>
        /// Construct an instance of the FolderRedirectSchemaException class
        /// </summary>
        /// <param name="errorMessage">ErrorMessage</param>
        /// <param name="innerException">An inner exception</param>
        public FolderRedirectSchemaException(String errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        { }

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        /// <param name="si">Serialization Info</param>
        /// <param name="sc">Streaming Context</param>
        private FolderRedirectSchemaException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        { }

        #endregion
    }

    /// <summary>
    /// </summary>
    internal static class FolderManager
    {
        #region Public Properties

        /// <summary>
        /// Returns the location where the shared Sage libraries are installed
        /// </summary>
        /// <remarks>Although shared libraries are typically expected to be 
        /// located at "Program Files\Common Files\Sage", an alternate location 
        /// can be specified via the 'Location' attribute on the 'SharedLibraries' 
        /// element in a FolderRedirect.xml file at: 
        /// C:\Documents and Settings\All Users\Application Data\Sage</remarks>
        public static String SageCommonProgramFilesLocation
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_sageCommonProgramFilesLocation == null)
                    {
                        _sageCommonProgramFilesLocation = GetRedirectLocation(Constants.SageCommonProgramFilesRedirectExpression);
                        if (_sageCommonProgramFilesLocation == null)
                        {
                            _sageCommonProgramFilesLocation = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), Constants.Sage);
                        }
                    }

                    ConditionalTrace("SageCommonProgramFilesLocation is '{0}'.", _sageCommonProgramFilesLocation);

                    return _sageCommonProgramFilesLocation;
                }
            }
        }


        /// <summary>
        /// Returns the location where the shared libraries are installed
        /// </summary>
        /// <remarks>Although shared libraries are typically expected to be 
        /// located at "Program Files\Common Files", an alternate location 
        /// can be specified via the 'Location' attribute on the 'SharedLibraries' 
        /// element in a FolderRedirect.xml file at: 
        /// C:\Documents and Settings\All Users\Application Data\Sage</remarks>
        public static String CommonProgramFilesLocation
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_commonProgramFilesLocation == null)
                    {
                        _commonProgramFilesLocation = GetRedirectLocation(Constants.CommonProgramFilesRedirectExpression);
                        if (_commonProgramFilesLocation == null)
                        {
                            _commonProgramFilesLocation = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                        }
                    }

                    ConditionalTrace("CommonProgramFilesLocation is '{0}'.", _commonProgramFilesLocation);

                    return _commonProgramFilesLocation;
                }
            }
        }

        /// <summary>
        /// Return the location for shared documents on the current machine
        /// </summary>
        /// <remarks>Although this location is typically 
        /// "C:\Documents and Settings\All Users\Application Data\Shared Documents", 
        /// an alternate location can be specified via the 'Location' attribute on the 'SharedDocuments'
        /// element in a FolderRedirect.xml file at: 
        /// C:\Documents and Settings\All Users\Application Data\Sage</remarks>
        public static String SharedDocumentsLocation
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_sharedDocumentsLocation == null)
                    {
                        _sharedDocumentsLocation = GetRedirectLocation(Constants.SharedDocumentsRedirectExpression);
                        if (_sharedDocumentsLocation == null)
                        {
                            String location = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.SharedDocumentsFolder);
                            _sharedDocumentsLocation = Path.Combine(location, Constants.SharedDocumentsFolder);
                        }
                    }
                }

                ConditionalTrace("SharedDocumentsLocation is '{0}'.", _sharedDocumentsLocation);

                return _sharedDocumentsLocation;
            }
        }

        /// <summary>
        /// Return the location for configuration files shared by all users on a machine
        /// </summary>
        /// <remarks>Although this location is typically 
        /// "C:\Documents and Settings\All Users\Application Data\Sage", 
        /// an alternate location can be specified via the 'Location' attribute on the 
        /// 'SharedConfiguration' element in a FolderRedirect.xml file at: 
        /// C:\Documents and Settings\All Users\Application Data\Sage</remarks>
        public static String SageCommonApplicationDataLocation
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_sageCommonApplicationDataLocation == null)
                    {
                        _sageCommonApplicationDataLocation = GetRedirectLocation(Constants.SageCommonApplicationDataRedirectExpression);
                        if (_sageCommonApplicationDataLocation == null)
                        {
                            _sageCommonApplicationDataLocation = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.Sage);
                        }
                    }
                }

                ConditionalTrace("SageCommonApplicationDataLocation is '{0}'.", _sageCommonApplicationDataLocation);

                return _sageCommonApplicationDataLocation;
            }
        }

        /// <summary>
        /// Base location for configuration files for a specific user on a machine
        /// </summary>
        /// <remarks>Although this location is typically 
        /// "C:\Documents and Settings\[User Name]\Application Data\Sage", 
        /// an alternate location can be specified via the 'Location' attribute on 
        /// the 'UserConfigurationBase' element in a FolderRedirect.xml file at: 
        /// C:\Documents and Settings\All Users\Application Data\Sage</remarks>
        public static String SageApplicationDataLocation
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_sageApplicationDataLocation == null)
                    {
                        String userConfigBaseLocation = GetRedirectLocation(Constants.SageApplicationDataRedirectExpression);
                        if (userConfigBaseLocation != null)
                        {
                            _sageApplicationDataLocation = Path.Combine(userConfigBaseLocation, Path.Combine(Environment.UserName, Path.Combine(Constants.ApplicationData, Constants.Sage)));
                        }
                        else
                        {
                            _sageApplicationDataLocation = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.Sage);
                        }
                    }
                }

                ConditionalTrace("SageApplicationDataLocation is '{0}'.", _sageApplicationDataLocation);

                return _sageApplicationDataLocation;
            }
        }

        /// <summary>
        /// Return the location for My Documents for a specific user on a machine
        /// </summary>
        /// <remarks>Although this location is typically 
        /// "C:\Documents and Settings\[User Name]\My Documents", 
        /// an alternate location can be specified via the 'Location' attribute on the 
        /// 'UserConfigurationBase' element in a FolderRedirect.xml file at: 
        /// C:\Documents and Settings\All Users\Application Data\Sage</remarks>
        public static String MyDocumentsLocation
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_myDocumentsLocation == null)
                    {
                        String sageApplicationDataBaseLocation = GetRedirectLocation(Constants.SageApplicationDataRedirectExpression);
                        if (sageApplicationDataBaseLocation != null)
                        {
                            String myDocuments = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            _myDocumentsLocation = Path.Combine(sageApplicationDataBaseLocation, Path.Combine(Environment.UserName, myDocuments.Substring(myDocuments.LastIndexOf('\\') + 1)));
                        }
                        else
                        {
                            _myDocumentsLocation = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        }
                    }
                }

                ConditionalTrace("MyDocumentsLocation is '{0}'.", _myDocumentsLocation);

                return _myDocumentsLocation;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieve a location specified by a redirect file
        /// </summary>
        /// <param name="redirectTypeExpression">An XPath expression used to locate the redirect information for a specific type of file</param>
        /// <returns>The location specified by a redirect file</returns>
        /// <remarks>The redirect file is a simple XML file that contains alternate locations for various default locations</remarks>
        private static String GetRedirectLocation(String redirectTypeExpression)
        {
            Debug.Assert(!String.IsNullOrEmpty(redirectTypeExpression));

            String redirectLocation = null;
            String redirectFile = GetRedirectFile();
            if (redirectFile != null)
            {
                ConditionalTrace("Processing folder redirect file '{0}'. XPath Expression: {1}", redirectFile, redirectTypeExpression);

                XPathNavigator documentNavigator = null;
                try
                {
                    documentNavigator = new XPathDocument(redirectFile, XmlSpace.Default).CreateNavigator();
                }
                catch (XmlException error)
                {
                    throw new FolderRedirectSchemaException(String.Format(CultureInfo.CurrentUICulture, "An error was encountered in the schema of the {0} file. please see the inner exception for details on the specific error encountered.", Path.GetFileName(redirectFile)), error);
                }

                XPathNodeIterator iterator = documentNavigator.Select(redirectTypeExpression);
                if (iterator.MoveNext())
                {
                    redirectLocation = iterator.Current.GetAttribute(Constants.LocationAttribute, String.Empty);
                    if (!String.IsNullOrEmpty(redirectLocation))
                    {
                        ConditionalTrace("Redirect location specified is '{0}'", redirectLocation);
                    }
                }

                if (String.IsNullOrEmpty(redirectLocation))
                {
                    throw new DirectoryNotFoundException(String.Format(CultureInfo.CurrentUICulture, "No location was specified in {0} for {1}.", redirectFile, redirectTypeExpression));
                }
            }
            return redirectLocation;
        }

        /// <summary>
        /// Returns the name and path of the redirect file if it exists 
        /// or null if it does not exists.
        /// </summary>
        private static String GetRedirectFile()
        {
            String result = null;

            // To improve the SxS dev environment story, lookup possible folder redirection based upon the location
            // of the entry assembly or the executing assembly, if we have not entry assembly.  Allows multiple
            // sandboxes to coexist on a single machine.
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            if (assembly != null)
            {
                String entryAssemblyLocation = assembly.Location;
                if (!String.IsNullOrEmpty(entryAssemblyLocation))
                {
                    using (var subKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Sage\Sandbox\Tools\FolderRedirects", false))
                    {
                        if (subKey != null)
                        {
                            String pathRoot = Path.GetPathRoot(entryAssemblyLocation).ToLowerInvariant();

                            do
                            {
                                entryAssemblyLocation = Path.GetDirectoryName(entryAssemblyLocation);
                                result = (String)subKey.GetValue(entryAssemblyLocation);
                            } while (String.IsNullOrEmpty(result) &&
                                (pathRoot != entryAssemblyLocation.ToLowerInvariant()));
                        }
                    }
                }
            }

            // if no redirect location was found based on the EntryAssembly, then look for the default FolderRedirect.xml
            // in the OS's "Common AppData\Sage"
            if (String.IsNullOrEmpty(result))
            {
                String location = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                result = Path.Combine(location, Path.Combine("Sage", "FolderRedirect.xml"));
            }

            // if no file exists, then we aren't doing any redirecting
            if (!File.Exists(result))
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Writes a message to the trace listeners if the "Sage.Sandbox.Tools.LinkedSource.FolderManager"
        /// trace switch is enabled.
        /// </summary>
        /// <remarks>
        /// To enable the switch, place the following in the application's exe.config:
        /// <example>
        ///     <system.diagnostics>
        ///         <switches>
        ///             <add name="Sage.Sandbox.Tools.LinkedSource.FolderManager" value="1" />
        ///         </switches> 
        ///     </system.diagnostics>
        /// </example>
        /// </remarks>
        /// <param name="format">A String containing zero or more format items.</param>
        /// <param name="args">An Object array containing zero or more objects to format.</param>
        private static void ConditionalTrace(String format, params Object[] args)
        {
            Trace.WriteLineIf(_traceSwitch.Enabled, String.Format(CultureInfo.CurrentUICulture, String.Format(CultureInfo.CurrentUICulture, "{0}: {1}", typeof(FolderManager).FullName, format), args));
        }

        #endregion

        #region Fields
        // The location where shared libraries are installed
        private static String _sageCommonProgramFilesLocation; // = null; Initialized by the runtime

        // The location where common program files are installed
        private static String _commonProgramFilesLocation; // = null; Initialized by the runtime

        //The location of the shared documents folder
        private static String _sharedDocumentsLocation; // = null; Initialized by the runtime.

        // The location of Shared configuration files
        private static String _sageCommonApplicationDataLocation; // = null; Initialized by the runtime.

        // The location of user configuration files
        private static String _sageApplicationDataLocation; // = null; Initialized by the runtime.

        // The location of My Documents
        private static String _myDocumentsLocation; // = null; Initialized by the runtime.

        // Used for thread syncronization
        private static Object _syncRoot = new Object();

        // A switch to control trace output.
        private static BooleanSwitch _traceSwitch = new BooleanSwitch("Sage.Sandbox.Tools.LinkedSource.FolderManager", "Controls tracing for Sage.Sandbox.Tools.FolderManager.", "0");
        #endregion

        private class Constants
        {
            public const String SageCommonProgramFilesRedirectExpression = "//FolderRedirect/SharedLibraries";
            public const String CommonProgramFilesRedirectExpression = "//FolderRedirect/CommonProgramFiles";
            public const String SharedDocumentsRedirectExpression = "//FolderRedirect/SharedDocuments";
            public const String SageCommonApplicationDataRedirectExpression = "//FolderRedirect/SharedConfiguration";
            public const String SageApplicationDataRedirectExpression = "//FolderRedirect/UserConfigurationBase";
            public const String LocationAttribute = "Location";
            public const String FolderRedirectFile = "FolderRedirect.xml";
            public const String Sage = "Sage";
            public const String SharedDocumentsFolder = "Shared Documents";
            public const String ApplicationData = "Application Data";
        }
    }
}
