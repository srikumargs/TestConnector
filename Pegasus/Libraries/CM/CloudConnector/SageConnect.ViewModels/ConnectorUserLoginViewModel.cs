using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Sage.Connector.Common;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sage.Connector.Management;


namespace SageConnect.ViewModels
{
    /// <summary>
    /// /
    /// </summary>
    public class ConnectorUserLoginViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public ConnectorUserLoginViewModel()
        {
            ExecuteCommandAction = LoginCommands;
            CheckSiteAddressVisibility();
            ProgressBarVisibility = Visibility.Hidden;
        }

        #region variables

        private Uri _siteAddressUri;
        private SiteGroup _selectedSiteGroup;

        #endregion

        #region loginproperties

        /// <summary>
        /// Site Address for getting the Tenant List
        /// </summary>
        public Uri SiteAddressUri
        {
            get { return _siteAddressUri; }
            set
            {
                _siteAddressUri = value;
                ApplicationHelpers.SiteAddressUri = _siteAddressUri;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Site Address for getting the Tenant List
        /// </summary>
        public String DataCloudWebPageDisplayName { get; set; }
        /// <summary>
        /// Site Address for getting the Tenant List
        /// </summary>
        public SiteGroup SelectedSiteGroup
        {
            get { return _selectedSiteGroup; }
            set
            {
                _selectedSiteGroup = value;
                ApplicationHelpers.SelectedSiteGroup = _selectedSiteGroup;
                DataCloudWebPageDisplayName = _selectedSiteGroup.DataCloudWebPageDisplayName;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// To set the List of URI available.
        /// </summary>
        public IList<SiteGroup> SiteAddressList
        {
            get { return _sitegroup; }

            set
            {
                _sitegroup = value;
                var firstOrDefault = _sitegroup.SingleOrDefault(i => i.IsDefault);
                if (firstOrDefault != null)
                {
                    ApplicationHelpers.SiteAddressUri = firstOrDefault.CloudSiteUri;
                    SelectedSiteGroup = firstOrDefault;
                    ShowEndPointAddress = (_sitegroup.Count == 1 ? Visibility.Hidden : Visibility.Visible);
                }
                OnPropertyChanged("SelectedSiteGroup");
            }
        }

        /// <summary>
        /// Site Address for getting the Tenant List
        /// </summary>
        public Visibility ShowEndPointAddress
        {
            get
            {
                return
                    _showEndPointAddress;
            }
            set
            {
                _showEndPointAddress = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///  Visibility of the webbrowser 
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get
            {
                return
                    _webBrowserVisibility;
            }
            set
            {
                _webBrowserVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void LoginCommands()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (ValidateSageAuthentication())
                {
                    GetTenanatData();
                }
                else if (ShowEndPointAddress == Visibility.Visible)
                {
                    GetTenanatData();
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }

            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }

        }


        private void ShowErrorMessage(string errormessage, string errorcaption)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowErrorMessage(errormessage, errorcaption);
            Mouse.OverrideCursor = Cursors.Arrow;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Uri GetCloudDeploymentUri()
        {
            Uri retvalUri = ConfigurationHelpers.GetDefaultCloudSiteUri();
            return retvalUri;
        }

        #region validation


        /// <summary>
        /// 
        /// </summary>

        public bool ValidateSageAuthentication()
        {
            var clientId = ApplicationHelpers.ClientId;
            var scope = ApplicationHelpers.Scope;
            ApplicationHelpers.SageIdToken = ConfigurationHelpers.ObtainAuthorizationToken(clientId, scope);
            ApplicationHelpers.SageloginSucess = !String.IsNullOrEmpty(ApplicationHelpers.SageIdToken);
            return !String.IsNullOrEmpty(ApplicationHelpers.SageIdToken);
        }

        #endregion

        /// <summary>
        /// Get the Tenant details list from the cloud
        /// </summary>
        /// <returns></returns>
        public List<UserManagementTenant> GetTenantDetail()
        {
            if (ApplicationHelpers.SiteAddressUri != null)
            {
                List<UserManagementTenant> tenantlist =
                    (List<UserManagementTenant>)
                        ConfigurationHelpers.GetTenantList(ApplicationHelpers.SageIdToken,
                            ApplicationHelpers.ConnectorServiceUri.ToString());
                return tenantlist;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DisplayTenantDetail()
        {
            try
            {
                List<UserManagementTenant> tenantList = GetTenantDetail();

                if (tenantList != null)
                {
                    // tenantList.RemoveRange(3, tenantList.Count-3);
                    List<ConfigurationViewModel> dataList = tenantList.Select(tenant => new ConfigurationViewModel
                    {
                        TenantGuid = tenant.TenantGuid,
                        TenantName = tenant.TenantName,
                        RegisteredConnectorId = tenant.RegisteredConnectorId,
                        RegisterdCompanyId = tenant.RegisteredCompanyId,
                        ConnectionStatus =
                            ApplicationHelpers.GetTenantStatus(tenant.TenantGuid, tenant.RegisteredConnectorId),
                        CloudStatus =
                            ApplicationHelpers.CloudConnectionStatus(tenant.TenantGuid, tenant.RegisteredConnectorId),
                        BackOfficeStatus = (ApplicationHelpers.BackofficeConnectionStatus(tenant.TenantGuid) ? 3 : 0),
                    }).ToList();
                    TenantDataList = dataList;
                    if (dataList.Count == 0) return false;
                    return true;
                }
            }
            catch (Exception ex)
            {

                ShowErrorMessage(ex.Message, CustomerFacingMessages.ErrorCaption);
            }
            return false;
        }

        private BackgroundWorker _worker;

        /// <summary>
        /// To Start the Connector Services
        /// </summary>
        public void GetTenanatData()
        {

            AssignBackgroundWorker();
            _worker.RunWorkerAsync();
            ErrorMessageViewModel.ErrorMessageViewModelInstance.DisplayProgressbar = Visibility.Visible;
        }

        /// <summary>
        /// To get tenant data thru service
        /// </summary>
        private void AssignBackgroundWorker()
        {
            _worker = new BackgroundWorker { WorkerReportsProgress = true }; // variable declared in the class
            _worker.DoWork += StartservicesWorkerDoWork;
            _worker.ProgressChanged += StartservicesWorkerProgressChanged;
            _worker.RunWorkerCompleted += StartservicesWorkerRunWorkerCompleted;
        }

        private void StartservicesWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            _worker.ReportProgress(DisplayTenantDetail() ? 100 : 00);
        }

        private void StartservicesWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.DisplayProgressbar = Visibility.Hidden;
            if (e.ProgressPercentage == 100)
            {

                AnimateGrid.CollapseLoginForm();
            }
            else
            {

                ShowErrorMessage(CustomerFacingMessages.ErrorMessage_NoTenantExists,
                    CustomerFacingMessages.ErrorMessage_NoTenantCaption);
            }
        }

        private void StartservicesWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        { }

        /// <summary>
        /// To check for the visiblity of the Siteaddress.
        /// </summary>
        public void CheckSiteAddressVisibility()
        {
            SiteAddressList = ConfigurationHelpers.GetSiteGroups();
        }

        private List<ConfigurationViewModel> _tenantDataList;

        /// <summary>
        /// Property to identify the Accordion index
        /// </summary>
        public List<ConfigurationViewModel> TenantDataList
        {
            get { return _tenantDataList; }
            set
            {
                _tenantDataList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Property Changed event derived from the List.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private Visibility _showEndPointAddress;
        private Visibility _webBrowserVisibility;
        private IList<SiteGroup> _sitegroup;


        ///  <summary>
        /// Create the OnPropertyChanged method to raise the event  
        ///  </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
