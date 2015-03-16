using System;
using System.Xml;
using Codentia.Common.Data;
using Codentia.Common.Helper;

namespace Codentia.Test.DataLoader
{
    /// <summary>
    /// An Xml Document
    /// </summary>
    public class DataLoadDocument
    {
        private XmlDocument _xmlDoc = new XmlDocument();
        private string _database;
        private string _defaultAssembly;
        private XmlNode _rootNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadDocument"/> class.
        /// </summary>
        /// <param name="database">The data source.</param>
        /// <param name="xmlFileAndPath">The XML file and path.</param>
        public DataLoadDocument(string database, string xmlFileAndPath)
        {
            ParameterCheckHelper.CheckIsValidString(database, "database", false);
            ParameterCheckHelper.CheckIsValidString(xmlFileAndPath, "xmlFileAndPath", false);
            
            _xmlDoc.Load(xmlFileAndPath);
            _database = database;

            _rootNode = _xmlDoc.ChildNodes[0];

            _defaultAssembly = _rootNode.Attributes["defaultassembly"].Value;
        }

        /// <summary>
        /// Process the DataLoad Document
        /// </summary>
        /// <returns>XmlDocument object</returns>
        public XmlDocument Execute()
        {
            if (_rootNode.HasChildNodes)
            {
                for (int i = 0; i < _rootNode.ChildNodes.Count; i++)
                {
                    XmlNode currentNode = _rootNode.ChildNodes[i];
                    ProcessNode(currentNode, null);
                }
            }

            return _xmlDoc;
        }

        private void ProcessNode(XmlNode currentMethodNode, DataLoadMethod parentMethod)
        {
            XmlNodeList paramsNodeList = currentMethodNode.SelectNodes("params");

            // zero param call
            if (paramsNodeList.Count == 0)
            {
                ProcessMethod(currentMethodNode, null, parentMethod);                
                return;
            }

            // method with params
            for (int i = 0; i < paramsNodeList.Count; i++)
            {
                XmlNode parametersNode = paramsNodeList[i];
                ProcessMethod(currentMethodNode, parametersNode, parentMethod);    
            }
        }

        private void ProcessMethod(XmlNode currentMethodNode, XmlNode parametersNode, DataLoadMethod parentMethod)
        {
            DataLoadMethod method = new DataLoadMethod(_database, _defaultAssembly, currentMethodNode, parametersNode, parentMethod);
               
            try
            {
                object returnvar = method.Invoke();
                if (parametersNode != null)
                {
                    if (parametersNode.Attributes["returnvar"] != null)
                    {
                        parametersNode.Attributes["returnvar"].Value = returnvar.ToString();
                    }
                }

                XmlNode childMethodsNode = currentMethodNode.SelectSingleNode("childmethods");
                if (childMethodsNode != null)
                {
                    if (childMethodsNode.ChildNodes.Count > 0)
                    {
                        for (int j = 0; j < childMethodsNode.ChildNodes.Count; j++)
                        {
                            XmlNode methodNode = childMethodsNode.ChildNodes[j];
                            ProcessNode(methodNode, method);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 if (parentMethod == null && method.Method.HasTransaction)
                 {
                     DbInterface.RollbackTransaction(method.Method.TransactionId);
                 }

                 throw ex;
            }

            if (parentMethod == null && method.Method.HasTransaction)
            {
                DbInterface.CommitTransaction(method.Method.TransactionId);
            }
        }
    }
}
