using System;

namespace Sage.Connector.AutoUpdate.Addin
{
    /// <summary>
    /// Constant class for addin name strings.
    /// </summary>
    internal class AddinNames
    {
        internal AddinNames(string baseName)
        {
            if(baseName == null) throw new NullReferenceException("baseName");

            _baseName = baseName;
        }

        /// <summary>
        /// Global mutex name for hiding staged updates.
        /// </summary>
        public const string Lock = "Global\\addin.update";

        /// <summary>
        /// addin.version string value.
        /// </summary>
        public const string Version = "addin.version.txt";

        /// <summary>
        /// addin.version string value.
        /// </summary>
        public const string Update = "addin.update.txt";

        /// <summary>
        /// sentinel file to disable updates for that addin.
        /// </summary>
        public const string NoUpdateFlag = "no.addin.update.txt";

        /// <summary>
        /// addin.staging string value.
        /// </summary>
        public string Staging 
        {
            get
            {
                if (string.IsNullOrEmpty(_staging))
                {
                    _staging = ComposeName("addin.staging");
                };
                return _staging;
            }
        }

        /// <summary>
        /// addin.backup string value.
        /// </summary>
        public string Backup
        {
            get
            {
                if (string.IsNullOrEmpty(_backup))
                {
                    _backup = ComposeName("addin.backup");
                }
                return _backup;
            }        
        }

        private string ComposeName(string target)
        {
            string retval = string.Format("{0}.{1}", _baseName, target);
            return retval;
        }

        /// <summary>
        /// notes.txt string value.
        /// </summary>
        public const string Notes = "notes.txt";


        private readonly string _baseName;
        private string _staging;
        private string _backup;
    }
}