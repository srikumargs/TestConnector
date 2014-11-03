using System;

namespace Sage.Connector.DomainContracts.Data.Attributes
{
    /// <summary>
    /// External Id reference identifier attribute for the data contract classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExternalIdReferenceAttribute : Attribute
    {
    }
}
