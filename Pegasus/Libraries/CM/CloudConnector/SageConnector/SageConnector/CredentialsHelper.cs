using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace SageConnector
{
    static class CredentialsHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",Justification="Factory - Rule N/A")]
        static public void AddCredentialControls(
           string controlDescriptionsAsString,
           IDictionary<string, string> currentValues,
           IDictionary<string, Control> newValueControls,
           Control.ControlCollection controlCollection,
           Control lableMarker,
           Control editMarker)
        {
            List<string> tokens = new List<string>();

            if (!String.IsNullOrEmpty(controlDescriptionsAsString) && String.CompareOrdinal(controlDescriptionsAsString,"null") !=0 )
            {
                var jDesc = JObject.Parse(controlDescriptionsAsString);

                int i = 0;

                foreach (var item in jDesc.Properties())
                {
                    tokens.Add(item.Name);

                    var description = jDesc[item.Name];
                    bool itemHasProperties = (description.Any());
                    if (itemHasProperties)
                    {
                        int xOffset = 0;
                        int yOffset = 30 * i;
                        var lblSize = lableMarker.Size;
                        var lblLoc = lableMarker.Location;
                        var offset = new Point(xOffset, yOffset);
                        lblLoc.Offset(offset);

                        var conSize = editMarker.Size;
                        var conLoc = editMarker.Location;
                        conLoc.Offset(offset);

                        var lbl = new Label
                        {
                            Location = lblLoc,
                            Size = lblSize,
                            Visible = true,
                            Text = (string)description["DisplayName"] + ":"
                        };

                        controlCollection.Add(lbl);


                        string v = string.Empty;
                        if (currentValues.ContainsKey(item.Name))
                        {
                            v = currentValues[item.Name];
                        };
                        bool treatAsList = (description.Value<object>("ValueName") != null);
                        
                        if (treatAsList)
                        {
                            List<ListItem> listItems = new List<ListItem>();
                            IList<string> emptyList = new List<string>() as IList<string>;

                            bool hasName = description["ValueName"] != null;
                            bool hasId = (jDesc[item.Name].Value<object>("ValueId") != null);
                            bool hasDescription = (jDesc[item.Name].Value<object>("ValueDescription") != null);
                            
                            //assumes that all are same size of others are null. ok for now but value up checking 
                            IList<string> names = (hasName ? description["ValueName"].ToObject<IList<string>>() : emptyList);
                            IList<string> ids = (hasId ? description["ValueId"].ToObject<IList<string>>() : names);
                            int selectedindex = ids.IndexOf(v);
                            IList<string> descriptions = (hasDescription ? description["ValueDescription"].ToObject<IList<string>>() :names);
                            
                            var con = new ComboBox()
                            {
                                Location = conLoc,
                                Size = conSize,
                                Visible = true,
                            };
                            newValueControls[item.Name] = con;
                            controlCollection.Add(con);
                                
                            //we finally know all list parts are here, are IList<string> and not empty and the same length. Finally time to make a connectionlist
                            int count = names.Count;
                            for (int j = 0; j < count; j++)
                            {
                                ListItem listItem = new ListItem
                                {
                                    Name =names[j], 
                                    Id = ids[j],
                                    Description = descriptions[j],
                                  
                                };
                                listItems.Add(listItem);
                            }
                            con.DataSource = listItems;
                            con.DisplayMember = "Name";
                            con.SelectedIndex = selectedindex;

                            //todo: look at adding data bindings instead of simple pull push
                            //txtPremiseDataPath.DataBindings.Add(new Binding("Text", _configuration, "BackOfficeConnectionInformationDisplayable"));
                            //txtPremiseUsername.DataBindings.Add(new Binding("Text", _configuration, "BackOfficeUserName"));
                            //txtMarker.DataBindings.Add(new Binding("Text", _configuration, "BackOfficeUserPassword"));
                        }
                        else
                        {
                            bool? isPassword = (bool?)description["IsPassword"];
                            //todo: add path picker?
                            bool? isPath = (bool?)description["IsPath"];
                            
                            if (isPath.HasValue && isPath == true)
                            {
                                conSize = new Size(conSize.Width - conSize.Height, conSize.Height);
                            }

                            var con = new TextBox()
                            {
                                Location = conLoc,
                                Size = conSize,
                                Visible = true,
                                Text = v,
                                PasswordChar = (isPassword.HasValue && isPassword.Value ? '*' : (char)0)
                            };
                            newValueControls[item.Name] = con;
                            controlCollection.Add(con);

                            if (isPath.HasValue && isPath == true)
                            {
                                var pickerLoc = new Point(conLoc.X + conSize.Width + 2, conLoc.Y);
                                var pickerSize = new Size(conSize.Height, conSize.Height);
                                var pickerButton = new Button()
                                {
                                    Location = pickerLoc,
                                    Size = pickerSize,
                                    Text = "...",
                                    Visible = true,
                                    Tag = con,
                                    
                                };
                                pickerButton.Click += pickerButton_Click;
                                controlCollection.Add(pickerButton);
                            }
                        }

                        i++;
                    }
                }
            }
        }

        static void pickerButton_Click(object sender, EventArgs e)
        {
            Button s = sender as Button;
            if (s != null)
            {
                TextBox target = s.Tag as TextBox;
                if (target != null)
                {
                    using(OpenFileDialog dialog = new OpenFileDialog())
                    { 
                        dialog.InitialDirectory = target.Text;
                        var result = dialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            target.Text = dialog.FileName;
                        }
                    }
                }
            }          
        }
    }


    internal class ListItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
    }
}
