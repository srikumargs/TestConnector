using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sage.Connector.TestUtilities
{
    public static class DataObjectComparisonUtil
    {
        /// <summary>
        /// Compares the properties of two objects of the same type OR interface type and returns if all properties are equal.
        /// This works for comparison between like contracts in 
        ///    - Sage.Connector.Cloud.Integration.Interfaces.DataContracts   AND 
        ///    - Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts
        ///    
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compare.</param>
        /// <param name="results"> </param>
        /// <param name="startingResult"> </param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static bool AreObjectsEqual(object objectA, object objectB, ref PropertyComparisonResults results, bool startingResult , params string[] ignoreList)
        {

            bool result = true;

            if (objectA != null && objectB != null)
            {
                //If objects are of type list then recurse on each item from the very beginning
                if (typeof(IEnumerable).IsAssignableFrom(objectA.GetType()))
                {
                    IEnumerable<object> valueAList;
                    IEnumerable<object> valueBList;

                    valueAList = ((IEnumerable)objectA).Cast<object>();
                    object[] valueAArray = valueAList.ToArray();
                    valueBList = ((IEnumerable)objectB).Cast<object>();
                    object[] valueBArray = valueBList.ToArray();
                    for (int i = 0; i < valueAArray.Length; i++)
                    {
                        startingResult = AreObjectsEqual(valueAArray[i], valueBArray[i], ref results, startingResult, ignoreList);
                    }
                }
                else
                {
                    Type objectType1, objectType2;
                    objectType1 = objectA.GetType();

                    foreach (PropertyInfo propertyInfo1 in objectType1.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name)))
                    {
                        result = true; // assume by default they are equal
                        objectType2 = objectB.GetType();
                        PropertyInfo propertyInfo2 = objectType2.GetProperty(propertyInfo1.Name);

                        object valueA;
                        object valueB;

                        valueA = propertyInfo1.GetValue(objectA, null);
                        valueB = propertyInfo2.GetValue(objectB, null);

                        // if it is a primative type, value type or implements IComparable, just directly try and compare the value
                        if (CanDirectlyCompare(propertyInfo1.PropertyType) && CanDirectlyCompare(propertyInfo2.PropertyType))
                        {
                            if (!AreValuesEqual(valueA, valueB))
                            {
                                string msg = string.Format("Mismatch with property '{0}.{1}' found.", objectType1.FullName, propertyInfo1.Name);
                                result = false;
                                results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                            }
                            else
                            {
                                results.Add(new PropertyComparisonResult(propertyInfo1.Name, 0));
                            }
                        }
                        // if it implements IEnumerable, then scan any items
                        else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo1.PropertyType))
                        {
                            IEnumerable<object> collectionItems1;
                            IEnumerable<object> collectionItems2;
                            int collectionItemsCount1;
                            int collectionItemsCount2;

                            // null check
                            if (valueA == null && valueB != null || valueA != null && valueB == null)
                            {
                                string msg = string.Format("Mismatch with property '{0}.{1}' found.", objectType1.FullName, propertyInfo1.Name);
                                result = false;
                                results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                            }
                            else if (valueA != null && valueB != null)
                            {
                                collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                                collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                                collectionItemsCount1 = collectionItems1.Count();
                                collectionItemsCount2 = collectionItems2.Count();

                                // check the counts to ensure they match
                                if (collectionItemsCount1 != collectionItemsCount2)
                                {
                                    string msg = string.Format("Collection counts for property '{0}.{1}' do not match.", objectType1.FullName, propertyInfo1.Name);
                                    result = false;
                                    results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                                }
                                // and if they do, compare each item... this assumes both collections have the same order
                                else
                                {
                                    for (int i = 0; i < collectionItemsCount1; i++)
                                    {
                                        object collectionItem1;
                                        object collectionItem2;
                                        Type collectionItemType1;
                                        Type collectionItemType2;

                                        collectionItem1 = collectionItems1.ElementAt(i);
                                        collectionItem2 = collectionItems2.ElementAt(i);
                                        collectionItemType1 = collectionItem1.GetType();
                                        collectionItemType2 = collectionItem2.GetType();

                                        if (CanDirectlyCompare(collectionItemType1))
                                        {
                                            if (!AreValuesEqual(collectionItem1, collectionItem2))
                                            {
                                                string msg = string.Format("Item {0} in property collection '{1}.{2}' does not match.", i, objectType1.FullName, propertyInfo1.Name);
                                                result = false;
                                                results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                                            }
                                            else
                                            {
                                                results.Add(new PropertyComparisonResult(propertyInfo1.Name, 0));
                                            }
                                        }
                                        else if (!AreObjectsEqual(collectionItem1, collectionItem2, ref results, true, ignoreList))
                                        {
                                            string msg = string.Format("Item {0} in property collection '{1}.{2}' does not match.", i, objectType1.FullName, propertyInfo1.Name);
                                            result = false;
                                            results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                                        }
                                    }
                                }
                            }
                        }
                        else if (propertyInfo1.PropertyType.IsClass)
                        {
                            if (!AreObjectsEqual(propertyInfo1.GetValue(objectA, null), propertyInfo2.GetValue(objectB, null), ref results, true, ignoreList))
                            {
                                string msg = string.Format("Mismatch with property '{0}.{1}' found.", objectType1.FullName, propertyInfo1.Name);
                                result = false;
                                results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                            }
                        }
                        else
                        {
                            string msg = string.Format("Cannot compare property '{0}.{1}'.", objectType1.FullName, propertyInfo1.Name);
                            result = false;
                            results.Add(new PropertyComparisonResult(propertyInfo1.Name, result, msg));
                        }
                        //Maintains false if there has ever been a false result in collection.
                        if (!result)
                            startingResult = false;
                    }
                }
            }
            else
            {
                //both objects null
                result = object.Equals(objectA, objectB);
                if (result)
                { results.Add(new PropertyComparisonResult("Null Equality", true, "both object are null")); }
                else
                {
                    results.Add(new PropertyComparisonResult("Null Equality", false, "Objects are somehow both null but not equal"));
                    startingResult = false;
                }
            }

            return startingResult;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// NOTE: objects of Cloud.DataContract. will not be comparible to the same object in CloudConnector.DataContract
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this value instances of the specified type can be directly compared; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match, otherwise <c>false</c>.</returns>
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result = true;
            IComparable selfValueComparerA;
            IComparable selfValueComparerB;

            selfValueComparerA = valueA as IComparable;
            selfValueComparerB = valueB as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
            {
                result = false; // one of the values is null
            }
            else if (selfValueComparerA != null && selfValueComparerB != null)
            {
                if (selfValueComparerA.GetType() == selfValueComparerB.GetType())
                {
                    // the comparison using IComparable (only works on non-data contract specific types)
                    result = selfValueComparerA.CompareTo(valueB) == 0;
                }
                else
                {
                    // the comparison using the value of each comparer
                    result = selfValueComparerA.ToString().Equals(selfValueComparerB.ToString());
                }
            }
            else if (!object.Equals(valueA, valueB))
            {
                result = false; // the comparison using Equals failed
            }
            else
            {
                result = true; // match
            }

            return result;
        }

    }

    public class PropertyComparisonResults
    {
        public PropertyComparisonResults()
        {
            ResultList = new List<PropertyComparisonResult>();
        }

        public List<PropertyComparisonResult> ResultList { get; private set; }

        public void Add(PropertyComparisonResult item)
        {
            ResultList.Add(item);
        }

        public void Clear()
        {
            ResultList.Clear();
        }

        public void Add(string propertyName, bool exception)
        {
            ResultList.Add(new PropertyComparisonResult(propertyName, exception));
        }

        public void Add(string propertyName, int result)
        {
            ResultList.Add(new PropertyComparisonResult(propertyName, result));
        }

        public void AddRange(PropertyComparisonResults addedResults)
        {
            foreach (PropertyComparisonResult r in addedResults.ResultList)
            {
                ResultList.Add(r);
            }
        }

        //Failure results
        // NOTE: failures may be subproperty failure which can propogate up.
        public List<PropertyComparisonResult> Failures
        {
            get { return (ResultList.FindAll(x => x.AreEqual == false)); }
        }
    }

    public class PropertyComparisonResult
    {
        public PropertyComparisonResult(string propertyName, bool result, string msg = "")
        {
            PropertyName = propertyName;
            _result = result ? 0 : -1;
            Message = msg;
        }

        public PropertyComparisonResult(string propertyName, int result, string msg = "")
        {
            PropertyName = propertyName;
            _result = result;
            Message = msg;
        }
        private int _result;
        public string PropertyName { get; private set; }
        public string Message { get; set; }
        public bool AreEqual
        {
            get
            {
                return _result == 0;
            }
        }
        public bool ExceptionOccured { get; set; }
    }

}
