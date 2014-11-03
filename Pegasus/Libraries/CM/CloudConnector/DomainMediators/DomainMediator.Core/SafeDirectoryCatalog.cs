using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sage.Connector.DomainMediator.Core
{

    /*
     *  Example usage:
     * 
     *  var safeCatalog = new SafeDirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
     *  CompositionContainer Container = new CompositionContainer(safeCatalog.Catalog);
     * 
     */

    /// <summary>
    /// Safe directory catalog that filters out unloadable assemblies during the enumertion process
    /// vs. the composition process.
    /// </summary>
    public class SafeDirectoryCatalog : ComposablePartCatalog
    {
        #region Private Members

        private readonly AggregateCatalog _catalog = new AggregateCatalog();
        private readonly List<string> _loadedFiles = new List<string>();
        private readonly List<string> _unloadedFiles = new List<string>();
        private string[] _paths;

        #endregion

        #region Constructor

        /// <summary>
        /// Public constructor for the class.
        /// </summary>
        /// <param name="directory">The directory path to enumerate files for.</param>
        public SafeDirectoryCatalog(string directory)
        {
            string[] directories = new string[]{directory};
            BuildSafeDirectoryCatalog(directories);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDirectoryCatalog"/> class.
        /// </summary>
        /// <param name="directories">The directories.</param>
        public SafeDirectoryCatalog(IEnumerable<string> directories)
        {
            BuildSafeDirectoryCatalog(directories);
        }

        /// <summary>
        /// Builds the safe directory catalog.
        /// </summary>
        /// <param name="directories">The directories.</param>
        /// <exception cref="System.ArgumentNullException">directories</exception>
        private void BuildSafeDirectoryCatalog(IEnumerable<string> directories)
        {
            if (directories == null) throw new ArgumentNullException("directories");

            _paths = directories.ToArray();

            foreach (string directory in _paths)
            {
                AddDirectory(directory);   
            }
        }

        /// <summary>
        /// Adds the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <exception cref="System.ArgumentNullException">directory</exception>
        private void AddDirectory(string directory)
        {
            if (String.IsNullOrEmpty(directory)) throw new ArgumentNullException("directory");

            foreach (var file in Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories).ToList())
            {
                try
                {
                    var catalog = new AssemblyCatalog(file);

                    /* This is used to force a type load exception now vs. during the composition stage */
                    if (catalog.Parts.ToList().FirstOrDefault() != null)
                    {
                        _catalog.Catalogs.Add(catalog);
                        _loadedFiles.Add(file);
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    _unloadedFiles.Add(file);
                }
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _catalog.Parts;
            }
        }

        /// <summary>
        /// Returns the paths that were used to build the catalog from.
        /// </summary>
        public IList<string> Paths
        {
            get
            {
                return _paths;
            }
        }

        /// <summary>
        /// Returns the aggregate catalog.
        /// </summary>
        public AggregateCatalog Catalog
        {
            get
            {
                return _catalog;
            }
        }

        /// <summary>
        /// Returns the list of loaded files.
        /// </summary>
        public List<string> LoadedFiles
        {
            get
            {
                return _loadedFiles;
            }
        }

        /// <summary>
        /// Returns the list of unloaded files.
        /// </summary>
        public List<string> UnloadedFiles
        {
            get
            {
                return _unloadedFiles;
            }
        }

        #endregion
    }



}
