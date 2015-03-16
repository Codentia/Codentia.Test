using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Xml;
using Codentia.Test.Helper;

namespace Codentia.Test.Reflector
{
    /// <summary>
    /// Class for creating a dynamic method call with parameters
    /// </summary>
    public class Method
    {
        private const string DefaultStringValue = "MYTestSTRINGdEFAULTvaluE";               

        private Type _currentClass;
        private MethodInfo _methodInfo;
        private Type[] _types;
        private DataTable _parameterDataTable;
        private bool _isOverload;
        private string _database;
        private string _defaultValues;
        private string _omittedParameters;
        private bool _hasTransaction = false;
        private bool _useTransaction = false;
        private Guid _transactionId = Guid.Empty;
        private List<object> _argumentlist = new List<object>();
        private List<string> _omittedParametersList = new List<string>();
        private bool _isChild = false;
        private Method _parentMethod = null;
        private object _returnVar;

        /// <summary>
        /// Initializes a new instance of the <see cref="Method"/> class.
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="assemblyName">Assembly dll name e.g. Codentia.Test.Test.dll</param>
        /// <param name="className">Fully qualified class name e.g. Codentia.Test.Test.TestClass</param>
        /// <param name="methodName">Method Name e.g. OneCheckedStringParamWithLength</param>
        /// <param name="signature">For uses with overloads - comma delimited list of types to match overload being tested</param>
        /// <param name="defaultValues">comma delimited name/value pairs with = e.g. myId1=[TestTable1],myId2=[TestTable3] - use [] for the table for existant and non-existant ids</param>
        /// <param name="omittedParameters">comma delimited list of int or string parameters to be omitted from negative tests</param>  
        public Method(string database, string assemblyName, string className, string methodName, string signature, string defaultValues, string omittedParameters)
        {
            _database = database;
            _defaultValues = defaultValues;
            _omittedParameters = omittedParameters;
            SetUpProperties(assemblyName, className, methodName, signature);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Method"/> class.
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="assemblyName">Assembly dll name e.g. Codentia.Test.Test.dll</param>
        /// <param name="className">Fully qualified class name e.g. Codentia.Test.Test.TestClass</param>
        /// <param name="methodName">Method Name e.g. OneCheckedStringParamWithLength</param>
        /// <param name="signature">For uses with overloads - comma delimited list of types to match overload being tested</param>        
        /// <param name="useTransaction">must have a Guid parameter as the first param and called txnId</param>
        /// <param name="parametersNode">current params node</param>      
        /// <param name="parentMethod">optional parent Method</param>
        public Method(string database, string assemblyName, string className, string methodName, string signature, bool useTransaction, XmlNode parametersNode, Method parentMethod)
        {
            _database = database;
            _useTransaction = useTransaction;
            if (parentMethod != null)
            {
                _parentMethod = parentMethod;
                _isChild = true;
                _useTransaction = parentMethod.UseTransaction;
            }

            SetUpProperties(assemblyName, className, methodName, signature);
            _parentMethod = parentMethod;
            if (parametersNode != null)
            {
                SetParameterTableFromNodeList(parametersNode);
            }
        }

        /// <summary>
        /// Gets Database
        /// </summary>
        public string Database
        {
            get { return _database; }
        }

        /// <summary>
        /// Gets ReturnVar
        /// </summary>
        public object ReturnVar
        {
            get { return _returnVar; }
        }

        /// <summary>
        /// Gets Name of Method
        /// </summary>
        public string Name
        {
            get { return _methodInfo.Name; }
        }

        /// <summary>
        /// Gets a value indicating whether	the Method Is an Overload
        /// </summary>
        public bool IsOverload
        {
            get { return _isOverload; }
        }

        /// <summary>
        /// Gets TransactionId
        /// </summary>
        public Guid TransactionId
        {
            get { return _transactionId; }
        }

        /// <summary>
        /// Gets a value indicating whether	the Methods Has a Transaction
        /// </summary>
        public bool HasTransaction
        {
            get { return _hasTransaction; }
        }

        /// <summary>
        /// Gets a value indicating whether	to Use a Transaction
        /// </summary>
        public bool UseTransaction
        {
            get { return _useTransaction; }
        }

        /// <summary>
        /// Gets ParameterDataTable
        /// </summary>
        public DataTable ParameterDataTable
        {
            get { return _parameterDataTable; }
        }

        /// <summary>
        /// Invoke Method with parameter values derived from constructor
        /// </summary>
        /// <returns>object invoked</returns>
        public object Invoke()
        {
            _returnVar = Invoke(GetArgumentArray(true));
            return _returnVar;
        }

        /// <summary>
        /// Invoke Method with specific arguments object array
        /// </summary>
        /// <param name="arguments">object array of arguments</param>
        /// <returns>object invoked</returns>
        public object Invoke(object[] arguments)
        {
            return _currentClass.InvokeMember(_methodInfo.Name, BindingFlags.InvokeMethod, null, null, arguments);
        }

        /// <summary>
        /// Gets the argument array.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="useValue">The use value.</param>
        /// <returns>object array</returns>
        public object[] GetArgumentArray(string paramName, string useValue)
        {
            List<object> list = new List<object>();

            for (int i = 0; i < _parameterDataTable.Rows.Count; i++)
            {
                DataRow dr = _parameterDataTable.Rows[i];
                string paramType = Convert.ToString(dr["type"]);

                // do not include &&returnvar param
                if (Convert.ToString(dr["name"]) == "&&returnvar")
                {
                    break;
                }

                if (paramName == Convert.ToString(dr["name"]))
                {
                    dr["useValue"] = TypeHelper.GetValueAsType(useValue, paramType);
                }
                else
                {
                    string defaultValue = Convert.ToString(dr["defaultValue"]);
                    dr["useValue"] = defaultValue;

                    switch (paramType)
                    {
                        case "System.Int32":
                            if (defaultValue.Contains("[") && defaultValue.Contains("]"))
                            {
                                dr["useValue"] = SqlHelper.GetDatabaseValueFromToken(_database, defaultValue);
                            }

                            break;
                        case "System.DateTime":
                            if (defaultValue.ToLower() == "now")
                            {
                                dr["UseValue"] = DateTime.Now;
                            }

                            break;
                    }
                }
            }

            return GetArgumentArray(false);
        }

        private void SetUpProperties(string assemblyName, string className, string methodName, string signature)
        {
            _currentClass = AssemblyHelper.GetClass(assemblyName, className);            
            _types = TypeHelper.GetTypesArray(signature);
            _methodInfo = AssemblyHelper.GetMethod(_currentClass, methodName, _types, out _isOverload);

            ParameterInfo[] pars = _methodInfo.GetParameters();

            if (!string.IsNullOrEmpty(_omittedParameters))
            {
                string[] omitArray = _omittedParameters.Split(',');
                if (omitArray.Length > 0)
                {
                    for (int i = 0; i < omitArray.Length; i++)
                    {
                        if (omitArray[i] != string.Empty)
                        {
                            _omittedParametersList.Add(omitArray[i].ToLower());
                        }
                    }
                }
            }

            SetUpParamDataTable();
        }
       
        private void SetUpParamDataTable()
        {
            _parameterDataTable = new DataTable();
            _parameterDataTable.Columns.Add(new DataColumn("name", typeof(string)));
            _parameterDataTable.Columns.Add(new DataColumn("omitted", typeof(bool)));
            _parameterDataTable.Columns.Add(new DataColumn("defaultvalue", typeof(string)));
            _parameterDataTable.Columns.Add(new DataColumn("usevalue", typeof(string)));
            _parameterDataTable.Columns.Add(new DataColumn("type", typeof(string)));
            _parameterDataTable.Columns.Add(new DataColumn("txncolumn", typeof(bool)));
            PopulateDataTable();
        }

        private void PopulateDataTable()
        {
            // create a basic row for each parameter
            foreach (ParameterInfo p in _methodInfo.GetParameters())
            {
                DataRow dr = _parameterDataTable.NewRow();

                dr["name"] = p.Name;
                dr["txncolumn"] = false;

                if (p.Name == "txnId")
                {
                    _hasTransaction = true;
                    dr["txncolumn"] = true;
                    UpdateTableValues(dr, Guid.Empty.ToString());
                }

                dr["type"] = p.ParameterType.ToString();
                
                dr["omitted"] = false;

                if (_omittedParametersList != null)
                {
                    dr["omitted"] = _omittedParametersList.Contains(p.Name.ToLower());
                }

                if (Convert.ToBoolean(dr["txncolumn"]))
                {
                    if (_useTransaction && _hasTransaction)
                    {
                        if (_isChild)
                        {
                            _useTransaction = true;
                            _transactionId = _parentMethod.TransactionId;
                        }
                        else
                        {
                            _transactionId = Guid.NewGuid();
                        }

                        UpdateTableValues(dr, _transactionId.ToString());                        
                    }
                }
                else
                {
                    UpdateTableValues(dr, TypeHelper.GetDefaultValueForType(p.ParameterType.ToString()));                     
                }

                _parameterDataTable.Rows.Add(dr);
            }

            // Add returnvar Row
            DataRow rowRV = _parameterDataTable.NewRow();
            rowRV["name"] = "&&returnvar";
            rowRV["type"] = _methodInfo.ReturnType;
            rowRV["omitted"] = true;
            rowRV["txncolumn"] = false;

            _parameterDataTable.Rows.Add(rowRV);            

            if (!string.IsNullOrEmpty(_defaultValues))
            {
                string[] defaultValueArray = _defaultValues.Split(',');

                for (int i = 0; i < defaultValueArray.Length; i++)
                {
                    string[] valueDict = defaultValueArray[i].Split('=');

                    string name = valueDict[0];

                    for (int j = 0; j < _parameterDataTable.Rows.Count; j++)
                    {
                        DataRow dr = _parameterDataTable.Rows[j];
                        if (name == Convert.ToString(dr["name"]))
                        {
                            UpdateTableValues(dr, valueDict[1]);
                        }
                        else
                        {
                            // output a warning if names are the same apart from casing
                            if (name.ToLower() == Convert.ToString(dr["name"]).ToLower() || name.Trim() == Convert.ToString(dr["name"]))
                            {
                                Console.Out.WriteLine("**WARNING** Possible Case or Space issue: Data-Name='{0}', Reflection-Name='{1}'", name, dr["name"]); 
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTableValues(DataRow dr, string value)
        {
            dr["defaultvalue"] = value;
            dr["usevalue"] = value;
        }

        private void SetParameterTableFromNodeList(XmlNode parametersNode)
        {
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            
            if (parametersNode.Attributes.Count > 0)
            {
                for (int i = 0; i < parametersNode.Attributes.Count; i++)
                {
                    parameterList.Add(parametersNode.Attributes[i].Name, parametersNode.Attributes[i].Value);
                }
            }

            if (parametersNode.ChildNodes.Count > 0)
            {
                XmlNodeList paramNodes = parametersNode.ChildNodes;

                for (int i = 0; i < paramNodes.Count; i++)
                {
                    parameterList.Add(paramNodes[i].Name, parametersNode.InnerText);
                }
            }

            for (int j = 0; j < _parameterDataTable.Rows.Count; j++)
            {
                DataRow dr = _parameterDataTable.Rows[j];
                string paramName = Convert.ToString(dr["name"]);
                IEnumerator<string> ie = parameterList.Keys.GetEnumerator();
                while (ie.MoveNext())
                {
                    if (Convert.ToBoolean(dr["txncolumn"]) == false)
                    {
                        if (paramName == ie.Current)
                        {
                            UpdateTableValues(dr, GetParameterValue(parameterList[ie.Current]));
                        }
                    }
                }
            }
        }

        private object[] GetArgumentArray(bool useDefaultValues)
        {
            // iterate _parameterDataTable and return object array
            List<object> list = new List<object>();

            for (int i = 0; i < _parameterDataTable.Rows.Count; i++)
            {
                DataRow dr = _parameterDataTable.Rows[i];

                if (Convert.ToString(dr["name"]) == "&&returnvar")
                {
                    break;
                }
                
                string type = Convert.ToString(dr["type"]);
                string value = Convert.ToString(dr["defaultValue"]);
                if (Convert.ToBoolean(dr["omitted"]) == false)
                {                    
                    if (useDefaultValues == false)
                    {
                        value = Convert.ToString(dr["useValue"]);
                    }
                }

                list.Add(TypeHelper.GetValueAsType(value, type));
            }

            return list.ToArray();
        }
        
        private string GetParameterValue(string value)
        {
            string retVal = value;

            switch (value)
            {
                case "[~RandomGuid~]":
                    retVal = retVal.Replace("[~RandomGuid~]", Guid.NewGuid().ToString());
                    break;
                case "[~Database~]":
                    retVal = retVal.Replace("[~Database~]", _database);
                    break;
                case "[~ParentId~]":
                    retVal = retVal.Replace("[~ParentId~]", _parentMethod.ReturnVar.ToString());
                    break;
            }

            return retVal;
        }
    }
}