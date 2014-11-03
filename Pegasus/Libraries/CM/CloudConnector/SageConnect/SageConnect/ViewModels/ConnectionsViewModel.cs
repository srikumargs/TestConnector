
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Converters;
using Sage.Connector.Common;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using Sage.Connector.Configuration.Mediator;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.Configuration.Mediator.JsonConverters;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Data;
using SageConnect.Control;
using Sage.Connector.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SageConnect.Internal;


namespace SageConnect.ViewModels
{
    /// <summary>
    /// Handles the Connections 
    /// </summary>
    public class ConnectionsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly BackOfficeServiceManager _backOfficeServiceManager;
        private readonly FeatureServiceManager _featureServiceManager;
        private static ConnectionsViewModel _connectionsViewModel;
        private Dictionary<FeatureMetadata, IList<PropertyDefinition>> _metaPropertyLists;
        private PremiseConfigurationRecord _premiseConfigurationRecord;

        private static readonly string _connectionCredentialsCaption =
            CustomerFacingMessages.ConnectionCredentialsCaption;

        private static readonly string _adminCredentialsCaption = CustomerFacingMessages.AdminCredentialsCaption;
        private static readonly string _backOfficeCaption = CustomerFacingMessages.BackOfficeCaption;
        // The back office name is used in transactionvew model,in CommandSelected event, changing name here needs to be changed there too
        private const string BackOfficeName = "Backoffice";
        private static readonly string _errorCaption = CustomerFacingMessages.ErrorCaption;

