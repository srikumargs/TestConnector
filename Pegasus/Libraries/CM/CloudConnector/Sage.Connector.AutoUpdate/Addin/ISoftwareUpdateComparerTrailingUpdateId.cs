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
    internal class ISoftwareUpdateComparerTrailingUpdateId : IComparer<ISoftwareUpdate>
    {
        private string _baseName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISoftwareUpdateComparerTrailingUpdateId"/> class.
        /// </summary>
        /// <param name="baseName">Name of the base.</param>
        public ISoftwareUpdateComparerTrailingUpdateId(string baseName)
        {
            if(baseName == null) throw new NullReferenceException("baseName");
            _baseName = baseName;
        }


        /// <summary>
        /// Compares two version strings to determine versioning difference.
        /// </summary>
        /// <param name="leftParam">The left side to compare.</param>
        /// <param name="rightParam">The right side to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance precedes, follows, or appears
        /// in the same position in the sort order as the value parameter.
        /// </returns>
        private static int CompareNPartVersion(string leftParam, string rightParam)
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
        /// Compares the specified left parameter.
        /// </summary>
        /// <param name="leftParam">The left parameter.</param>
        /// <param name="rightParam">The right parameter.</param>
        public int Compare(string leftParam, string rightParam)
        {
            int retval = 0;
            bool leftPrefixed = leftParam.StartsWith(_baseName);
            bool rightPrefixed = rightParam.StartsWith(_baseName);

            //in use we should only hit cases that include the prefix, but code to handle no prefix for safety/correctness.
            if (leftPrefixed && rightPrefixed)
            {
                //both prefixed proceed to evaluate values, after stripping prefix
                int prefixLength = _baseName.Length;
                //stip prefix and the prefixing dot, prefixing dot must be part of the prefix
                string leftStripped = leftParam.Substring(prefixLength);
                string rightStripped = rightParam.Substring(prefixLength);

                retval = CompareNPartVersion(leftStripped, rightStripped);
            }
            else if (!leftPrefixed && !rightPrefixed)
            {
                //neither is prefixed, just string compare them
                retval = String.Compare(leftParam, rightParam, StringComparison.OrdinalIgnoreCase);
            }
            else if (leftPrefixed)
            {   //left prefixed is true but right is not, so left is greater.
                //return the prefixed value as greater as that will result in a value that includes the prefix if possible.
                retval = 1;
            }
            else
            {   //left prefixed is false but right is true, so right is greater
                //return the prefixed value as greater as that will result in a value that includes the prefix if possible.
                retval = -1;
            }
            
            return retval;
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
            //are both prefexied properly? what do we do if they are not?

            string leftValue = leftParam.UpdateId;
            string rightValue = rightParam.UpdateId;

            int retval = Compare(leftValue, rightValue);
            return retval;
        }
    }
}