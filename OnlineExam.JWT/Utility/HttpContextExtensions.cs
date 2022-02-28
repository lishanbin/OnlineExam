using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;

namespace OnlineExam.JWT.Utility
{
    public static class HttpContextExtensions
    {
        public static IPAddress GetRemoteIPAddress(this HttpContext context,bool allowForwarded = true)
        {
            if (allowForwarded)
            {
                string header = (context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault());
                if (IPAddress.TryParse(header,out IPAddress ip))
                {
                    return ip;
                }
            }
            return context.Connection.RemoteIpAddress;
        }
    }
}
