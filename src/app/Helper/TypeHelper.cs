using System;
using System.Collections.Generic;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// Class for use with dynamic types
    /// </summary>
    public class TypeHelper
    {
        /// <summary>
        /// Return a value as a converted baseType
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="type">Type to convert to</param>
        /// <returns>object after conversion</returns>
        public static object GetValueAsType(string value, string type)
        {
            object obj = null;

            switch (type)
            {
                case "System.Int32":
                    obj = Convert.ToInt32(value);
                    break;
                case "System.Byte":
                    obj = Convert.ToByte(value);
                    break;
                case "System.String":
                    obj = value;
                    break;
                case "System.Boolean":
                    obj = Convert.ToBoolean(value);
                    break;
                case "System.DateTime":
                    obj = Convert.ToDateTime(value);
                    break;
                case "System.Guid":
                    obj = new Guid(value);
                    break;
                case "System.Decimal":
                    obj = Convert.ToDecimal(value);
                    break;
            }

            return obj;
        }

        /// <summary>
        /// return a default value for a type
        /// </summary>
        /// <param name="type">Type to process</param>
        /// <returns>string of the type</returns>
        public static string GetDefaultValueForType(string type)
        {
            string retVal = string.Empty;

            switch (type)
            {
                case "System.Int32":
                    retVal = "1";
                    break;
                case "System.Byte":
                    retVal = "1";
                    break;
                case "System.String":
                    retVal = "MYTestSTRINGdEFAULTvaluE";
                    break;
                case "System.Boolean":
                    retVal = "true";
                    break;
                case "System.DateTime":
                    retVal = DateTime.Now.ToString();
                    break;
                case "System.Guid":
                    retVal = Guid.NewGuid().ToString();
                    break;
                case "System.Decimal":
                    retVal = "1.25";
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Return a comma delimited list of types as Type Array
        /// </summary>
        /// <param name="typeString">Comma delimited list of types</param>
        /// <returns>Type Array</returns>
        public static Type[] GetTypesArray(string typeString)
        {
            List<Type> list = new List<Type>();
            if (!string.IsNullOrEmpty(typeString))
            {
                list = new List<Type>();
                string[] types = typeString.Split(',');
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = Type.GetType(types[i]);
                    list.Add(type);
                }

                return list.ToArray();
            }

            return null;
        }
    }
}
