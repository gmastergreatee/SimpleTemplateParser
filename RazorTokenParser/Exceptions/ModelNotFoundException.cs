using System;

namespace SimpleTokenParser.Exceptions
{
    public class ModelNotFoundException : Exception
    {
        public ModelNotFoundException(string message) : base(message) { }
    }
}
