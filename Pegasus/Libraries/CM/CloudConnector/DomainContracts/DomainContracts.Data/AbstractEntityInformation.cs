using System.ComponentModel.DataAnnotations;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.DomainContracts.Data
{
    /// <summary>
    /// The Getters and Setters for Entity information
    /// </summary>
    public abstract class AbstractEntityInformation
    {
        private const int ExternalIdMaxLength = 400;
        /// <summary>
        /// ExternalId into the customer data
        /// </summary>
        [ExternalIdentifier, MaxLength(ExternalIdMaxLength)]
        public string ExternalId { get; set; }

        /// <summary>
        /// User facing exernal id reference
        /// </summary>
        [MaxLength(ExternalIdMaxLength)]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Entity Status <see cref="EntityStatus"/>
        /// </summary>
        public EntityStatus EntityStatus { get; set; }
    }
}
