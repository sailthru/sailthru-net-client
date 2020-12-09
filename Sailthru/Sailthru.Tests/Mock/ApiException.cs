using System;
namespace Sailthru.Tests.Mock
{
    public class ApiException : SystemException
    {
        public ErrorResponse Response { get; private set; }

        public ApiException(int error, string errormsg)
        {
            Response = new ErrorResponse(error, errormsg);
        }
    }
}
