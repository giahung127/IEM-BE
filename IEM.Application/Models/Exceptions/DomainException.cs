namespace IEM.Application.Models.Exceptions
{
    public class DomainException : Exception
    {
        public readonly int ErrorCode;

        public DomainException(int errorCode) : base()
        {
            ErrorCode = errorCode;
        }

        public DomainException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
