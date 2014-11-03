using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Taxes.Contracts.Data
{
    /// <summary>
    /// Tax Schedule
    /// </summary>
    public class TaxSchedule: AbstractEntityInformation
    {
        private ICollection<TaxScheduleDetail> _details;

        /// <summary>
        /// Tax Schedule Description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Tax Schedule Details
        /// </summary>
        public ICollection<TaxScheduleDetail> Details
        {
            get { return _details ?? (_details = new Collection<TaxScheduleDetail>()); }
            set { _details = value; }
        }
    }
}
