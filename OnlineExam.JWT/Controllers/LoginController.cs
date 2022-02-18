using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineExam.IService;
using OnlineExam.JWT.Utility._MD5;
using OnlineExam.JWT.Utility.ApiResult;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IStudentService _iStudentService;

        public LoginController(IStudentService iStudentService)
        {
            this._iStudentService = iStudentService;
        }

        /// <summary>
        /// 学生登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ApiResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return ApiResultHelper.Error("用户名或密码不能为空！");
            }

            string pwd = MD5Helper.MD5Encrypt32(password);
            var student=await _iStudentService.FindAsync(s=>s.Username==username&&s.Password==pwd);
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
                issuer: "http://localhost:6060",
                audience: "http://localhost:5000",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var jwtToken=new JwtSecurityTokenHandler().WriteToken(token);
            return ApiResultHelper.Success(jwtToken);
        }
    }
}
