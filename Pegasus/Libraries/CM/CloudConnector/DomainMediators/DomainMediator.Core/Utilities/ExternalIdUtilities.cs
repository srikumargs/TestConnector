using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.DomainMediator.Core.Utilities
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class ExternalIdUtilities
    {

        ///<summary>
        /// Use base 64 encoding to encode an external ID.
        ///</summary>
        public static String EncodeExternalId(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return HttpServerUtility.UrlTokenEncode(encbuff);
        }

        ///<summary>
        /// Use base 64 encoding to decode an external ID.
        ///</summary>
        public static String DecodeExternalId(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            try
            {
                byte[] decbuff = HttpServerUtility.UrlTokenDecode(str);
                return decbuff != null ? Encoding.UTF8.GetString(decbuff) : null;
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// Base 64 Url Encode the value for this property
        /// </summary>
        /// <param name="propInfo"></param>
        /// <param name="obj"></param>
        public static void Base64UrlEncode(this PropertyInfo propInfo, Object obj)
        {
            Object propValue = propInfo.GetValue(obj);
            if (propValue != null)
            {
                propInfo.SetValue(obj, propValue.ToString().EncodeExternalId());
            }
        }

        /// <summary>
        /// Apply encoding to all External Ids of the object 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void ApplyExternalIdEncoding<T>(T obj) where T : class
        {
            if (obj == null)
                return;

            foreach (var property in obj.GetType().GetProperties())
            {

                if (property.GetCustomAttributes(typeof(ExternalIdentifierAttribute), true).Length != 0)
                {
                    property.Base64UrlEncode(obj);
                    continue;
                }

                if (property.GetCustomAttributes(typeof(ExternalIdReferenceAttribute), true).Length != 0)
                {
                    property.Base64UrlEncode(obj);
                    continue;
                }

                var value = property.GetValue(obj);
// ReSharper disable UseIsOperator.2
                if (typeof(ICollection).IsInstanceOfType(value) || typeof(ICollection<>).IsInstanceOfType(value))
// ReSharper restore UseIsOperator.2
                {
                    foreach (var collItem in (IEnumerable) value)
                    {
                        ApplyExternalIdEncoding(collItem);
                    }
                    continue;
                }


                if (property.PropertyType.IsValueType 
                    || property.PropertyType.IsPrimitive 
                    || property.PropertyType.IsAssignableFrom(typeof(String)))
                    continue;

                ApplyExternalIdEncoding(property.GetValue(obj));
            }
        }



        /// <summary>
        /// Base 64 Url Decode the value for this property
        /// </summary>
        /// <param name="propInfo"></param>
        /// <param name="obj"></param>
        public static void Base64UrlDecode(this PropertyInfo propInfo, Object obj)
        {
            Object propValue = propInfo.GetValue(obj);
            if (propValue != null)
            {
                propInfo.SetValue(obj, propValue.ToString().DecodeExternalId());
            }
        }

        /// <summary>
        /// Apply decoding of all ExternalIds of the object 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void ApplyExternalIdDecoding<T>(T obj) where T: class
        {
            if (obj == null)
                return; 


            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.GetCustomAttributes(typeof (ExternalIdentifierAttribute), true).Length != 0)
                {
                    property.Base64UrlDecode(obj);
                    continue;
                }
                if (property.GetCustomAttributes(typeof(ExternalIdReferenceAttribute), true).Length != 0)
                {
                    property.Base64UrlDecode(obj);
                    continue;
                }

                var value = property.GetValue(obj);
                // ReSharper disable UseIsOperator.2
                if (typeof(ICollection).IsInstanceOfType(value) || typeof(ICollection<>).IsInstanceOfType(value))
                // ReSharper restore UseIsOperator.2
                {
                    foreach (var collItem in (IEnumerable)value)
                    {
                        ApplyExternalIdDecoding(collItem);
                    }
                    continue;
                }


                if (property.PropertyType.IsValueType
                    || property.PropertyType.IsPrimitive
                    || property.PropertyType.IsAssignableFrom(typeof(String)))
                    continue;

                ApplyExternalIdDecoding(property.GetValue(obj));
            }
        }
    }

}

