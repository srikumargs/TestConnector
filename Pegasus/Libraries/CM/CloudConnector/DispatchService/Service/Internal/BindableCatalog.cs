using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Diagnostics;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// A cached catalog of bindables (catalog is built on first access
    /// and will not be refreshed until the entire service is restarted)
    /// </summary>
    internal static class BindableCatalog
    {
        #region Private Helpers

        /// <summary>
        /// Protect for concurrent access/update of the cached bindable list
        /// </summary>
        private static readonly object _listLock = new Object();
        private static readonly Dictionary<string, IBindable> _lookup = new Dictionary<string, IBindable>();

        /// <summary>
        /// The static cached bindable list
        /// </summary>
        private static List<IBindable> _flatListCatalog = null;

        /// <summary>
        /// The type name for error purposes
        /// </summary>
        private static readonly String _myTypeName = typeof(BindableCatalog).FullName;

        /// <summary>
        /// Builds or retrieves the cached bindables
        /// </summary>
        /// <param name="logManager"></param>
        /// <returns></returns>
        private static List<IBindable> GetIBindableCatalog(LogManager logManager)
        {
                if (null == _flatListCatalog)
                {
                    _flatListCatalog = new List<IBindable>();
                    string lastFileName = string.Empty;
                    try
                    {
                        string[] fileNames = Directory.GetFiles(BinderPlugInComponentPath, PlugInSearchPattern);
                        foreach (string fileName in fileNames)
                        {
                            //Save off name so in case of an exception we know what we were doing.
                            lastFileName = fileName;
                            Assembly asm = Assembly.LoadFile(Path.Combine(BinderPlugInComponentPath, fileName));
                            if (null != asm)
                            {
                                Type[] types = asm.GetTypes();
                                foreach (Type type in types)
                                {
                                    if (null != type.GetInterface(typeof(IBindable).FullName))
                                    {
                                        object obj = asm.CreateInstance(type.FullName);
                                        IBindable ibd = obj as IBindable;
                                        if (null != ibd)
                                        {
                                            _flatListCatalog.Add(ibd);
                                            using (var lm = new LogManager())
                                            {
                                                lm.WriteInfo(null, "Dispatch binding catalog; adding {0} to support {1}", fileName, type.FullName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //NOTE: Moving exception management to per binder was discussed with Product. They felt that failing more visibly was better
                        //then only failing to load only the specific binder that failed.
                        if (null != logManager)
                        {
                            if (ex.GetType() == typeof(ReflectionTypeLoadException))
                            {
                                logManager.WriteError(null, "Unexpected ReflectionTypeLoadException during bindable catalog construction: {0}{1} while processing file: {2}", ex.ExceptionAsString(), Environment.NewLine, lastFileName);
                            }
                            else
                            {
                                logManager.WriteError(null, "Unexpected exception during bindable catalog construction: {0}", ex.ExceptionAsString());
                            }
                            
                        }
                    }
                }
                return _flatListCatalog;
        }
        #endregion

        #region Binding Path Helpers

        /// <summary>
        /// The path where compatible plug-in components are stored
        /// </summary>
        public static string BinderPlugInComponentPath
        { get { return Utils.BinderPluginsPath; } }

        /// <summary>
        /// The search pattern for plug in components
        /// </summary>
        public static string PlugInSearchPattern
        {
            get { return "*.dll"; }
        }

        #endregion

        /// <summary>
        /// Return the IBindable interface to use if one can be found. Null otherwise
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="logManager"></param>
        /// <returns></returns>
        public static IBindable FindIBindable(String messageType, LogManager logManager)
        {
            ArgumentValidator.ValidateNonEmptyString(messageType, "Message Type", _myTypeName + ".FindIBindable()");

            IBindable retval;

            lock (_listLock)
            {
                if (_lookup.TryGetValue(messageType.ToLower(), out retval))
                {
                    return retval;
                }

                List<IBindable> catalog = GetIBindableCatalog(logManager);

                foreach (IBindable bindableCandidate in catalog)
                {
                    if (bindableCandidate == null) continue;

                    if (bindableCandidate.SupportRequestKind(messageType))
                    {
                        _lookup[messageType.ToLower()] = bindableCandidate;
                        return bindableCandidate;
                    }
                }
            }

            return null;
        }
    }
}
