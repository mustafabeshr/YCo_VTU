using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.api.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            resultCode = statusCode;
            resultDesc = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int resultCode { get; set; }
        public string resultDesc { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "You have made a bad request, ",
                401 => "You are not an authorized",
                404 => "Resource was not found",
                500 => "Server error was happened",
                _ => null
            };
        }
    }
}
