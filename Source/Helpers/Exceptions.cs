using System;

namespace Mogre.Builder
{
    class UserException : Exception
    {
        public UserException()               : base()        { }
        public UserException(string message) : base(message) { }
    }




    class ParseException : Exception
    {
        public ParseException() : base() { }
        //public ParseException(String message) : base(message) { }
    }

}