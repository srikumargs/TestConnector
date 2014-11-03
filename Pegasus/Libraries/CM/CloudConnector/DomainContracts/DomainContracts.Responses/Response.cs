

namespace Sage.Connector.DomainContracts.Responses
{
    /// <summary>
    /// Status for a standard contract response
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Not yet set. Default value 
        /// </summary>
        Indeterminate = 0,

        /// <summary>
        /// Success indicates that although there may be diagnosis messages,
        /// the action fulfilled the request, so accept it.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Failure indicates that something happened whether or not there are messages in the 
        /// diagnosis list.   
        /// </summary>
        Failure = 2
    }
    /// <summary>
    ///  Response used by all response contracts
    /// </summary>
    public  class Response
    {
        /// <summary>
        /// default value is Indeterminate.
        /// </summary>
        Status _statusCode = Status.Indeterminate;

        private Diagnoses _diagnoses;

        /// <summary>
        /// <see cref="Status"/>  The default is Indeterminate
        /// </summary>
        public Status Status
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        /// <summary>
        /// <see cref="Diagnoses"/> is a list of <see cref="Diagnosis"/>
        /// </summary>
        public Diagnoses Diagnoses
        {
            get { return _diagnoses ?? (_diagnoses = new Diagnoses()); }
            set { _diagnoses = value; }
        }
    }
}
