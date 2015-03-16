using System;

namespace Codentia.Test
{
    /// <summary>
    /// Use this to mark code as excluded from coverage - for test purposes only
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class CoverageExcludeAttribute : Attribute 
    { 
    }
}
