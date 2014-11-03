using System;

namespace Sage.Connector.Customers.Contracts.BackOffice.Utilities
{
    /// <summary>
    /// Transforms a name field into its First and Last name components.
    /// </summary>
    public class NameTransform
    {
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Full Name
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Transforms a name field into its First and Last name components.
        /// </summary>
        /// <param name="name">The name field that will be transformed.</param>
        private NameTransform(string name)
        {
            FullName = name;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                string[] nameSplit = name.Trim().Replace("  ", " ").Split(' ');

                if (nameSplit.Length == 1)
                {
                    //If only one name is specified, we will assume it's the First name
                    FirstName = nameSplit[nameSplit.Length - 1];
                }
                else
                {
                    FirstName = string.Join(" ", nameSplit, 0, nameSplit.Length - 1);
                    LastName = nameSplit[nameSplit.Length - 1];
                }
            }
        }

        /// <summary>
        /// Transforms a name field into its First and Last name components.
        /// </summary>
        /// <param name="firstName">The first Name field that will be transformed.</param>
        /// <param name="lastName">The last Name field that will be transformed.</param>
        private NameTransform(string firstName, string lastName)
        {
            FirstName = (firstName ?? "").Trim();
            LastName = (lastName ?? "").Trim();
            FullName = String.Format("{0} {1}", FirstName, LastName).Trim();
        }

        /// <summary>
        /// Transforms a name field into its First and Last name components.
        /// </summary>
        /// <param name="name">The name field that will be transformed.</param>
        /// <returns>A <see cref="NameTransform"/></returns>
        public static NameTransform TransformName(string name)
        {
            return new NameTransform(name);
        }


        /// <summary>
        /// Transforms a first and last name field into its full name.
        /// </summary>
        /// <param name="firstName">The first Name field that will be transformed.</param>
        /// <param name="lastName">The last Name field that will be transformed.</param>
        /// <returns>A <see cref="NameTransform"/></returns>
        public static NameTransform TransformNameDown(string firstName, string lastName)
        {
            return new NameTransform(firstName, lastName);
        }
    }
}
