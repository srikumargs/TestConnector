using System;
using System.Security;
using System.Runtime.InteropServices;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - n/a
namespace Sage.Connector.LinkedSource
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SecureStringUtils
    {
        /// <summary>
        /// Convert a String to a SecureString
        /// </summary>
        /// <param name="stringToSecure"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this String stringToSecure)
        {
            SecureString result = null;
            try
            {
                result = new SecureString();
                if (!String.IsNullOrEmpty(stringToSecure))
                {
                    char[] charArray = stringToSecure.ToCharArray();
                    foreach (char c in charArray)
                    {
                        result.AppendChar(c);
                    }
                }
                result.MakeReadOnly();
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
                    result = null;
                }
            }
            return result;
        }

        /// <summary>
        /// Convert a SecureString to a String
        /// Eventually will involve encryption and decryption
        /// </summary>
        /// <param name="sString"></param>
        /// <returns></returns>
        public static String ToNonSecureString(this SecureString sString)
        {
            if (null == sString)
            {
                return String.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(sString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
