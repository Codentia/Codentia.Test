using System;
using System.Diagnostics;
using System.Reflection;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// Methods for helping create objects using Reflection
    /// </summary>
    public class AssemblyHelper
    {
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="currentClass">The current class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="types">The types.</param>
        /// <param name="isOverload">if set to <c>true</c> [is overload].</param>
        /// <returns>MethodInfo object</returns>
        public static MethodInfo GetMethod(Type currentClass, string methodName, Type[] types, out bool isOverload)
        {
            isOverload = false;
            MethodInfo method = null;
            if (types == null)
            {
                method = currentClass.GetMethod(methodName);
            }
            else
            {
                method = currentClass.GetMethod(methodName, types);
                isOverload = true;
            }

            if (method == null)
            {
                throw new Exception(string.Format("No method matching name: {0} and type signature exists in class: {1}", methodName, currentClass.FullName));
            }

            return method;
        }

        /// <summary>
        /// Get Base Object
        /// </summary>
        /// <param name="currentClass">current Class</param>
        /// <returns>object (base)</returns>
        public static object GetBaseObject(Type currentClass)
        {
            return Activator.CreateInstance(currentClass);
        }

        /// <summary>
        /// Get Class
        /// </summary>
        /// <param name="assemblyName">assembly Name</param>
        /// <param name="className">class Name</param>
        /// <returns>Type object</returns>
        public static Type GetClass(string assemblyName, string className)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            Type currentClass = assembly.GetType(className);
            if (currentClass == null)
            {
                throw new Exception(string.Format("Class: {0} does not exist in Assembly: {1}", className, assemblyName));
            }

            return currentClass;
        }

        /// <summary>
        /// Get Calling Assembly Name
        /// </summary>
        /// <param name="stackTrace">a stack trace from method</param>
        /// <returns>string - assembly name</returns>
        public static string GetCallingAssemblyName(StackTrace stackTrace)
        {
            StackFrame[] stackFrames = stackTrace.GetFrames();

            StackFrame callingFrame = stackFrames[1];            

            return callingFrame.GetMethod().DeclaringType.Module.Name.Replace(".DLL", ".dll");
        }
    }
}
