using System;
using System.Diagnostics;
using System.Globalization;
using Sage.Diagnostics;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - n/a
namespace Sage.Connector.LinkedSource
{
    /// <summary>
    /// A  helper class to facilitate writing simple messages to Trace output.  This class is intended for use only 
    /// in TEST assemblies.  PRODUCTION assemblies should use the more robust *Trace classes in Sage.Diagnostics
    /// (in Sage.CRE.Core.dll).
    /// </summary>
    internal static class TraceUtils
    {
        /// <summary>
        /// Need this version for the message inspector delegate
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(String message)
        {
            WriteLine(message, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageToFormat"></param>
        /// <param name="list"></param>
        public static void WriteLine(String messageToFormat, params Object[] list)
        {
            try
            {
                // for efficiency, don't call string.Format if we don't have to (i.e., if no replaceable param value were supplied)
                if (list != null && list.Length > 0)
                {
                    Trace.WriteLine(String.Format(System.Globalization.CultureInfo.InvariantCulture, messageToFormat, list));
                }
                else
                {
                    Trace.WriteLine(messageToFormat);
                }
            }
            catch (FormatException)
            {
                // Catch any exception while trying to write a trace message and prevent it from changing the control
                // flow of the production code.  This can heppen, for example, if the caller supplied an invalid
                // messageToFormat (e.g., one that contains literal "{" and "}" characters that do not match up
                // with replaceable parameters).
                try
                {
                    String message = String.Format(CultureInfo.InvariantCulture, "FormatException failure occurred during output trace message '{0}'", messageToFormat);
                    Trace.WriteLine(message);
                    Assertions.Assert(false, message);
                }
                catch (Exception e)
                {
                    // Catch any exception while trying to write a trace message and prevent it from changing the control
                    // flow of the production code.
                    Assertions.Assert(false, e.Message);
                }
            }
        }
    }
}
