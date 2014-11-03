using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - N/A
namespace Sage.Connector.LinkedSource
{
    internal sealed class ServiceConstants
    {
        public const String V1_SERVICE_NAMESPACE = "http://Sage.CRE.CloudConnector.com/2011/11";
    }
}
