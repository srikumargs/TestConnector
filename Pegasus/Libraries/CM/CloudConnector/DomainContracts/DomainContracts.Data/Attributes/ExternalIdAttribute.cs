using System;

namespace Sage.Connector.DomainContracts.Data.Attributes
{
    /// <summary>
    /// External Id identifier attribute for the data contract classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExternalIdentifierAttribute : Attribute
    {
    }
}
