using System;

namespace Mogre.Builder
{
    class UserException : Exception
    {
        public UserException()               : base()        { }
        public UserException(string message) : base(message) { }
    }
}