        /// <summary>
        /// Constructor for Connections
        /// </summary>
        /// 
        public ConnectionsViewModel()
        {
            if (_connectionsViewModel == null)
                _connectionsViewModel = this;

            _backOfficeServiceManager = new BackOfficeServiceManager();
            _featureServiceManager = new FeatureServiceManager();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ConnectionsViewModel InstanceConnectionsViewModel
        {
            get { return _connectionsViewModel; }
            set { _connectionsViewModel = value; }

        }


        /// <summary>
        /// 
        /// </summary>
        public TransistionControl ContentControl { get; set; }


        /// <summary>
        /// Revoke any current authentication token
        /// </summary>
        public void SignOut()
        {
            ConfigurationHelpers.SignOut(ApplicationHelpers.ClientId);
            ContentControl = null;
        }




        /// <summary>
        /// To Display the Selected Tenant details/ Create new Tenant
        /// </summary>
        ///
        public void Connectionselect(int count)
        {
            try
            {
                string progressstate = CustomerFacingMessages.Progressbar_LoadingConnection;
                Mouse.OverrideCursor = Cursors.AppStarting;
                _premiseConfigurationRecord = null;
                _premiseConfigurationRecord =
                    ConfigurationHelpers.RetrieveTenantConfiguration(ConfigurationViewModel.TenantGuid.ToString());
                TenantName = ConfigurationViewModel.TenantName;
                MessageDescription = CustomerFacingMessages.Connection_Gridcustomersite + " : " + TenantName;
                ShowHelpSetting = Visibility.Hidden;
                if (ConfigurationViewModel.RegisteredConnectorId != String.Empty &&
                    ConfigurationViewModel.RegisteredConnectorId != ConfigurationHelpers.GetConnectorId().ToString() &&
                    _premiseConfigurationRecord == null)
                {
                    ShowInformationMessage(CustomerFacingMessages.InfoMessage_TenantConnectionExists, CustomerFacingMessages.InfoCaption_TenantConnectionExists, HelpLinkManager.FindString("Connetion_HelpFile"));
                    ConnectiondetailVisiblity = Visibility.Hidden;
                    ShowEditSetting = Visibility.Hidden;
                    AnimateGrid.CollapseConnectionDetails();
                    return;
                }
                AnimateGrid.ExpandConnectionDetails();
                ConnectiondetailVisiblity = Visibility.Visible;
                if (_premiseConfigurationRecord != null)
                {
                    ShowEditSetting = Visibility.Visible;
                    DisplayConfigurationDetails(_premiseConfigurationRecord,progressstate);


                }
                else
                {
                    ShowEditSetting = Visibility.Hidden;
                    _premiseConfigurationRecord = ConfigurationHelpers.CreateNewTenantConfiguration();
                    ConfigurationViewModel.BackOfficeid = String.Empty;
                    Controlcollection controlcollection = DisplayBackOfficePlugin(progressstate);
                    if (controlcollection != null)
                    {
                        PropertyControl prop = controlcollection.First();
                        if (prop.ItemDataDictionary.Count != 1)
                        {
                            ContentControl.CreateContent(controlcollection, _connectionCredentialsCaption,
                                CommandTypes.None,
                                ConfigurationViewModel.TenantName, true, true, false);
                            ContentControl.TransistionViewModel.CommandoneclickEvent -= Validateclicked;
                            ContentControl.TransistionViewModel.CommandoneclickEvent += Validateclicked;
                        }
                        else
                        {
                            ClearBackofficeCredentialsData();
                            ConfigurationViewModel.BackOfficeid = prop.ItemDataDictionary.First().Key;
                            //controlcollection = DisplayBackOfficePlugin();
                            controlcollection.First().SelectedValue = ConfigurationViewModel.BackOfficeid;
                            ContentControl.CreateContent(controlcollection, _connectionCredentialsCaption,
                                CommandTypes.None,
                                ConfigurationViewModel.TenantName, true, true, false);
                            LoadBackOfficePlugin(ConfigurationViewModel.BackOfficeid,progressstate);
                            if (!LoadAdminCredentialsAvailable(ConfigurationViewModel.BackOfficeid, progressstate))
                                DisplayConnectionCredentialsDetails(progressstate);
                        }
                    }
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

        private void DisplayConfigurationDetails(PremiseConfigurationRecord premiseConfigurationRecord, string progressstate)
        {
            try
            {

                BackOfficeConnectionCredentials = premiseConfigurationRecord.BackOfficeConnectionCredentials;
                ConfigurationViewModel.BackOfficeCompanyName = premiseConfigurationRecord.BackOfficeCompanyName;
                ConfigurationViewModel.BackOfficeid = premiseConfigurationRecord.ConnectorPluginId;
                ConfigurationViewModel.BackOfficeConnectionCredentialsDescription =
                    premiseConfigurationRecord.BackOfficeConnectionCredentialsDescription;
                ConfigurationViewModel.BackofficeProductName = _premiseConfigurationRecord.BackOfficeProductName;
                Controlcollection controlcollection = new Controlcollection();


                Dictionary<string, string> controlvalues = JsonConvert.DeserializeObject<Dictionary<string, string>>
                    (_premiseConfigurationRecord.BackOfficeConnectionCredentials);
                var response =
                    new ConnectionCredentialsNeededResponse(
                        _premiseConfigurationRecord.BackOfficeConnectionCredentials, controlvalues, new[] { "" },
                        new[] { "" });

                // Both the values to be passed null for Sage 50 get the Company details and name
                //var response1 = _backOfficeServiceManager.GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                //        null, null);

               controlcollection.AddRange(CreateConnectionControlDisplay(response, ConfigurationViewModel.BackOfficeConnectionCredentialsDescription));

                
                ContentControl.CreateContent(controlcollection, _connectionCredentialsCaption, CommandTypes.None,
                    ConfigurationViewModel.TenantName, true, true, true);
                DisplayFeatureRequestConfiguration(true, progressstate);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void EditConfigurationDetails(PremiseConfigurationRecord premiseConfigurationRecord, String progressstate)
        {
            try
            {
                

                Mouse.OverrideCursor = Cursors.Wait;
                BackOfficeConnectionCredentials = premiseConfigurationRecord.BackOfficeConnectionCredentials;
                ConfigurationViewModel.BackOfficeCompanyName = premiseConfigurationRecord.BackOfficeCompanyName;
                ConfigurationViewModel.BackOfficeid = premiseConfigurationRecord.ConnectorPluginId;
                ConfigurationViewModel.BackofficeProductName = _premiseConfigurationRecord.BackOfficeProductName;
                Controlcollection controlcollection = DisplayBackOfficePlugin(progressstate,true);
                TransistionControl transbackoffControl = ContentControl;
                //ManagementCredentialsNeededResponse managementcredentialsresponse =
                //    _backOfficeServiceManager.GetManagementCredentialsNeeded(ConfigurationViewModel.BackOfficeid);
                ManagementCredentialsNeededResponse managementcredentialsresponse =
                new ProgressBarWorker().GetManagementCredentialsNeeded(ConfigurationViewModel.BackOfficeid, progressstate);

                if (managementcredentialsresponse.CurrentValues != null &&
                    managementcredentialsresponse.DescriptionsAsString != "{}" &&
                    managementcredentialsresponse.DescriptionsAsString != "null" && !string.IsNullOrWhiteSpace(managementcredentialsresponse.DescriptionsAsString))
                {
                    controlcollection.AddRange(CreatePropcontrol(managementcredentialsresponse));
                    transbackoffControl.CreateContent(controlcollection, _adminCredentialsCaption,
                        CommandTypes.ValidateOnly,
                        _premiseConfigurationRecord.CloudCompanyName, false, true, false);
                    ContentControl.TransistionViewModel.CommandoneclickEvent -= ValidateAdmincredentials;
                    ContentControl.TransistionViewModel.CommandoneclickEvent += ValidateAdmincredentials;
                }
                else
                {

                    JsonConvert.DeserializeObject<Dictionary<string, string>>
                        (_premiseConfigurationRecord.BackOfficeConnectionCredentials);
                    //var response =
                    //    _backOfficeServiceManager.GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                    //        new Dictionary<string, string>(),
                    //        JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    //            _premiseConfigurationRecord.BackOfficeConnectionCredentials));
                    var response =
                    new ProgressBarWorker().GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                            new Dictionary<string, string>(),
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                _premiseConfigurationRecord.BackOfficeConnectionCredentials), progressstate);
                    if (response.UserFacingMessages.Any())
                    {
                        ShowErrorMessage(response.UserFacingMessages[0].Trim(), _errorCaption);
                        return;
                    }

                    controlcollection.AddRange(CreateConnectionControl(response));
                    Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists =
                   GetFeatureConfigurationProperties(progressstate);
                    CommandTypes commandtype = CommandTypes.None;
                    if (featurePropertyLists.Count == 0)
                    {
                        commandtype = CommandTypes.SaveOnly;
                     
                    }


                    ContentControl.CreateContent(controlcollection, _connectionCredentialsCaption, commandtype,
                        ConfigurationViewModel.TenantName, true, true, false);
                    if (featurePropertyLists.Count == 0)
                    {
                        ContentControl.TransistionViewModel.CommandoneclickEvent -= SaveConnection;
                        ContentControl.TransistionViewModel.CommandoneclickEvent += SaveConnection;
                    }
                    if (featurePropertyLists.Count != 0) DisplayFeatureRequestConfiguration(false, progressstate);
                }



            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }

            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ValidateAdmincredentials(object sender, EventArgs e)
        {
            try
            {
                string progressstate = CustomerFacingMessages.Progressbar_Validatingcredentials;

                Mouse.OverrideCursor = Cursors.Wait;
                Controlcollection controlcollection = DisplayBackOfficePlugin(progressstate,true);
                TransistionControlViewModel trans = (TransistionControlViewModel)sender;
                SetBackOfficeCredentials(trans.ControlValues);

                //var response =
                //    _backOfficeServiceManager.GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                //        BackOfficeAdminCredentials,
                //      JsonConvert.DeserializeObject<Dictionary<string, string>>(
                //                _premiseConfigurationRecord.BackOfficeConnectionCredentials));
                var response =
                new ProgressBarWorker().GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                        BackOfficeAdminCredentials,
                      JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                _premiseConfigurationRecord.BackOfficeConnectionCredentials), progressstate);

                if (response.UserFacingMessages.Any())
                {
                    ShowErrorMessage(response.UserFacingMessages[0].Trim(), _errorCaption);
                    return;
                }

                controlcollection.AddRange(CreateConnectionControl(response));


                ContentControl.CreateContent(controlcollection, _connectionCredentialsCaption, CommandTypes.None,
                    ConfigurationViewModel.TenantName, true, true, false);
                DisplayFeatureRequestConfiguration(false, progressstate);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }


        private Controlcollection DisplayBackOfficePlugin(string progressstate,bool readonblydisplay = false)
        {

            //BackOfficePluginsResponse backOfficePluginsResponse = _backOfficeServiceManager.GetBackOfficePlugins();
            BackOfficePluginsResponse backOfficePluginsResponse = new ProgressBarWorker().GetBackOfficePlugins(progressstate);
            if (backOfficePluginsResponse != null)
            {
                BackOfficePlugins = backOfficePluginsResponse.BackOfficePlugins;
                var itemdata = Plugindata(backOfficePluginsResponse.BackOfficePlugins);
                
                PropertyControl p = new PropertyControl
                {
                    Name = BackOfficeName,
                    DisplayName = _backOfficeCaption,
                    DataType = DataTypeEnum.StringType,
                    SelectionType = SelectTypes.Lookup,
                    SelectedValue = ConfigurationViewModel.BackOfficeid,
                    IsDisabled = (readonblydisplay || (itemdata.Count() == 1)),
                    ItemDataDictionary = itemdata,
                    CommandAction = Validateclicked
                };
                Controlcollection controlcollection = new Controlcollection { p };

                return controlcollection;
            }
            return null;
        }

        private void ClearBackofficeCredentialsData()
        {
            BackOfficeConnectionCredentials = string.Empty;
            _premiseConfigurationRecord.BackOfficeConnectionCredentials = string.Empty;
            if (BackOfficeAdminCredentials != null) BackOfficeAdminCredentials.Clear();
        }
        private void Validateclicked(object sender, EventArgs e)
        {
            string progressstate = CustomerFacingMessages.Progressbar_Validatingcredentials;
            Mouse.OverrideCursor = Cursors.Wait;
            ClearBackofficeCredentialsData();
            TransistionControlViewModel trans = (TransistionControlViewModel)sender;
            string backoffvalue;
            trans.ControlValues.TryGetValue(BackOfficeName, out backoffvalue);
            ConfigurationViewModel.BackOfficeid = backoffvalue;

            LoadBackOfficePlugin(ConfigurationViewModel.BackOfficeid, progressstate);
            bool admincredentialsavailable = LoadAdminCredentialsAvailable(ConfigurationViewModel.BackOfficeid, progressstate);
            if (!admincredentialsavailable)
                DisplayConnectionCredentialsDetails(progressstate);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void LoadBackOfficePlugin(String backofficeid, string progressstate)
        {
            Mouse.SetCursor(Cursors.Wait);

            BackOfficePlugin backOfficePlugin = GetBackoffPluginSelected(backofficeid);

            if (backOfficePlugin != null)
            {
                ConfigurationViewModel.BackofficeProductName = backOfficePlugin.BackofficeProductName;
                new ProgressBarWorker().DownloadBackOfficePlugin(backOfficePlugin, progressstate);
                //ConfigurationHelpers.DownloadBackOfficePlugin(backOfficePlugin.PluginId,
                //    backOfficePlugin.BackOfficePluginAutoUpdateProductId,
                //    backOfficePlugin.BackOfficePluginAutoUpdateProductVersion,
                //    backOfficePlugin.BackOfficePluginAutoUpdateComponentBaseName);
            }
            Mouse.SetCursor(Cursors.Arrow);

        }

        private bool LoadAdminCredentialsAvailable(String backofficeid,string progressstate)
        {
            //ManagementCredentialsNeededResponse response =
            //    _backOfficeServiceManager.GetManagementCredentialsNeeded(backofficeid);

            ManagementCredentialsNeededResponse response = new ProgressBarWorker().GetManagementCredentialsNeeded(backofficeid,progressstate);

            if (response.CurrentValues != null && response.DescriptionsAsString != null &&
                String.CompareOrdinal(response.DescriptionsAsString, "null") != 0 && String.CompareOrdinal(response.DescriptionsAsString, "{}") != 0)
            {

                Controlcollection controlcollection = CreatePropcontrol(response);

                if (controlcollection != null)
                {
                    TransistionControl transbackoffControl = ContentControl;
                    transbackoffControl.CreateContent(controlcollection, _adminCredentialsCaption,
                        CommandTypes.ValidateOnly,
                        ConfigurationViewModel.TenantName, false, false, false);
                    ContentControl.TransistionViewModel.CommandoneclickEvent -= ValidateManagementCredentials;
                    ContentControl.TransistionViewModel.CommandoneclickEvent += ValidateManagementCredentials;
                }
            }
            else if (response.RawErrorMessage.Any())
            {
                ShowErrorMessage(response.UserFacingMessages[0].Trim(), _errorCaption);

            }
            else
            {
                return false;
            }
            return true;
        }




        /// <summary>
        /// 
        /// </summary>
        public Controlcollection CreatePropcontrol(ManagementCredentialsNeededResponse response)
        {
            Controlcollection controlcollection = new Controlcollection();
            BackOfficeAdminCredentials = response.CurrentValues;


            var jDesc = JObject.Parse(response.DescriptionsAsString);
            IDictionary<string, string> currentValues = response.CurrentValues;
            foreach (var item in jDesc.Properties())
            {
                var description = jDesc[item.Name];

                if (!description.HasValues)
                    return controlcollection;

                bool? isPassword = (bool?)description["IsPassword"];

                PropertyControl p = new PropertyControl
                {
                    Name = item.Name,
                    DisplayName = (string)description["DisplayName"] + ":",
                    DisplayValue = currentValues[item.Name],
                    IsPassword = isPassword.HasValue,

                    DataType = DataTypeEnum.StringType,
                    SelectionType = SelectTypes.None,
                };
                controlcollection.Add(p);
            }

            return controlcollection;
        }

        private void ValidateManagementCredentials(object sender, EventArgs e)
        {
            try
            {
                string progressstate = CustomerFacingMessages.Progressbar_Validatingcredentials;
                Mouse.OverrideCursor = Cursors.Wait;
                TransistionControlViewModel trans = (TransistionControlViewModel)sender;
                SetBackOfficeCredentials(trans.ControlValues);

                //ValidateBackOfficeAdminCredentialsResponse response =
                //    _backOfficeServiceManager.ValidateBackOfficeAdminCredentials(ConfigurationViewModel.BackOfficeid,
                //        BackOfficeAdminCredentials);
                ValidateBackOfficeAdminCredentialsResponse response =
                    new ProgressBarWorker().ValidateBackOfficeAdminCredentials(ConfigurationViewModel.BackOfficeid,
                        BackOfficeAdminCredentials, progressstate);
                if (response.IsValid)
                {
                    string credentialsAsJson = JsonConvert.SerializeObject(BackOfficeAdminCredentials);
                    BackOfficeConnectionCredentials = credentialsAsJson;
                    _premiseConfigurationRecord.BackOfficeConnectionCredentials = BackOfficeConnectionCredentials;
                    //Passed both backofficecredentials need to check the out put if different
                    DisplayConnectionCredentialsDetails(progressstate);
                }
                else if (response.RawErrorMessage.Any())
                {
                    ShowErrorMessage(response.UserFacingMessages[0].Trim(), _errorCaption);
                }
            }
            catch (Exception ex)
            {

                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }


        private void DisplayConnectionCredentialsDetails(string progressstate)
        {
            try
            {
                var response =
                    new ProgressBarWorker().GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                        BackOfficeAdminCredentials, null, progressstate);
                    //_backOfficeServiceManager.GetConnectionCredentialsNeeded(ConfigurationViewModel.BackOfficeid,
                    //    BackOfficeAdminCredentials, null);


                if (!response.UserFacingMessages.Any() && response.CurrentValues != null && response.DescriptionsAsString != null &&
                String.CompareOrdinal(response.DescriptionsAsString, "null") != 0 && String.CompareOrdinal(response.DescriptionsAsString, "{}") != 0)
                {
                    Controlcollection controlcollection = CreateConnectionControl(response);
                    if (controlcollection != null)
                    {
                        TransistionControl transbackoffControl = ContentControl;
                        transbackoffControl.CreateContent(controlcollection, _connectionCredentialsCaption,
                            CommandTypes.SubmitOnly, ConfigurationViewModel.TenantName, false, false, false);
                        ContentControl.TransistionViewModel.CommandoneclickEvent -= TestBackOfficeConnection;
                        ContentControl.TransistionViewModel.CommandoneclickEvent += TestBackOfficeConnection;
                    }
                }
                else if (response.UserFacingMessages.Any())
                {
                    ShowErrorMessage(response.UserFacingMessages[0].Trim(), _errorCaption);
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public Controlcollection CreateConnectionControl(ConnectionCredentialsNeededResponse response)
        {
            CompanyselectedEvent = null;
            Controlcollection controlcollection = new Controlcollection();
            try
            {
                ConfigurationViewModel.BackOfficeConnectionCredentialsDescription = response.DescriptionsAsString;
                var jDesc = JObject.Parse(response.DescriptionsAsString);
                IDictionary<string, string> currentValues = response.CurrentValues;
                controlcollection.AddRange(from item in jDesc.Properties()
                    let description = jDesc[item.Name]
                                           let isPassword = (bool?)description["IsPassword"]
                    let isCompanyId = (String.Equals(item.Name, "CompanyId", StringComparison.CurrentCultureIgnoreCase) || 
                                           String.Equals(item.Name, "Companycode", StringComparison.CurrentCultureIgnoreCase))
                                           let isPath = (bool?)description["IsPath"]
                    let treatAsList = (description.Value<object>("ValueName") != null)
                    
                
                    select new PropertyControl
                    {
                        Name = item.Name,
                        DisplayName = (string)description["DisplayName"] + ":",
                        DisplayValue = (currentValues[item.Name]),
                        SelectedValue = (currentValues[item.Name]),
                        IsPassword = isPassword.HasValue,
                        IsPath = isPath.HasValue,
                        DataType = DataTypeEnum.StringType,
                        CommandAction = (isCompanyId?ReLoadFeatureConfiguration:EmptyMethod),
                        SelectionType = (!treatAsList ? SelectTypes.None : SelectTypes.Lookup),
                        ItemDataDictionary =
                            (!treatAsList ? null : ConnectionListDictionary(response.DescriptionsAsString, item.Name)),
                            
                    }
                    
                    );
               
                return controlcollection;
            }
            catch (Exception ex)
            {

                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                return null;
            }

        }

        /// <summary>
        /// Creating empty method to do nothing
        /// </summary>
        private EventHandler EmptyMethod { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Controlcollection CreateConnectionControlDisplay(ConnectionCredentialsNeededResponse response,  string descriptionsAsString = "")
        {
            Controlcollection controlcollection = new Controlcollection();
            try
            {
                var jDesc = JObject.Parse(descriptionsAsString == "" ? response.DescriptionsAsString : descriptionsAsString);
                IDictionary<string, string> currentValues = response.CurrentValues;
                string keyValue = ConfigurationViewModel.BackofficeProductName;
                controlcollection.Add(new PropertyControl
                {
                    Name = "Backoffice",
                    DisplayName = CustomerFacingMessages.BackOfficeCaption,
                    DisplayValue = keyValue,
                    DataType = DataTypeEnum.StringType,
                });

                if (!string.IsNullOrEmpty(descriptionsAsString))
                {
                    controlcollection.AddRange(from item in jDesc.Properties()
                                               let description = jDesc[item.Name]
                                               let isPassword = (bool?)description["IsPassword"]
                                               let isCompanyId = (String.Equals(item.Name, "CompanyId", StringComparison.CurrentCultureIgnoreCase))
                                               let isPath = (bool?)description["IsPath"]
                                               let treatAsList = (description.Value<object>("ValueName") != null)

                                               select new PropertyControl
                                               {
                                                   Name = item.Name,
                                                   DisplayName = (string)description["DisplayName"] + ":",
                                                   DisplayValue = (currentValues[item.Name]),
                                                   // SelectedValue = (!isCompanyId ? currentValues[item.Name] : ConfigurationViewModel.BackOfficeCompanyName),
                                                   SelectedValue = (currentValues[item.Name]),
                                                   IsPassword = isPassword.HasValue,
                                                   IsPath = isPath.HasValue,
                                                   DataType = DataTypeEnum.StringType,
                                                   SelectionType = (!treatAsList ? SelectTypes.None : SelectTypes.Lookup),
                                                   ItemDataDictionary = (!treatAsList ? null : ConnectionListDictionary(descriptionsAsString, item.Name)),
                                               });
                    return controlcollection;
                }
                foreach (var currentValue in currentValues)
                {
                    if (currentValue.Key == "Backoffice" || currentValue.Key == "DummyBackOfficeSpecialValue")
                        continue;
                   
                   controlcollection.Add(new PropertyControl
                                           {
                                               Name = currentValue.Key,
                                               DisplayName = currentValue.Key + ":",
                                               DisplayValue = currentValue.Value,
                                               IsPassword = (String.Equals(currentValue.Key, "Password", StringComparison.CurrentCultureIgnoreCase)),
                                               IsPath = (String.Equals(currentValue.Key, "Path", StringComparison.CurrentCultureIgnoreCase)),
                                               DataType = DataTypeEnum.StringType,
                                           });
                }

                return controlcollection;
            }
            catch (Exception ex)
            {

                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                return null;
            }

        }

        private Dictionary<string, string> ConnectionListDictionary(string controlDescriptionsAsString, string itemname)
        {
            var jDesc = JObject.Parse(controlDescriptionsAsString);
            var description = jDesc[itemname];
            IList<string> emptyList = new List<string>();

            bool hasName = description["ValueName"] != null;
            bool hasId = (jDesc[itemname].Value<object>("ValueId") != null);


            //assumes that all are same size of others are null. ok for now but value up checking 
            IList<string> names = (hasName ? description["ValueName"].ToObject<IList<string>>() : emptyList);
            IList<string> ids = (hasId ? description["ValueId"].ToObject<IList<string>>() : names);
            // IList<string> descriptions = (hasDescription ? description["ValueDescription"].ToObject<IList<string>>() : names);

            //we finally know all list parts are here, are IList<string> and not empty and the same length. Finally time to make a connectionlist
            int count = names.Count;
            Dictionary<string, string> listDictionary = new Dictionary<string, string>();
            for (int j = 0; j < count; j++)
            {
                listDictionary.Add(ids[j], names[j]);

                //ListItem listItem = new ListItem
                //{
                //    Name = names[j],
                //    Id = ids[j],
                //    Description = descriptions[j],

                //};

            }
            return listDictionary;
        }


        private void TestBackOfficeConnection(object sender, EventArgs e)
        {
            try
            {
                string progressstate = CustomerFacingMessages.Progressbar_Validatingcredentials;
                Mouse.OverrideCursor = Cursors.Wait;
                TransistionControlViewModel trans = (TransistionControlViewModel)sender;
                SetBackOfficeCredentials(trans.ControlValues);
                string backOfficeAdminCredentialsasString = JsonConvert.SerializeObject(BackOfficeAdminCredentials);
                if (ValidateBackOfficeConnection(backOfficeAdminCredentialsasString, progressstate))
                {
                    if (!DisplayFeatureRequestConfiguration(false, progressstate))
                        SaveConnection(null, null);
                }
            }
            catch (Exception ex)
            {

                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ReLoadFeatureConfiguration(object sender, EventArgs e)
        {
            try
            {
                string progressstate = CustomerFacingMessages.Progressbar_Refreshtransaction;
                Mouse.OverrideCursor = Cursors.Wait;
                if (CompanyselectedEvent == null) return;
                TransistionControlViewModel trans = (TransistionControlViewModel)sender;
                SetBackOfficeCredentials(trans.ControlValues);
                string backOfficeAdminCredentialsasString = JsonConvert.SerializeObject(BackOfficeAdminCredentials);
                if (!ValidateBackOfficeConnection(backOfficeAdminCredentialsasString, progressstate)) return;
                Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists =
                    GetFeatureConfigurationProperties(progressstate);
                if (featurePropertyLists == null || featurePropertyLists.Count == 0) return;
                CompanyselectedEvent = ReLoadFeatureConfiguration;
                ReloadfeatureConfiguration(featurePropertyLists, progressstate);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CustomerFacingMessages.ErrorMessage_ErrorwhenlistingFeatureConfguration + ex.Message,
                    _errorCaption);
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

        private void ReloadfeatureConfiguration(Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists, string progressstate)
        {
            if (featurePropertyLists != null && featurePropertyLists.Count != 0)
            {

                IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> featurePropertyEntryValues =
                    GetFeaturePropertyEntryValues(featurePropertyLists, progressstate);
                IDictionary<String, Dictionary<string, object>> featurePropertyValuePairs =
                    GetCompanyFeaturePropertyValuePairs(featurePropertyLists, progressstate);
                TransistionControl transbackoffControl = ContentControl;
                transbackoffControl.ReloadFeatureContent(featurePropertyLists, featurePropertyEntryValues,
                    featurePropertyValuePairs, (ShowEditSetting == Visibility.Visible ? CommandTypes.SaveOnly : CommandTypes.RegisterOnly),
                    false);
                ContentControl.TransistionViewModel.CommandoneclickEvent -= SaveConnection;
                ContentControl.TransistionViewModel.CommandoneclickEvent += SaveConnection;
                // Assiging the back office connection so when an company is selected the feature control is loaded again;
               

            }
            
        }
        private bool ValidateBackOfficeConnection(string backOfficeAdminCredentialsasString,string progressstate)
        {


            
            //ValidateBackOfficeConnectionResponse response =
            //    _backOfficeServiceManager.ValidateBackOfficeConnectionCredentialsAsString(
            //        ConfigurationViewModel.BackOfficeid, backOfficeAdminCredentialsasString);
            ValidateBackOfficeConnectionResponse response =
            new ProgressBarWorker().ValidateBackOfficeConnectionCredentialsAsString(
                    ConfigurationViewModel.BackOfficeid, backOfficeAdminCredentialsasString, progressstate);
            if (response.RawErrorMessage.Any())
            {
                ShowErrorMessage(response.UserFacingMessages[0].Trim(), _errorCaption);
                return false;
            }
            BackOfficeConnectionCredentials = JsonConvert.SerializeObject(response.ConnectionCredentials);
            ConfigurationViewModel.BackOfficeCompanyName = response.CompanyNameForDisplay;

            return !ValidateTenantCompanyDuplication(response.CompanyNameForDisplay);
        }


        private bool DisplayFeatureRequestConfiguration(bool readOnly,string progressstate)
        {
            try
            {
                Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists =
                    GetFeatureConfigurationProperties(progressstate);
                if (featurePropertyLists != null && featurePropertyLists.Count != 0)
                {
                    CreateFeatureControl(featurePropertyLists, readOnly, progressstate);
                    return true;
                }
                if (featurePropertyLists != null && featurePropertyLists.Count == 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CustomerFacingMessages.ErrorMessage_ErrorwhenlistingFeatureConfguration + ex.Message, _errorCaption);
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return false;
        }

        private Dictionary<FeatureMetadata, IList<PropertyDefinition>> GetFeatureConfigurationProperties(string progressstate)
        {
            //FeatureResponse featureResponse =
            //    _featureServiceManager.GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
            //        BackOfficeConnectionCredentials, FeatureMessageTypes.GetFeatureConfigurationProperties,
            //        ConfigurationViewModel.TenantGuid.ToString(), "");

            FeatureResponse featureResponse = new ProgressBarWorker().GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
                    BackOfficeConnectionCredentials, FeatureMessageTypes.GetFeatureConfigurationProperties,
                    ConfigurationViewModel.TenantGuid.ToString(), "", progressstate);
            if (featureResponse.RawErrorMessage.Any() || featureResponse.UserFacingMessages.Any())
            {
                ShowErrorMessage(featureResponse.UserFacingMessages[0].Trim(), _errorCaption);
                return null;
            }
            if (featureResponse.Payload == null)
            {
                throw new Exception(CustomerFacingMessages.ErrorMessage_FeatureConfigurationNull);
            }
            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new AbstractDataTypeConverter());
            cfg.Converters.Add(new AbstractSelectionValueTypesConverter());
            var featurePropDefsList =
                JsonConvert.DeserializeObject<IList<KeyValuePair<FeatureMetadata, IList<PropertyDefinition>>>>(
                    featureResponse.Payload, cfg);


            _metaPropertyLists = (featurePropDefsList != null)
                ? featurePropDefsList.ToDictionary(x => x.Key, y => y.Value)
                : new Dictionary<FeatureMetadata, IList<PropertyDefinition>>();
            return _metaPropertyLists;

        }

        private IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> GetFeaturePropertyEntryValues(
            IEnumerable<KeyValuePair<FeatureMetadata, IList<PropertyDefinition>>> featurePropertyLists,string progressstate)
        {


            IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> featurePropertyEntryValues;


            var featurePropSetups = new Dictionary<string, Dictionary<String, AbstractSelectionValueTypes>>();
            foreach (var featureSet in featurePropertyLists)
            {
                var propDefinitions = (List<PropertyDefinition>)featureSet.Value;
                var propSetupDictionary = new Dictionary<String, AbstractSelectionValueTypes>();
                foreach (var propDef in propDefinitions)
                {
                    if (propDef.PropertyDataType.SelectionType.Equals(SelectionTypes.List))
                    {
                        propSetupDictionary.Add(propDef.PropertyName, new ListTypeValues());
                        continue;
                    }
                    if (propDef.PropertyDataType.SelectionType.Equals(SelectionTypes.Lookup))
                    {
                        propSetupDictionary.Add(propDef.PropertyName, new LookupTypeValues());
                    }

                    //other types unsupported for backoffice setup at this time. 
                }
                featurePropSetups.Add(featureSet.Key.Name, propSetupDictionary);
            }

            if (!featurePropSetups.Any())
            {
                featurePropertyEntryValues = new Dictionary<string, Dictionary<string, AbstractSelectionValueTypes>>();
                return featurePropertyEntryValues;
            }

            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());

            string setupFeatureConfigPayload = JsonConvert.SerializeObject(featurePropSetups, cfg);

            cfg.Converters.Add(new AbstractSelectionValueTypesConverter());
            //var featureResponse = _featureServiceManager.GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
            //    BackOfficeConnectionCredentials, FeatureMessageTypes.SetupCompanyFeaturePropertySelectionValues,
            //    ConfigurationViewModel.TenantGuid.ToString(), setupFeatureConfigPayload);

             var featureResponse = new ProgressBarWorker().GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
                BackOfficeConnectionCredentials, FeatureMessageTypes.SetupCompanyFeaturePropertySelectionValues,
                ConfigurationViewModel.TenantGuid.ToString(), setupFeatureConfigPayload, progressstate);


            cfg = new DomainMediatorJsonSerializerSettings { ContractResolver = new DictionaryFriendlyContractResolver() };
            cfg.Converters.Add(new KeyValuePairConverter());
            cfg.Converters.Add(new AbstractDataTypeConverter());
            cfg.Converters.Add(new AbstractSelectionValueTypesConverter());
            var featurePropEntryValues = JsonConvert.DeserializeObject<IList<KeyValuePair<string,
                IList<KeyValuePair<String, AbstractSelectionValueTypes>>>>>(featureResponse.Payload, cfg);

            Dictionary<string, Dictionary<string, AbstractSelectionValueTypes>> test =
                featurePropEntryValues.ToDictionary(x => x.Key, y => y.Value.ToDictionary(a => a.Key, b => b.Value));
            featurePropertyEntryValues = test;
            return featurePropertyEntryValues;
        }

        private Dictionary<String, Dictionary<string, object>> GetCompanyFeaturePropertyValuePairs(
            IEnumerable<KeyValuePair<FeatureMetadata, IList<PropertyDefinition>>> featurePropertyLists, string progressstate)
        {
            var featureListPayload = JsonConvert.SerializeObject(from f in featurePropertyLists select f.Key.Name,
                new DomainMediatorJsonSerializerSettings());
            //var featureResponse = _featureServiceManager.GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
            //    BackOfficeConnectionCredentials, FeatureMessageTypes.GetCompanyFeaturePropertyValuePairs,
            //    ConfigurationViewModel.TenantGuid.ToString(), featureListPayload);
            var featureResponse = new ProgressBarWorker().GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
                BackOfficeConnectionCredentials, FeatureMessageTypes.GetCompanyFeaturePropertyValuePairs,
                ConfigurationViewModel.TenantGuid.ToString(), featureListPayload, progressstate);
            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());

            var currentValues =
                JsonConvert.DeserializeObject<IList<KeyValuePair<String, IList<KeyValuePair<string, object>>>>>(
                    featureResponse.Payload, cfg);
            return currentValues.ToDictionary(x => x.Key, y => y.Value.ToDictionary(a => a.Key, b => b.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateFeatureControl(Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists, bool readOnly, string progressstate)
        {
            try
            {

                if (featurePropertyLists != null && featurePropertyLists.Count!=0 )
                {

                IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> featurePropertyEntryValues =
                    GetFeaturePropertyEntryValues(featurePropertyLists, progressstate);
                IDictionary<String, Dictionary<string, object>> featurePropertyValuePairs =
                    GetCompanyFeaturePropertyValuePairs(featurePropertyLists, progressstate);
                TransistionControl transbackoffControl = ContentControl;
                transbackoffControl.CreateFeatureContent(featurePropertyLists, featurePropertyEntryValues,
                    featurePropertyValuePairs, (ShowEditSetting == Visibility.Visible ? CommandTypes.SaveOnly : CommandTypes.RegisterOnly),
                    readOnly);
                ContentControl.TransistionViewModel.CommandoneclickEvent -= SaveConnection;
                    ContentControl.TransistionViewModel.CommandoneclickEvent += SaveConnection;
                    // Assiging the back office connection so when an company is selected the feature control is loaded again;
                    CompanyselectedEvent = ReLoadFeatureConfiguration;
                    
                }
                
            }
            catch (Exception ex)
            {
                ShowErrorMessage(CustomerFacingMessages.ErrorMessage_ErrorwhenlistingFeatureConfguration + ex.Message, _errorCaption);
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private bool SaveFeatureConfiguration(Dictionary<string, IDictionary<string, object>> featureSaveValues, string progressstate)
        {

            if (featureSaveValues != null)
            {

                Dictionary<string, ICollection<PropertyValuePairValidationResponse>> validationResponses =
                    ValidateFeaturePropertyValuePairs(featureSaveValues,progressstate);
                if (ContentControl.TransistionViewModel.ValidateFeatureConfigurationvalues(validationResponses))
                {
                    var response = SaveFeaturePropertyValuePairs(featureSaveValues, progressstate);
                    if (response.Status.Equals(Status.Failure))
                    {
                        //_canClose = false;
                        string msg = @"Unexpected error. Entry values could not be saved.";
                        if (response.Diagnoses.Any())
                            msg = response.Diagnoses[0].UserFacingMessage;
                        //dislay Error Message and cancel the close
                        ShowErrorMessage(msg, _errorCaption);
                        return false;

                    }
                    return true;
                }
                ShowErrorMessage(CustomerFacingMessages.ErrorMessage_FeatureConfigValuesMissing, CustomerFacingMessages.ErrorMessage_FeatureConfivalueMisssing_Caption, HelpLinkManager.FindString("ErrorMessage_FeatureConfigValueMissing_HelpUri"));
                return false;
            }
            if (featureSaveValues == null)
            {
                // We are returning true as ther will be atleast one value will be available if the back office displays 
                // the feature configuration. if the back office does not have any feature configuration
                // available to configure featuresavevalues will be null.
                return true;
            }
            return false;
        }


        private void SaveConnection(object sender, EventArgs e)
        {
            try
            {

                string progressstate = CustomerFacingMessages.Progressbar_Savingconnection;
                Mouse.OverrideCursor = Cursors.Wait;
                Dictionary<string,IDictionary<string,object>> featuresavevalues=null;
                if (sender != null)
                {
                    TransistionControlViewModel trans = (TransistionControlViewModel)sender;
                    SetBackOfficeCredentials(trans.ControlValues);
                    featuresavevalues=  trans.FeatureSaveValues;
                }

                string backOfficeAdminCredentialsasString = JsonConvert.SerializeObject(BackOfficeAdminCredentials);
                if (ValidateBackOfficeConnection(backOfficeAdminCredentialsasString, progressstate))
                {
                    if (featuresavevalues != null && !SaveFeatureConfiguration(featuresavevalues, progressstate))
                        return;
                    if (RegisterConnection(progressstate))
                    {
                        ShowEditSetting = Visibility.Visible;
                        CompanyselectedEvent = null;
                        DisplayConfigurationDetails(
                            ConfigurationHelpers.RetrieveTenantConfiguration(
                                ConfigurationViewModel.TenantGuid.ToString()), progressstate);
                        ShowSaveMessage(CustomerFacingMessages.InfoMessage_ConnectionSaved, CustomerFacingMessages.InfoCaption_ConnectionSaved);
                    }
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


        private bool ValidateCompanyData(string companyName)
        {
           
            if (companyName != _premiseConfigurationRecord.BackOfficeCompanyName
                && _premiseConfigurationRecord.BackOfficeCompanyName != "")
            {
                ShowErrorMessage(CustomerFacingMessages.ErrorMessage_DifferenentCompanySelectedonEdit, _errorCaption);
                return true;
            }
            return ValidateTenantCompanyDuplication(companyName);
        }

        private bool ValidateTenantCompanyDuplication(string companyName)
        {
            IEnumerable<PremiseConfigurationRecord> tenantlist = ConfigurationHelpers.GetAllTenantConfigurations();
            foreach (PremiseConfigurationRecord tenant in tenantlist)
            {
                if (tenant.BackOfficeCompanyName == companyName &&
                     ConfigurationViewModel.TenantName != tenant.CloudCompanyName)
                {
                    ShowErrorMessage(CustomerFacingMessages.ErrorMessage_CompanyRegisteredtodifferentTenant.Replace("{site/us}",tenant.CloudCompanyName), _errorCaption);
                    return true;
                }
            }
            return false;
        }
        
        private Dictionary<string, ICollection<PropertyValuePairValidationResponse>> ValidateFeaturePropertyValuePairs(
            Dictionary<string, IDictionary<string, object>> validateFeaturePropValuePairs, string progressstate)
        {
            var cfg = new DomainMediatorJsonSerializerSettings();
            cfg.ContractResolver = new DictionaryFriendlyContractResolver();
            cfg.Converters.Add(new KeyValuePairConverter());

            string validateFeatureConfigPayload = JsonConvert.SerializeObject(validateFeaturePropValuePairs, cfg);

            //var featureResponse = _featureServiceManager.GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
            //    BackOfficeConnectionCredentials, FeatureMessageTypes.ValidateCompanyFeaturePropertyValuePairs,
            //    ConfigurationViewModel.TenantGuid.ToString(), validateFeatureConfigPayload);

            var featureResponse = new ProgressBarWorker().GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
                BackOfficeConnectionCredentials, FeatureMessageTypes.ValidateCompanyFeaturePropertyValuePairs,
                ConfigurationViewModel.TenantGuid.ToString(), validateFeatureConfigPayload, progressstate);

            var response =
                JsonConvert
                    .DeserializeObject<IList<KeyValuePair<String, ICollection<PropertyValuePairValidationResponse>>>>(
                        featureResponse.Payload, cfg);
            if (response == null)
                throw new ApplicationException("Invalid response returned from validation");

            return response.ToDictionary(x => x.Key, y => y.Value);
        }

        private Response SaveFeaturePropertyValuePairs(
            Dictionary<string, IDictionary<string, object>> savePropValuePairs,string progressstate)
        {
            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());

            string saveFeatureConfigPayload = JsonConvert.SerializeObject(savePropValuePairs, cfg);
            //var featureResponse = _featureServiceManager.GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
            //    BackOfficeConnectionCredentials, FeatureMessageTypes.SaveCompanyFeaturePropertyValuePairs,
            //    ConfigurationViewModel.TenantGuid.ToString(), saveFeatureConfigPayload);
            var featureResponse =  new ProgressBarWorker().GetFeatureResponse(ConfigurationViewModel.BackOfficeid,
                BackOfficeConnectionCredentials, FeatureMessageTypes.SaveCompanyFeaturePropertyValuePairs,
                ConfigurationViewModel.TenantGuid.ToString(), saveFeatureConfigPayload, progressstate);

            var response = JsonConvert.DeserializeObject<Response>(featureResponse.Payload, cfg);
            if (response == null)
                throw new ApplicationException("Invalid response returned from save feature entry values");

            return response;
        }

        private bool RegisterConnection(string progressstate)
        {
            try
            {
                using (new TransactionScope())
                {
                    //ValidateBackOfficeConnectionResponse response =
                    //    _backOfficeServiceManager.ValidateBackOfficeConnectionCredentialsAsString(
                    //        ConfigurationViewModel.BackOfficeid, BackOfficeConnectionCredentials);
                    ValidateBackOfficeConnectionResponse response =
                    new ProgressBarWorker().ValidateBackOfficeConnectionCredentialsAsString(
                            ConfigurationViewModel.BackOfficeid, BackOfficeConnectionCredentials, progressstate);

                    if (response.RawErrorMessage.Any())
                    {
                        ShowErrorMessage(response.UserFacingMessages[0], _errorCaption);
                        return false;
                    }
                    if (ValidateCompanyData(response.CompanyNameForDisplay))
                        return false;

                    _premiseConfigurationRecord.BackOfficeCompanyName = response.CompanyNameForDisplay;
                    _premiseConfigurationRecord.BackOfficeCompanyUniqueId = response.CompanyUniqueIndentifier;
                    if (response.ConnectionCredentials != null && response.ConnectionCredentials.Count > 0)
                        BackOfficeConnectionCredentials = JsonConvert.SerializeObject(response.ConnectionCredentials);

                    _premiseConfigurationRecord.BackOfficeConnectionCredentials = BackOfficeConnectionCredentials;

                    _premiseConfigurationRecord.ConnectorPluginId = ConfigurationViewModel.BackOfficeid;
                    _premiseConfigurationRecord.BackOfficeProductName = ConfigurationViewModel.BackofficeProductName;
                    _premiseConfigurationRecord.BackOfficeConnectionCredentialsDescription =
                        ConfigurationViewModel.BackOfficeConnectionCredentialsDescription;
                    _premiseConfigurationRecord.SiteAddress = ApplicationHelpers.ConnectorServiceUri.ToString();
                    _premiseConfigurationRecord.CloudCompanyName = ConfigurationViewModel.TenantName;
                    RegistrationResult registrationResult =
                        ConfigurationHelpers.RegisterConnection(
                            ApplicationHelpers.ConnectorServiceUri,
                            ConfigurationViewModel.TenantGuid.ToString(),
                            response.CompanyUniqueIndentifier,
                            ApplicationHelpers.SageIdToken);


                    if (registrationResult.Successful)
                    {
                        _premiseConfigurationRecord.CloudTenantClaim = registrationResult.TenantClaim;
                        var tenantresponse =
                            ConfigurationHelpers.ValidateTenantConnection(
                                registrationResult.SiteAddressBaseUri.ToString(),
                                registrationResult.TenantId, registrationResult.TenantKey,
                                registrationResult.TenantClaim);

                        if (tenantresponse.Success)
                        {
                            _premiseConfigurationRecord.CloudTenantId = registrationResult.TenantId;
                            _premiseConfigurationRecord.CloudCompanyUrl = registrationResult.TenantUrl.ToString();
                            _premiseConfigurationRecord.CloudPremiseKey = registrationResult.TenantKey;

                        }
                        else
                        {
                            ConfigurationViewModel.ConnectionStatus = ConnectionState.Error;
                            ConfigurationViewModel.CloudStatus = ConnectionState.Error;
                            ShowErrorMessage(CustomerFacingMessages.ErrorMessage_Connectionsavefailed + tenantresponse.UserFacingError, _errorCaption);
                            return false;
                        }
                        if (ConfigurationHelpers.SaveTenantConfiguration(_premiseConfigurationRecord))
                        {
                            var changeType = Convert.ChangeType(response.BackOfficeConnectivityStatus,
                                response.BackOfficeConnectivityStatus.GetTypeCode());
                            if (changeType != null)
                                SetConnectionStatus((tenantresponse.Success
                                    ? ConnectionState.OnLine
                                    : ConnectionState.OffLine),
                                    (BackOfficeConnectivityStatus)
                                        changeType,
                                    ConnectionState.OnLine);
                           
                            return true;
                        }
                    }
                    else
                    {
                        ShowErrorMessage(CustomerFacingMessages.ErrorMessage_CloudRegistratoinFailed, _errorCaption, HelpLinkManager.FindString("ErrorMessage_CloudRegistratoinFailed_HelpUri"));
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {

                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return false;
        }

        private Dictionary<string, string>  Plugindata(BackOfficePlugin[] backOfficePlugins)
        {
            Dictionary<string, string> itemdata = new Dictionary<string, string>();
            for (int i = 0; i < backOfficePlugins.Count(); i++)
            {
                if (backOfficePlugins[i].BackOfficeVersion != null &&
                    backOfficePlugins[i].BackOfficeVersion.Trim() != "" &&
                    backOfficePlugins[i].BackOfficeVersion != string.Empty &&
                    !itemdata.ContainsKey(backOfficePlugins[i].PluginId))
                    itemdata.Add(backOfficePlugins[i].PluginId, backOfficePlugins[i].BackofficeProductName);
            }
            return itemdata;
        }

        private void SetBackOfficeCredentials(Dictionary<string, string> credentials)
        {
            if (BackOfficeAdminCredentials == null)
                BackOfficeAdminCredentials = new Dictionary<string, string>();

            foreach (var key in credentials.Keys)
            {
                if (BackOfficeAdminCredentials.ContainsKey(key))
                    BackOfficeAdminCredentials[key] = credentials[key];
                else
                {
                    BackOfficeAdminCredentials.Add(key, credentials[key]);
                }
            }
        }


        private BackOfficePlugin GetBackoffPluginSelected(string backofficeid)
        {
            if (!BackOfficePlugins.Any())
                return null;

            return BackOfficePlugins.FirstOrDefault(key => key.PluginId == backofficeid);
        }


        private void ShowErrorMessage(string errormessage, string errorcaption)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowErrorMessage(errormessage, errorcaption);
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        private void ShowErrorMessage(string errormessage, string errorcaption, string helplinkuri)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowErrorMessage(errormessage, errorcaption,helplinkuri);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ShowInformationMessage(string inforamtionmessage, string infocaption, string helplinkuri)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowInformationMessage(inforamtionmessage, infocaption,helplinkuri);
        }
        private void ShowSaveMessage(string savemessage, string savecaption)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowSaveInformationMessage(savemessage, savecaption, CustomerFacingMessages.InfoCaption_OpenConnectionMonitor);

        }
        private bool ShowConfirmationMessage()
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.DisplayConfirmationMessage = Visibility.Visible;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public BackOfficePlugin[] BackOfficePlugins { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BackOfficePluginsResponse BackOfficePluginsResponse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BackOfficeConnectionCredentials { get; set; }

        private Dictionary<string, string> BackOfficeAdminCredentials { get; set; }


        /// <summary>
        /// Property Changed event derived from the List.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        ///  <summary>
        /// Create the OnPropertyChanged method to raise the event  
        ///  </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// ConfigurationViewModel for saveing the connection details
        /// </summary>
        public ConfigurationViewModel ConfigurationViewModel { get; set; }


        private string _tenantName;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public String TenantName
        {
            get { return _tenantName; }
            set
            {
                _tenantName = value;
                OnPropertyChanged();
            }

        }

        private Visibility _showEditSetting;

        /// <summary>
        /// To show the Edit settings and clear Setting in Settings control
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public Visibility ShowEditSetting
        {
            get { return _showEditSetting; }

            set
            {
                _showEditSetting = value;
                OnPropertyChanged();
            }
        }

        private Visibility _showHelpSetting;

        /// <summary>
        /// To show the Edit settings and clear Setting in Settings control
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public Visibility ShowHelpSetting
        {
            get { return _showHelpSetting; }

            set
            {
                _showHelpSetting = value;
                OnPropertyChanged();
            }
        }
        private string _settingsHeaderDscription;
        private string _editSettingsTooltip;
        private string _clearSettingsTooltip;

        /// <summary>
        /// Header Description for Settings Control
        /// </summary>
        public string MessageDescription
        {
            get { return _settingsHeaderDscription; }

            set
            {
                _settingsHeaderDscription = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Tool Tip for Edit Settings in  Settings Control
        /// </summary>
        public string EditSettingsTooltip
        {
            get { return _editSettingsTooltip; }

            set
            {
                _editSettingsTooltip = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Tool Tip for Clear Settings in  r Settings Control
        /// </summary>
        public string ClearSettingsTooltip
        {
            get { return _clearSettingsTooltip; }

            set
            {
                _clearSettingsTooltip = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// To Handle the click Event on Edit Setting from settings control
        /// </summary>
        public EventHandler EditSettingsClickHandler;

        private Visibility _connectionDetailVisibility;
        private EventHandler CompanyselectedEvent;

        /// <summary>
        /// To set the connection Detail's control's visiblility
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public Visibility ConnectiondetailVisiblity
        {
            get { return _connectionDetailVisibility; }

            set
            {
                _connectionDetailVisibility = value;
                OnPropertyChanged();
            }
        }

        internal void EditSettingsClick(object sender, RoutedEventArgs e)
        {
            EditConfigurationDetails(_premiseConfigurationRecord, CustomerFacingMessages.Progressbar_LoadingConnection);

        }

        /// <summary>
        /// Displays the Confirmation Dialog to the User
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ClearSettingsClick(object sender, RoutedEventArgs e)
        {
            if (ShowConfirmationMessage())
            {

            }
        }

        /// <summary>
        /// Delete the connection from the PCR and clears the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ConfirmSettingsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfigurationHelpers.TenantExists(ConfigurationViewModel.TenantGuid.ToString()))
                {
                    var registrationInfo = ConfigurationHelpers.ClearConnectorRegistration(
                        ApplicationHelpers.ConnectorServiceUri,
                        ConfigurationViewModel.TenantGuid.ToString(),
                        ApplicationHelpers.SageIdToken);
                    if (registrationInfo.Successful)
                    {
                        ConfigurationHelpers.DeleteTenant(ConfigurationViewModel.TenantGuid.ToString());
                        SetConnectionStatus(ConnectionState.OffLine, BackOfficeConnectivityStatus.None,
                            ConnectionState.OffLine);
                        _metaPropertyLists = null;
                        Connectionselect(0);
                    }
                }

            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                throw;
            }
        }


        private void SetConnectionStatus(ConnectionState cloudstatus, BackOfficeConnectivityStatus backofficestatus,
            ConnectionState connectionstatus)
        {
            ConfigurationViewModel.CloudStatus = cloudstatus;
            ConfigurationViewModel.BackOfficeStatus = backofficestatus;
            ConfigurationViewModel.ConnectionStatus = connectionstatus;
        }

    }


}
