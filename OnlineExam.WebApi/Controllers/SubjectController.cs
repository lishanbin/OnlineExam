using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.WebApi.Utility.ApiResult;
using OnlineExam.WebApi.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OnlineExam.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ISubjectService _iSubjectService;

        public SubjectController(IWebHostEnvironment env,ISubjectService iSubjectService)
        {
            this._env = env;
            this._iSubjectService = iSubjectService;
        }

        /// <summary>
        /// 获取所有题库信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Subjects")]
        public async Task<ApiResult> GetSubjects()
        {
            var subjects=await _iSubjectService.QueryAsync();
            if (subjects.Count == 0) return ApiResultHelper.Error("没有更多的题库");
            return ApiResultHelper.Success(subjects);
        }

        /// <summary>
        /// 添加题库信息
        /// </summary>
        /// <param name="question"></param>
        /// <param name="more"></param>
        /// <param name="list"></param>
        /// <param name="answer"></param>
        /// <param name="grade"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ApiResult> Create(AddSubjectViewModel addSubjectViewModel)
        {
            if (string.IsNullOrWhiteSpace(addSubjectViewModel.Question) ||string.IsNullOrWhiteSpace(addSubjectViewModel.List)||string.IsNullOrWhiteSpace(addSubjectViewModel.Answer))
            {
                return ApiResultHelper.Error("题库信息不完整");
            }

            Subject subject = new Subject
            {
                Question = addSubjectViewModel.Question,
                More = string.IsNullOrWhiteSpace(addSubjectViewModel.More) ?"": addSubjectViewModel.More,
                List = addSubjectViewModel.List,
                Answer = addSubjectViewModel.Answer,
                Grade = addSubjectViewModel.Grade,
                State = addSubjectViewModel.State
            };

            int id=await _iSubjectService.CreateAsync(subject);
            if (id <= 0) return ApiResultHelper.Error("题库信息添加失败");
            var newsubject=await _iSubjectService.FindAsync(id);
            return ApiResultHelper.Success(newsubject);
        }

        /// <summary>
        /// 删除题库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<ApiResult> Delete(int id)
        {
            bool b = await _iSubjectService.DeleteAsync(id);
            if (!b) return ApiResultHelper.Error("删除失败");
            return ApiResultHelper.Success(b);
        }

        /// <summary>
        /// 修改题库信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="question"></param>
        /// <param name="more"></param>
        /// <param name="list"></param>
        /// <param name="answer"></param>
        /// <param name="grade"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut("Edit")]
        public async Task<ApiResult> Edit(int id, string question, string more, string list, string answer, int grade, int state)
        {
            if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(list) || string.IsNullOrWhiteSpace(answer))
            {
                return ApiResultHelper.Error("题库信息不完整");
            }
            var subject = await _iSubjectService.FindAsync(id);
            if (subject == null) return ApiResultHelper.Error("要修改的题库信息不存在");
            subject.Question = question;
            subject.More = more;
            subject.List = list;
            subject.Answer = answer;
            subject.Grade = grade;
            subject.State = state;

            bool b = await _iSubjectService.EditAsync(subject);
            if (!b) return ApiResultHelper.Error("题库信息修改失败");
            return ApiResultHelper.Success(subject);
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpPost("UploadPhoto")]
        public  ApiResult UploadPhoto()
        {
            //获取上传文件后缀
            string ext = Path.GetExtension(Request.Form.Files[0].FileName).ToLower();
            Console.WriteLine("file:"+Request.Form.Files[0].FileName);
            if (!ext.Contains(".jpg") && !ext.Contains(".png"))
            {
                return ApiResultHelper.Error("图片格式有误，只支持上传.jpg、.png文件");
            }
            //限制单次只能上传5M
            float fileSize = Request.Form.Files[0].Length / 1024 / 1024;
            if (fileSize > 5)
            {
                return ApiResultHelper.Error("图片文件大小超过限制（上传不大于5M）");
            }

            //图片路径
            string filePath = _env.ContentRootPath + "/Images/";
            //string filePath = _env.WebRootPath + "/Images/";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fullName = string.Empty;
            string fileName = Guid.NewGuid().ToString();

            fullName =Path.Combine(filePath,fileName + ext);

            //用using语句释放文件资源
            using (FileStream stream = new FileStream(fullName, FileMode.Create, FileAccess.Write))
            {
                Request.Form.Files[0].CopyTo(stream);
                stream.Flush();
            }

            return ApiResultHelper.Success("http://"+Request.HttpContext.Connection.LocalIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.LocalPort + "/Images/" + fileName + ext);

        }



    }
}
