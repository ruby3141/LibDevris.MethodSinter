using System;

namespace Devris.LibDevris.MethodSinter
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MethodFragmentAttribute : Attribute
    {
        public string Identifier { get; private set; }

        public MethodFragmentAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }
}
