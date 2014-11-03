using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.Logging;

namespace Sage.Connector.ConfigurationService.Internal
{
    /// <summary>
    /// A configuration service for storing and retrieving premise-cloud connectivity attributes
    /// </summary>
    public sealed class AuthorizationHelper
    {
        static AuthorizationHelper()
        {
            // add authorized accounts that are allowed to access ConfigurationService methods
            try
            {
                _authorizationResults.TryAdd(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null).Translate(typeof(NTAccount)).Value, true);
                _authorizationResults.TryAdd(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null).Translate(typeof(NTAccount)).Value, true);
                _authorizationResults.TryAdd(new SecurityIdentifier(WellKnownSidType.LocalServiceSid, null).Translate(typeof(NTAccount)).Value, true);
                _authorizationResults.TryAdd(ConnectorServiceUtils.GetServiceAccountUserName(), true);
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void AuthorizeClientPrimaryIdentity()
        {
            var primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity;
            var primaryIdentityName = primaryIdentity.Name;

            Boolean authorized = false;
            try
            {
                authorized = _authorizationResults.GetOrAdd(primaryIdentityName,
                     (String theUserName) =>
                     {
                         Boolean isLocalAdminGroupMember = false;

                         if (ServiceSecurityContext.Current.WindowsIdentity.ImpersonationLevel == TokenImpersonationLevel.Impersonation ||
                             ServiceSecurityContext.Current.WindowsIdentity.ImpersonationLevel == TokenImpersonationLevel.Delegation)
                         {
                             // Impersonate and check if the user is a local admin
                             using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                             {
                                 // Check to see if the current (impersonated) identity is a local admin group member; if so, then client is authorized
                                 isLocalAdminGroupMember = IsCurrentWindowsIdentityALocalAdminGroupMember();
                             }
                         }
                         else
                         {
                             // Since the ConfigurationServiceProxy automatically sets the AllowedImpersonationLevel, this case should really never occur
                             using (var lm = new LogManager())
                             {
                                 lm.WriteWarning(null, "{0} attempted to use ConfigurationService but '{1}' is not a high enough ImpersonationLevel to perform an authorisation check", theUserName, ServiceSecurityContext.Current.WindowsIdentity.ImpersonationLevel);
                             }
                         }

                         return isLocalAdminGroupMember;
                     });
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }

            if (!authorized)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(null, "{0} is not authorized and was denied access to ConfigurationService", primaryIdentityName);
                }
                throw new SecurityException(); // WCF automatically translates SecurityException from server into a SecurityAccessDeniedException on the client
            }
        }

        private enum TOKEN_INFORMATION_CLASS : int
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass  // MaxTokenInfoClass should always be the last enum
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public UInt32 Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_GROUPS
        {
            public UInt32 GroupCount;
            [MarshalAs(UnmanagedType.ByValArray)]
            public SID_AND_ATTRIBUTES[] Groups;
        };

        // Using IntPtr for pSID instead of Byte[]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean ConvertSidToStringSid(IntPtr pSID, out IntPtr ptrSid);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            UInt32 TokenInformationLength,
            out UInt32 ReturnLength);


        private static string GetSidString(IntPtr sid)
        {
            IntPtr ptrSid = IntPtr.Zero;

            String result = String.Empty;
            if (sid != IntPtr.Zero)
            {
                if (ConvertSidToStringSid(sid, out ptrSid))
                {
                    try
                    {
                        result = Marshal.PtrToStringAuto(ptrSid);
                    }
                    finally
                    {
                        LocalFree(ptrSid);
                    }
                }
            }

            return result;
        }

        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        private static Boolean IsCurrentWindowsIdentityALocalAdminGroupMember()
        {
            Boolean result = false;

            IntPtr tokenInformation = IntPtr.Zero;
            try
            {
                var localAdminGroupSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                // first call gets length of TokenInformation
                UInt32 tokenInformationLength = 0;
                if (!GetTokenInformation(WindowsIdentity.GetCurrent().Token, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, tokenInformationLength, out tokenInformationLength))
                {
                    if (Marshal.GetLastWin32Error() != ERROR_INSUFFICIENT_BUFFER)
                    {
                        return false;
                    }
                }

                tokenInformation = Marshal.AllocHGlobal(System.Convert.ToInt32(tokenInformationLength));
                if (!GetTokenInformation(WindowsIdentity.GetCurrent().Token, TOKEN_INFORMATION_CLASS.TokenGroups, tokenInformation, tokenInformationLength, out tokenInformationLength))
                {
                    return false;
                }

                TOKEN_GROUPS tokenGroups = (TOKEN_GROUPS)Marshal.PtrToStructure(tokenInformation, typeof(TOKEN_GROUPS));
                Int32 sidAndAttrSize = Marshal.SizeOf(new SID_AND_ATTRIBUTES());
                IntPtr pSidAndAttributes = new IntPtr((Int64)tokenInformation + (Int64)Marshal.OffsetOf(typeof(TOKEN_GROUPS), "Groups"));
                for (Int32 i = 0; i < tokenGroups.GroupCount; i++)
                {
                    SID_AND_ATTRIBUTES sidAndAttributes = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure(pSidAndAttributes + (i * sidAndAttrSize), typeof(SID_AND_ATTRIBUTES));
                    if (GetSidString(sidAndAttributes.Sid) == localAdminGroupSid.Value)
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tokenInformation);
            }

            return result;
        }


        #region Private fields
        private static readonly String _myTypeName = typeof(AuthorizationHelper).FullName;
        private static ConcurrentDictionary<String, Boolean> _authorizationResults = new ConcurrentDictionary<String, Boolean>();
        #endregion
    }
}
