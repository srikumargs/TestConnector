using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using Sage.Diagnostics;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// Static class hosting process tree killing utility function
    /// </summary>
    public static class ProcessKiller
    {
        /// <summary>
        /// Kills the specified process and its child processes
        /// </summary>
        /// <param name="iProcessId"></param>
        public static void KillProcessTree(int iProcessId)
        {
            String myQuery = string.Format("select * from win32_process where ParentProcessId={0}", iProcessId);
            ManagementScope mScope = new ManagementScope(@"\\localhost\root\cimv2", null);
            mScope.Connect();

            if (mScope.IsConnected)
            {
                ObjectQuery objQuery = new ObjectQuery((myQuery));
                using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(mScope, objQuery))
                {
                    using (ManagementObjectCollection result = objSearcher.Get())
                    {
                        if (null != result)
                        {
                            foreach (ManagementObject item in result)
                            {
                                KillProcessTree(Convert.ToInt16(item["ProcessId"]));
                            }
                        }
                    }
                }
            }
            else
            {
                EventLogger.WriteMessage(EventLogger.DefaultSourceName, "Unable to connect to WMI to cancel a process.", MessageType.Warning);
            }

            try
            {
                Process pToKill = Process.GetProcessById(iProcessId);
                pToKill.Kill();
                pToKill.WaitForExit(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds));
            }
            catch (Exception ex)
            {
                // Win32Exception - Could not be terminated / terminating / Win16
                // NotSupportedException - On a remote system
                // ArgumentException - iProcessId not running
                // InvalidOperationException - Not started /exited
                EventLogger.WriteMessage(EventLogger.DefaultSourceName, "Exception encountered canceling a process: " + ex.ToString(), MessageType.Warning);
            }
        }
    }
}
