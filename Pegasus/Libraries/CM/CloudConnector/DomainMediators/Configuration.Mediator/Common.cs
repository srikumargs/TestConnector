using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Configuration.Mediator
{
    static class Common
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backOfficeHandlers"></param>
        /// <param name="backOfficeId"></param>
        /// <returns></returns>
        static public T FindFeatureProcessor<T>(IEnumerable<Lazy<T, IBackOfficeData>> backOfficeHandlers, string backOfficeId)
        {
            // run the the back office handers that provide interface T.
            //find the one that has a matching back office id
            var featureProcessor = (from backOfficeHandler in backOfficeHandlers
                                               where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeId, StringComparison.CurrentCultureIgnoreCase)
                                               select backOfficeHandler.Value).FirstOrDefault();

            return featureProcessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureProcessor"></param>
        /// <param name="backOfficeId"></param>
        public static void ValidateFeatureProcessor<T>(T featureProcessor, string backOfficeId)
            where T : class
        {
            if (featureProcessor == null)
            {
                string typename = typeof (T).ToString();
                Debug.Print("{0} back office to process the {1} was not found!", backOfficeId, typename);
                throw new ApplicationException(String.Format("{0} BackOffice to process the {1}  was not found!", backOfficeId, typename));
            }
        }

        public static void ValidateResponse (Response response, string backOfficeId, string requestName)
        {

            //check we got a valid success or failure
            if (response == null || response.Status.Equals(Status.Indeterminate))
            {
                throw new ApplicationException(String.Format("{0} back office failed to supply the proper {1} response Status.", backOfficeId, requestName));
            }
        }
    }
}
