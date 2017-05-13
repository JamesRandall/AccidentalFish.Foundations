using System;

namespace AccidentalFish.Foundations.Resources.Azure.TableStorage
{
    public class UnableToAcquireLeaseException : Exception
    {
        public UnableToAcquireLeaseException()
        {
        }

        public UnableToAcquireLeaseException(string message) : base(message)
        {
        }

        public UnableToAcquireLeaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
