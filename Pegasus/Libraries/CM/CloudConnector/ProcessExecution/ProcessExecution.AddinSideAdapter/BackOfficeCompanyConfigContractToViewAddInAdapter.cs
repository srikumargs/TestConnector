using System;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Response Event Args Contract to View AddIn Adapter
    /// </summary>
    public class BackOfficeCompanyConfigContractToViewAddInAdapter : BackOfficeCompanyConfiguration, IDisposable
    {

        private readonly Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration _contract;

        private readonly System.AddIn.Pipeline.ContractHandle _handle;

        // Track whether Dispose has been called. 
        private bool _disposed;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventArgs"/></param>
        public BackOfficeCompanyConfigContractToViewAddInAdapter(Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }

        /// <summary>
        /// The Response Event Args source contract
        /// </summary>
        /// <returns><see cref="IResponseEventArgs"/></returns>
        internal Sage.Connector.ProcessExecution.Interfaces.IBackOfficeCompanyConfiguration GetSourceContract()
        {
            return _contract;
        }

        /// <summary>
        /// Dispose properly of this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. 
        /// If disposing equals true, the method has been called directly 
        /// or indirectly by a user's code. Managed and unmanaged resources 
        /// can be disposed. 
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        /// </summary>
        /// <param name="disposing">When false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        ///  </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    _handle.Dispose();
                }

                // Note disposing has been done.
                _disposed = true;

            }
        }

        /// <summary>
        /// Get contract's BackOffice Id
        /// </summary>
        public override string BackOfficeId
        {
            get { return _contract.BackOfficeId; }
        }

        /// <summary>
        /// Get contract's Connection Credentials
        /// </summary>
        public override string ConnectionCredentials
        {
            get { return _contract.ConnectionCredentials; }
        }

        /// <summary>
        /// Gets the data path.
        /// This is where the DM can store files for the tenant/request
        /// </summary>
        /// <value>
        /// The data path base.
        /// </value>
        /// <remarks>
        /// This value if populated is the data storage path available to the DM layer.
        /// </remarks>
        public override string DataStoragePath
        {
            get { return _contract.DataStoragePath; }
        }
    }
}
