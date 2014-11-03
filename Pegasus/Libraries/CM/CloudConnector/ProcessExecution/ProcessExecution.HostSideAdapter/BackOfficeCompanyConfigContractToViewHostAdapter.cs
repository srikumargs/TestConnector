using System;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Args Contract to View Host Adapter
    /// </summary>
    public class BackOfficeCompanyConfigContractToViewHostAdapter : BackOfficeCompanyConfiguration , IDisposable
    {

        private readonly IBackOfficeCompanyConfiguration _contract;

        private readonly System.AddIn.Pipeline.ContractHandle _handle;

        //Track whether dispose has been called
        private bool _disposed;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract"><see cref="IResponseEventArgs"/></param>
        public BackOfficeCompanyConfigContractToViewHostAdapter(IBackOfficeCompanyConfiguration contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }


        /// <summary>
        /// The Reponse Event Args Source Contract
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
        /// Get the contract's BackOffice Id
        /// </summary>
        public override string BackOfficeId
        {
            get { return _contract.BackOfficeId; }
        }

        /// <summary>
        /// Get the contract's User Id
        /// </summary>
        public override string ConnectionCredentials
        {
            get { return _contract.ConnectionCredentials; }
        }

        /// <summary>
        /// Gets the data storage path.
        /// </summary>
        /// <value>
        /// The data storage path.
        /// </value>
        public override string DataStoragePath
        {
            get { return _contract.DataStoragePath; }
        }
    }
}
