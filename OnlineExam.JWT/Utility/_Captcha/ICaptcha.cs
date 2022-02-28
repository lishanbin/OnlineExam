using System.Threading.Tasks;

namespace OnlineExam.JWT.Utility._Captcha
{
    public interface ICaptcha
    {
        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        Task<string> GenerateRandomCaptchaAsync(int codeLength = 4);

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        /// <param name="captchaCode"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<CaptchaResult> GenerateCaptchaImageAsync(string captchaCode, int width=0, int height=30);
    }
}
