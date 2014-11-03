using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SageConnect.ViewModels
{
    /// <summary>
    /// To Maintain all ateh configuration details
    /// </summary>
   public class ConfigurationViewModel : INotifyPropertyChanged
    {

       /// <summary>
        /// Gets or sets the tenant unique identifier.
        /// </summary>
        /// <value>
        /// The tenant unique identifier.
        /// </value>
        public Guid TenantGuid { get;  set; }

        /// <summary>
        /// Gets or sets the name of the user management tenant.
        /// </summary>
        /// <value>
        /// The name of the user management tenant.
        /// </value>
        public string TenantName { get;  set; }

        /// <summary>
        /// Gets or sets the name of the registered connector
        /// </summary>
        public string RegisteredConnectorId { get; set; }
 
        /// <summary>
        /// Gets or sets the name of the registered company
        /// </summary>
        public string RegisterdCompanyId { get; set; }

        private ConnectionState _connectionState;
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public ConnectionState ConnectionStatus
        {
            get { return _connectionState; }
            set
            {
                _connectionState = value;
                OnPropertyChanged();
            }
        }

        private ConnectionState _cloudstatus;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public ConnectionState CloudStatus
        {
            get { return _cloudstatus; }

            set
            {
                _cloudstatus = value;
                OnPropertyChanged();
            }
        }


        private object _backOfficeStatus;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public object BackOfficeStatus
        {
            get { return _backOfficeStatus; }
            set
            {
                _backOfficeStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// back office company name selected fro connection
        /// </summary>
        public string BackOfficeCompanyName { get; set; }

        /// <summary>
        /// back office id selected for connection
        /// </summary>
        public string BackOfficeid { get; set; }


        ///// <summary>
        ///// BackOfficePluginAutoUpdateProductId selected for connection
        ///// </summary>
        //public string BackOfficePluginAutoUpdateProductId { get; set; }
        /// <summary>
        /// BackofficeProductName selected for connection
        /// </summary>
        public static string BackofficeProductName { get; set; }

        /// <summary>
        /// Backoffice connection credentials description saved for view purpose
        /// </summary>
        public string BackOfficeConnectionCredentialsDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    /// <summary>
    /// Connection Status 
    /// </summary>
    public enum ConnectionState
    {
        
        /// <summary>
        /// On line status of Connection
        /// </summary>
        OnLine,

        /// <summary>
        /// Connection is Offline - Not connected
        /// </summary>
        OffLine,

        /// <summary>
        /// Error in Connection
        /// </summary>
        Error,


        /// <summary>
        /// The connection is configured in a different Machine
        /// </summary>
        Configured

    }
}

