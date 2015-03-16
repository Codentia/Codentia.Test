using System.Xml;
using Codentia.Test.Reflector;

namespace Codentia.Test.DataLoader
{
    /// <summary>
    /// DataLoad Method
    /// </summary>
    internal class DataLoadMethod
    {
        private Method _method = null;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadMethod"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="defaultAssemblyName">Default name of the assembly.</param>
        /// <param name="methodNode">The method node.</param>
        /// <param name="parametersNode">The parameters node.</param>
        /// <param name="parentDataMethod">The parent data method.</param>
        public DataLoadMethod(string database, string defaultAssemblyName, XmlNode methodNode, XmlNode parametersNode, DataLoadMethod parentDataMethod)
        {
            string methodName = methodNode.Name;
            string assemblyName = defaultAssemblyName;
            if (methodNode.Attributes["assembly"] != null)
            {
                assemblyName = methodNode.Attributes["assembly"].Value;
            }

            string className = methodNode.Attributes["class"].Value;
            bool useTransaction = false;

            if (methodNode.Attributes["usetransaction"] != null)
            {
                useTransaction = methodNode.Attributes["usetransaction"].Value.ToLower() == "true";
            }

            string signature = string.Empty;
            if (methodNode.Attributes["signature"] != null)
            {
                signature = methodNode.Attributes["signature"].Value;
            }

            XmlNodeList parsNodeList = null;
            if (methodNode.HasChildNodes)
            {
                parsNodeList = methodNode.SelectNodes("params");
            }

            Method parentMethod = null;
            if (parentDataMethod != null)
            {
                parentMethod = parentDataMethod.Method;
            }

            _method = new Method(database, assemblyName, className, methodName, signature, useTransaction, parametersNode, parentMethod);
        }

        /// <summary>
        /// Gets Method
        /// </summary>
        public Method Method
        {
            get { return _method; }
        }

        /// <summary>
        /// Invoke Method with parameter values derived from constructor
        /// </summary>
        /// <returns>object invoked</returns>
        public object Invoke()
        {
            return _method.Invoke();
        }
    }
}
