using System;
using System.ServiceModel;
using System.Threading;
using Sage.Connector.StateService.Interfaces.Faults;
using Sage.Diagnostics;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// Helper class to facilitate throwing a FaultException out of a WCF service boundary; inspired by ArgumentValidator
    /// in CRE\Core
    /// </summary>
    internal static class FaultArgumentValidator
    {
        /// <summary>
        /// Make sure a reference argument is non-null
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <exception cref="ArgumentNullException"/>
        public static void ValidateNonNullReference(Object argument, String name, String source)
        {
            if (argument == null)
            {
                ValidateCallerInfo(ref name, ref source);
                String errorMessage = String.Format(Thread.CurrentThread.CurrentCulture, "A null {0} reference was passed to {1}", name, source);
                throw new FaultException<ArgumentNullFault>(new ArgumentNullFault(name, errorMessage));
            }
        }

        /// <summary>
        /// Make sue a string argument is both non-null and non-empty
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static void ValidateNonEmptyString(String argument, String name, String source)
        {
            ValidateNonNullReference(argument, name, source);
            if (argument == String.Empty)
            {
                ValidateCallerInfo(ref name, ref source);
                String errorMessage = String.Format(Thread.CurrentThread.CurrentCulture, "An empty {0} string was passed to {1}", name, source);
                throw new FaultException<ArgumentFault>(new ArgumentFault(name, errorMessage));
            }
        }

        #region Private methods
        /// <summary>
        /// Make sure the caller info is ok before generating error messages
        /// </summary>
        /// <param name="name">The argument name</param>
        /// <param name="source">The calling source</param>
        private static void ValidateCallerInfo(ref String name, ref String source)
        {
            if (String.IsNullOrEmpty(name))
            {
                Assertions.Assert(false, "An empty or null argument name was passed to the FaultArgumentValidator");
                name = "argument";
            }

            if (String.IsNullOrEmpty(source))
            {
                Assertions.Assert(false, "An empty or null source was passed to the FaultArgumentValidator");
                source = "Method";
            }
        }
        #endregion
    }
}
