using System;
using System.Collections.Generic;
using Sage.Ssdp.Security.Client;

namespace Sage.Connector.AutoUpdate.Addin
{
    /// <summary>
    /// Class for handling update version comparisons.
    /// </summary>
    /// <remarks>
    /// Specialized string compare which allows for breaking a string in the format of X.X.X.X down into 
    /// integral parts, then comparing those parts. If one side has fewer parts than the other, then zero (0)
    /// is used for the missing part.
    /// </remarks>
    internal class ISoftwareUpdateComparerVersionId : IComparer<ISoftwareUpdate>
    {
        /// <summary>
        /// Compares two version strings to determine versioning difference.
        /// </summary>
        /// <param name="leftParam">The left side to compare.</param>
        /// <param name="rightParam">The right side to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears
        /// in the same position in the sort order as the value parameter.
        /// </returns>
        public static int Compare(string leftParam, string rightParam)
        {
            if (String.IsNullOrEmpty(leftParam)) throw new ArgumentNullException("leftParam");
            if (String.IsNullOrEmpty(rightParam)) throw new ArgumentNullException("rightParam");

            var leftParts = leftParam.Split(new[] { '.' });
            var rightParts = rightParam.Split(new[] { '.' });
            var count = Math.Max(leftParts.Length, rightParts.Length);
            var diff = 0;

            for (var i = 0; i < count; i++)
            {
                var lval = (i < leftParts.Length) ? Convert.ToInt32(leftParts[i]) : 0;
                var rval = (i < rightParts.Length) ? Convert.ToInt32(rightParts[i]) : 0;

                diff = lval.CompareTo(rval);

                if (diff != 0) return diff;
            }

            return diff;
        }

        /// <summary>
        /// Compares two ISoftwareUpdate interfaces to determine versioning difference.
        /// </summary>
        /// <param name="leftParam">The left side to compare.</param>
        /// <param name="rightParam">The right side to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears
        /// in the same position in the sort order as the value parameter.
        /// </returns>
        public int Compare(ISoftwareUpdate leftParam, ISoftwareUpdate rightParam)
        {
            return Compare(leftParam.VersionId, rightParam.VersionId);
        }
    }
}