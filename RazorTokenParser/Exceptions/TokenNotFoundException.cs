using System;

namespace SimpleTokenParser.Exceptions
{
    public class TokenNotFoundException : Exception
    {
        public TokenNotFoundException(string message) : base(message) { }
    }
}
