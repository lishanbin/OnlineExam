using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OnlineExam.JWT.Utility;
using OnlineExam.JWT.Utility._Captcha;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.JWT.Controllers
{
    [Route("dlapi/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly IDistributedCache _distributed;

        public CaptchaController(IDistributedCache distributed)
        {
            this._distributed = distributed;
        }

        [HttpGet]
        public async Task<FileContentResult> CaptChaAsync([FromServices] ICaptcha _captcha)
        {
            var code = await _captcha.GenerateRandomCaptchaAsync();

            var result = await _captcha.GenerateCaptchaImageAsync(code);

            HttpContext.Response.Headers["CaptchaCode"] = result.CaptchaCode;
            HttpContext.Response.Headers["Host"] = HttpContext.Request.Host.Host;
            HttpContext.Response.Headers["Port"] = HttpContext.Request.Host.Port.ToString();
            HttpContext.Response.Headers["RemoteAddress"] =Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            HttpContext.Response.Headers["RemotePort"] =Request.HttpContext.Connection.RemotePort.ToString();
            HttpContext.Response.Headers["ip"] = HttpContext.GetRemoteIPAddress().ToString();

            //转换byte[]
            byte[] bytes = Encoding.UTF8.GetBytes(result.CaptchaCode);
            //设置过期时间
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(100));
            //设置key
            string key = HttpContext.GetRemoteIPAddress().ToString();

            //使用Set存储redis存储对象
            _distributed.Set(key, bytes, options);

            //使用Get读取Redis存储对象
            byte[] value = _distributed.Get(key);
            string valueStr = Encoding.UTF8.GetString(value);
            HttpContext.Response.Headers["code"] = valueStr;

            return File(result.CaptchaMemoryStream.ToArray(), "image/png");
        }

    }
}
