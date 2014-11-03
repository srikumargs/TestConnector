using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Sage.Connector.Sync
{
    /// <summary>
    ///     Static utility class for generating an object's hash value.
    /// </summary>
    public static class SyncHash
    {
        private static readonly DataContractSerializerSettings _settings = new DataContractSerializerSettings();

        /// <summary>
        ///     Static initializer to allow for circular references in the object model.
        /// </summary>
        static SyncHash()
        {
            _settings.PreserveObjectReferences = true;
        }

        /// <summary>
        ///     Computes the hash value for the specified object using the generic T crypto service provider.
        /// </summary>
        /// <typeparam name="T">The crypto service provider type that will compute the hash value.</typeparam>
        /// <param name="instance">The object instance to perform the hash on.</param>
        /// <returns>The object's hash as a base 64 encoded string value.</returns>
        private static string GetHash<T>(object instance) where T : HashAlgorithm, new()
        {
            var serializer = new DataContractSerializer(instance.GetType(), _settings);

            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, instance);

                using (var cryptoServiceProvider = new T())
                {
                    return Convert.ToBase64String(cryptoServiceProvider.ComputeHash(memoryStream.ToArray()));
                }
            }
        }

        /// <summary>
        ///     Computes the hash value for the specified object using the provider specified by the enumerated provider type.
        /// </summary>
        /// <param name="instance">The object instance to perform the hash on.</param>
        /// <returns>The object's hash as a base 64 encoded string value.</returns>
        public static string GetHash(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            /*  The service provider can be swapped out for any one of:
             * 
             *  MD5CryptoServiceProvider
             *  SHA1CryptoServiceProvider
             *  SHA256CryptoServiceProvider
             *  SHA384CryptoServiceProvider
             *  SHA512CryptoServiceProvider
             */
            return GetHash<MD5CryptoServiceProvider>(instance);
        }
    }
}