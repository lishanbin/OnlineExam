using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using OnlineExam.IService;
using OnlineExam.JWT.Utility;
using OnlineExam.JWT.Utility._MD5;
using OnlineExam.JWT.Utility.ApiResult;
using OnlineExam.JWT.ViewModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.JWT.Controllers
{
    [Route("dlapi/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IStudentService _iStudentService;
        private readonly IDistributedCache _distributed;

        public LoginController(IStudentService iStudentService, IDistributedCache distributed)
        {
            this._iStudentService = iStudentService;
            this._distributed = distributed;
        }

        /// <summary>
        /// 学生登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ApiResult> Login(LoginViewModel loginViewModel)
        {
            if (string.IsNullOrWhiteSpace(loginViewModel.username) || string.IsNullOrWhiteSpace(loginViewModel.password)||string.IsNullOrWhiteSpace(loginViewModel.code))
            {
                return ApiResultHelper.Error("用户名或密码或验证码不能为空！");
            }

            if (!ValidateCaptchaCode(loginViewModel.code))
            {
                return ApiResultHelper.Error("验证码不正确！");
            }

            string pwd = MD5Helper.MD5Encrypt32(loginViewModel.password);
            var student=await _iStudentService.FindAsync(s=>s.Username==loginViewModel.username&&s.Password==pwd);
            if (student == null) return ApiResultHelper.Error("用户登录失败！");

            //登录成功
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,student.Name),
                new Claim("Id",student.Id.ToString()),
                new Claim("UserName",student.Username)
            };

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lishanbin-onlineexam"));//太短了不行
            //issuer代表颁发Token的web应用程序，audience是Token的受理者
            var token = new JwtSecurityToken(
                issuer: "http://172.16.36.13:6060",
                audience: "http://localhost:5000",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var jwtToken=new JwtSecurityTokenHandler().WriteToken(token);
            var tokenData = new { id = student.Id,role=student.Role, token = jwtToken };
            return ApiResultHelper.Success(tokenData);
        }

        private bool ValidateCaptchaCode(string userInputCaptcha)
        {
            //转换byte[]
            byte[] bytes = Encoding.UTF8.GetBytes(userInputCaptcha);
            //设置过期时间
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(100));
            //设置key
            string key = HttpContext.GetRemoteIPAddress().ToString();

            //使用Get读取Redis存储对象
            byte[] value = _distributed.Get(key);
            string valueStr = Encoding.UTF8.GetString(value);

            var isValid = userInputCaptcha.Equals(valueStr,System.StringComparison.OrdinalIgnoreCase);
            HttpContext.Response.Headers["captchacode"] = valueStr;

            return isValid;
        }
    }
}
