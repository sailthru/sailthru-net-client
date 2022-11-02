namespace Sailthru.Tests.Mock
{
    public class ApiException : SystemException
    {
        public ErrorResponse Response { get; private set; }

        public int StatusCode { get; private set; }

        public ApiException(int error, string errormsg, int statusCode)
        {
            Response = new ErrorResponse(error, errormsg);
            StatusCode = statusCode;
        }
    }
}
