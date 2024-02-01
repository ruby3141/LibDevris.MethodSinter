using System;

namespace Devris.LibDevris.MethodSinter
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class SinteredMethodAttribute : Attribute
    {
        public string Identifier { get; private set; }

        public SinteredMethodAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}
