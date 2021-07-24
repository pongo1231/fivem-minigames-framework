using System.Reflection;
using System;
using System.Collections.Generic;

namespace GamemodesShared.Utils
{
    /// <summary>
    /// Reflection related utils
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Finds and returns all methods in the specified class
        /// with the specified attribute attached
        /// </summary>
        /// <typeparam name="MethodType">Type of returned methods</typeparam>
        /// <param name="_instance">Instance of class</param>
        /// <param name="_attributeType">Type of attribute to filter by</param>
        /// <returns>Methods in specified class with specified attribute attached</returns>
        public static MethodType[] GetAllMethodsWithAttributeForClass<MethodType>(dynamic _instance,
            Type _attributeType)
        {
            List<MethodType> funcs = new List<MethodType>();

            // Helper function for creating a delegate out of method info
            Func<MethodInfo, MethodType> createDelegate = (_methodInfo) =>
            {
                return _methodInfo.IsStatic
                    ? Delegate.CreateDelegate(typeof(MethodType), _methodInfo)
                    : Delegate.CreateDelegate(typeof(MethodType), _instance, _methodInfo);
            };

            // Go through all functions of child (and all inherited) class(es) via reflection
            foreach (MethodInfo methodInfo in _instance.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                | BindingFlags.Instance))
            {
                if (methodInfo.GetCustomAttribute(_attributeType) != null)
                {
                    funcs.Add(createDelegate(methodInfo));
                }
            }

            return funcs.ToArray();
        }

        /// <summary>
        /// Finds and returns all methods in the specified class
        /// with the specified attribute attached and adds them to the given object
        /// </summary>
        /// <typeparam name="MethodType">Type of returned methods</typeparam>
        /// <param name="_instance">Instance of class</param>
        /// <param name="_attributeType">Type of attribute to filter by</param>
        /// <param name="_toAddTo">Object to add results to</param>
        public static void GetAllMethodsWithAttributeForClass<MethodType>(dynamic _instance,
            Type _attributeType, ref MethodType _toAddTo)
        {
            foreach (var func in GetAllMethodsWithAttributeForClass<MethodType>(_instance, 
                _attributeType))
            {
                _toAddTo += func;
            }
        }
    }
}
