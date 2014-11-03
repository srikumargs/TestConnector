using System;
using System.ComponentModel.DataAnnotations;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.DomainContracts.Data
{
    /// <summary>
    /// External Reference information for the referenced entity
    /// </summary>
    public class ExternalReference
    {
        // Large enough without warning on unique index for Sql migration script
        private const int ExternalIdMaxLength = 400; 

        /// <summary>
        /// Not to be touched by Back office
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Back office External Id
        /// </summary>
        [ExternalIdReference, MaxLength(ExternalIdMaxLength)]
        public string ExternalId { get; set; }
    }
}
