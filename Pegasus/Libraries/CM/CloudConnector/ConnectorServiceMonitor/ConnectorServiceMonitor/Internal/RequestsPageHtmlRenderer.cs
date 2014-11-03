using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;
using Sage.Connector.Common.DataContracts;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class RequestsPageHtmlRenderer : HtmlRenderer
    {
        public RequestsPageHtmlRenderer(ImageManager imageManager, Control control)
            : base(imageManager, control)
        { }

        protected override void ComputeCol1(Graphics g, Font font)
        {

            int widthOfW = Convert.ToInt32(g.MeasureString("W", font).Width);

            _colWidths[0] = Convert.ToInt32(_imageManager.Blank16.Width);

            _colWidths[1] = Convert.ToInt32(Math.Max(g.MeasureString(DateTime.Now.ToString("G"), font).Width, _colWidths[1]));
            _colWidths[1] += widthOfW;
            
            _colWidths[2] = Convert.ToInt32(Math.Max(g.MeasureString("Customer Site", font).Width, _colWidths[2]));
            _colWidths[2] += widthOfW;
            
            _colWidths[3] = Convert.ToInt32(Math.Max(g.MeasureString(Strings.Requests_ColHeader_Type, font).Width, _colWidths[3]));
            _colWidths[3] += widthOfW;
   
            _colWidths[4] = Convert.ToInt32(Math.Max(g.MeasureString(DateTime.Now.ToString("G"), font).Width, _colWidths[4]));
            _colWidths[4] += widthOfW;
            
            _colWidths[5] = Convert.ToInt32(Math.Max(g.MeasureString(Strings.Requests_ColHeader_ElapsedTime, font).Width, _colWidths[5]));
            _colWidths[5] += widthOfW;

            _maxRowHeight = Convert.ToUInt32(_imageManager.Blank16.Height * 1.25);
        }

        public String Render(ServiceStatus status)
        {
            const int initialSize = 16384;
            var sb = new StringBuilder(Strings.General_HtmlDocument, initialSize);

            if (status.MonitorServiceStatus.HasValue)
            {
                var requests = FilterRequests(status.Requests);
                RenderRequests(sb, requests);
            }

            sb.Replace("{Column1Width}", Convert.ToString(_colWidths[0]));
            sb.Replace("{Column2Width}", Convert.ToString(_colWidths[1]));
            sb.Replace("{Column3Width}", Convert.ToString(_colWidths[2]));
            sb.Replace("{Column4Width}", Convert.ToString(_colWidths[3]));
            sb.Replace("{Column5Width}", Convert.ToString(_colWidths[4]));
            sb.Replace("{Column6Width}", Convert.ToString(_colWidths[5]));

            ReplaceCommonFormatting(sb);

            return sb.ToString();
        }

        Int32[] _colWidths = new Int32[6];
        private void RenderRequests(StringBuilder sb, IList<RequestState> requests)
        {
            DateTime now = DateTime.UtcNow;

            const string htmlFragment = 
                "<tr>" +
                "<td width='{{Column1Width}}' height='{{RowHeight}}'>{0}</td>" +
                "<td width='{{Column2Width}}' height='{{RowHeight}}'><b><u>{1}</u></b></td>" +
                "<td width='{{Column3Width}}' height='{{RowHeight}}'><b><u>{2}</u></b></td>" +
                "<td width='{{Column4Width}}' height='{{RowHeight}}'><b><u>{3}</u></b></td>" +
                "<td width='{{Column5Width}}' height='{{RowHeight}}'><b><u>{4}</u></b></td>" +
                "<td width='{{Column6Width}}' height='{{RowHeight}}'><b><u>{5}</u></b></td>" +
                "</tr>{6}";

            sb.Replace("{AdditionalHtml}", String.Format(htmlFragment,
                String.Empty,
                Strings.Requests_ColHeader_StartedOn,
                "Customer Site",
                Strings.Requests_ColHeader_Type,
                Strings.Requests_ColHeader_CompletedOn,
                Strings.Requests_ColHeader_ElapsedTime,
                "{AdditionalHtml}"));

            StringBuilder rowBuilder;
            if (requests != null)
            {
                //the loop below is performance critical. 
                //to the point that we want to make sure to stat the string builder with roughly the right amount of capacity
                //Things like computing the blank image width when in the loop were causing significant slow downs.
                //this is also why we use a secondary string builder for the rows. It lets us simply append rather then
                //replace. in this context replace is an expensive operation. Cases of 10,000+ requests are very real.
                //Yes there should be a better pattern so we do not have to process them all at once. But given
                //where we are in the release cycle for we want to make requests take seconds no minutes.
                String[] cols = new String[7];

                int precomputedColWidth0 = (int)(_imageManager.Blank16.Width * 1.25);
                int totalRequests = requests.Count;
                int roughSize = 100 + totalRequests*256;
                rowBuilder = new StringBuilder(roughSize);
                int rowNumber = 0;
                foreach (var request in requests)
                {
                    rowNumber++;
                    cols[0] = Strings.General_ImageWithAltHtmlFragment;
                    switch(request.RequestStatus)
                    {
                        case RequestStatus.CompletedWithErrorResponse:
                            cols[0] = cols[0].Replace("{BitmapSource}", _imageManager.Critical16ImageFileName);
                            cols[0] = cols[0].Replace("{AltText}", "Completed with error response");
                            break;
                        case RequestStatus.CompletedWithSuccessResponse:
                            cols[0] = cols[0].Replace("{BitmapSource}", _imageManager.OK16ImageFileName);
                            cols[0] = cols[0].Replace("{AltText}", "Completed successfully");
                            break;
                        case RequestStatus.InProgress:
                            cols[0] = cols[0].Replace("{BitmapSource}", _imageManager.Blank16ImageFileName);
                            cols[0] = cols[0].Replace("{AltText}", "In-progress");
                            break;
                        case RequestStatus.InProgressMediationBoundWorkProcessing:
                        case RequestStatus.InProgressBindableWorkProcessing:
                            cols[0] = cols[0].Replace("{BitmapSource}", _imageManager.Running16ImageFileName);
                            cols[0] = cols[0].Replace("{AltText}", "In-progress, bindable work processing");
                            break;
                        case RequestStatus.CompletedWithCancelResponse:
                            cols[0] = cols[0].Replace("{BitmapSource}", _imageManager.Cancel16ImageFileName);
                            cols[0] = cols[0].Replace("{AltText}", "Completed with cancel response");
                            break;
                    }

                    cols[1] = request.DateTimeUtc.ToLocalTime().ToString("G");
                    cols[2] = request.TenantName;

                    string requestTypeToShow = DisplayValueForRequestType(request);
                    cols[3] = requestTypeToShow;    
                    
                    //these values are not longer used in the pegasus connector messages for first release. Some maybe be back later.
                    //cols[4] = request.CloudRequestSummary;
                    //cols[5] = request.CloudProjectName;
                    //cols[6] = request.CloudRequestRequestingUser;
                    cols[4] = (request.State17DateTimeUtc.HasValue) ? request.State17DateTimeUtc.Value.ToLocalTime().ToString("G") : String.Empty;
                    cols[5] = (request.State17DateTimeUtc.HasValue) ? Common.CreateElapsedTimeString(request.State17DateTimeUtc.Value - request.DateTimeUtc, true) : Common.CreateElapsedTimeString(now - request.DateTimeUtc, true);

                    //only add additionalHtml tag to last row for fill in later.
                    bool lastRow = (rowNumber == totalRequests);
                    cols[6] = (lastRow ? "{AdditionalHtml}" : string.Empty);
                    
                    string additionalFragment =
                        "<tr>" +
                        "<td width='{{Column1Width}}' height='{{RowHeight}}' bgcolor='{7}'>{0}</td>" +
                        "<td width='{{Column2Width}}' height='{{RowHeight}}' bgcolor='{7}'>{1}</td>" +
                        "<td width='{{Column3Width}}' height='{{RowHeight}}' bgcolor='{7}'>{2}</td>" +
                        "<td width='{{Column4Width}}' height='{{RowHeight}}' bgcolor='{7}'>{3}</td>" +
                        "<td width='{{Column5Width}}' height='{{RowHeight}}' bgcolor='{7}'>{4}</td>" +
                        "<td width='{{Column6Width}}' height='{{RowHeight}}' bgcolor='{7}'>{5}</td>" +
                        "</tr>{6}";
                    string additionalHtml = String.Format(additionalFragment, cols[0], cols[1], cols[2], cols[3], cols[4], cols[5], cols[6],
                        ((rowNumber % 2) != 0) ? "{ControlLightBackgroundColor}" : "{WindowBackgroundColor}");

                    rowBuilder.Append(additionalHtml);

                    _colWidths[0] = precomputedColWidth0;
                    _colWidths[1] = ComputeColWidth(cols[1] + "W", _colWidths[1]);
                    _colWidths[2] = ComputeColWidth(cols[2] + "W", _colWidths[2]);
                    _colWidths[3] = ComputeColWidth(cols[3] + "W", _colWidths[3]);
                    _colWidths[4] = ComputeColWidth(cols[4] + "W", _colWidths[4]);
                    _colWidths[5] = ComputeColWidth(cols[5] + "W", _colWidths[5]);
                }
                string rowHtml = rowBuilder.ToString();
                sb.Replace("{AdditionalHtml}", rowHtml);
            }
        }

        private static string DisplayValueForRequestType(RequestState request)
        {
            string retval = (!String.IsNullOrWhiteSpace(request.CloudRequestInnerType)
                                ? request.CloudRequestInnerType
                                : request.CloudRequestType);

            //we only get a "short/friendly" name for name for a dm request once its in the binding layer.
            //However requests show up well before that in the monitor. So show a less intimidating name for them
            string dmRequestName = "Sage.Connector.Cloud.Integration.Interfaces.Requests.DomainMediationRequest";
            if (String.Compare(retval, dmRequestName,StringComparison.OrdinalIgnoreCase ) == 0)
            {
                string friendlyName = Strings.RequestsPageHtmlRenderer_DisplayValueForRequestType_DomainMediation;
                retval = friendlyName;
            }

            return retval;
        }

        /// <summary>
        /// Filter system requests from the request list unless asked to show them all.
        /// </summary>
        /// <param name="requestsState"></param>
        /// <returns></returns>
        private IList<RequestState> FilterRequests(IEnumerable<RequestState> requestsState)
        {
            //only check the machine environment variable once. This is mostly a small performance boost
            if (!_showAllMessages.HasValue)
            {
                _showAllMessages = false;
                String showAll = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_MONITOR_SHOW_ALL", EnvironmentVariableTarget.Machine);
                if (!String.IsNullOrEmpty(showAll) && showAll == "1")
                {
                    _showAllMessages = true;
                }
            }

            IList<RequestState> retval;
            if (requestsState != null)
            {
                retval = (_showAllMessages.Value ? requestsState : requestsState.Where(r => !r.IsSystemRequest)).ToList();
            }
            else
            {
                //should we log empty list?
                //provide an empty list for down stream.
                retval = new List<RequestState>();
            }
            return retval;
        }
        private bool? _showAllMessages = null;
        
    }
}

