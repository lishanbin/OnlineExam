using System;
using System.IO;

namespace OnlineExam.JWT.Utility._Captcha
{
    public class CaptchaResult
    {
        public string CaptchaCode { get; set; }
        public MemoryStream CaptchaMemoryStream { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
