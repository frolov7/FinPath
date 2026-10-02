using System;
using System.Collections.Generic;
using System.Text;

namespace FinPath.Application.Common.Exceptions
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
