﻿using System;
using System.Runtime.Serialization;

namespace Penguin.Robinhood.Exceptions
{
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException()
        {
        }

        public SessionExpiredException(string message) : base(message)
        {
        }

        public SessionExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SessionExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}