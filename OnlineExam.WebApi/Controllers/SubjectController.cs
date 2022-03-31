using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.Model.DTO;
using OnlineExam.WebApi.Utility._Npoi;
using OnlineExam.WebApi.Utility.ApiResult;
using OnlineExam.WebApi.ViewModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
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
        /// 获取前n条数据
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("GetTopSubjects")]
        public async Task<ApiResult> GetTopSubjects(int top)
        {
            var subjects = await _iSubjectService.QueryTopAsync(top);
            if (subjects.Count == 0) return ApiResultHelper.Error("没有更多的题库");
            return ApiResultHelper.Success(subjects);
        }

        /// <summary>
        /// 获取随机前n条数据
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("GetRndTopSubjects")]
        public async Task<ApiResult> GetRndTopSubjects(int top)
        {
            var subjects = await _iSubjectService.QueryRndTopAsync(top);
            if (subjects.Count == 0) return ApiResultHelper.Error("没有更多的题库");
            return ApiResultHelper.Success(subjects);
        }

        /// <summary>
        /// 获取题库的分页数据
        /// </summary>
        /// <param name="iMapper"></param>
        /// <param name="subPageViewModel"></param>
        /// <returns></returns>
        [HttpPost("GetPageSubjects")]
        public async Task<ApiResult> GetPageSubjects([FromServices] IMapper iMapper, SubPageViewModel subPageViewModel)
        {
            List<Subject> subList = new List<Subject>();

            Expressionable<Subject> exp = Expressionable.Create<Subject>();
            exp = exp.And(s => true);
            if (!string.IsNullOrWhiteSpace(subPageViewModel.Question))
            {
                exp = exp.And(s => s.Question.Contains(subPageViewModel.Question));
            }

            if (!string.IsNullOrWhiteSpace(subPageViewModel.Grade))
            {
                int grade = int.Parse(subPageViewModel.Grade);
                exp = exp.And(s => s.Grade == grade);
            }

            if (!string.IsNullOrWhiteSpace(subPageViewModel.State))
            {
                int state = int.Parse(subPageViewModel.State);
                exp = exp.And(s => s.State == state);
            }

            Expression<Func<Subject, bool>> expression = exp.ToExpression();

            subList = await _iSubjectService.QueryAsync(expression, subPageViewModel.Page, subPageViewModel.Size, subPageViewModel.Total);

            if (subList.Count == 0) return ApiResultHelper.Error("题库信息不存在！");

            try
            {
                var subjects = iMapper.Map<List<SubjectDTO>>(subList);
                return ApiResultHelper.Success(subjects, subPageViewModel.Total);
            }
            catch (Exception)
            {
                return ApiResultHelper.Error("AutoMapper映射错误");
            }

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
        /// 修改题库信息
        /// </summary>
        /// <param name="addSubjectViewModel"></param>
        /// <returns></returns>
        [HttpPut("EditForm")]
        public async Task<ApiResult> Edit(AddSubjectViewModel addSubjectViewModel)
        {
            if (string.IsNullOrWhiteSpace(addSubjectViewModel.Question) || string.IsNullOrWhiteSpace(addSubjectViewModel.List) || string.IsNullOrWhiteSpace(addSubjectViewModel.Answer))
            {
                return ApiResultHelper.Error("题库信息不完整");
            }
            var subject = await _iSubjectService.FindAsync(addSubjectViewModel.Id);
            if (subject == null) return ApiResultHelper.Error("要修改的题库信息不存在");
            subject.Question = addSubjectViewModel.Question;
            subject.More = addSubjectViewModel.More;
            subject.List = addSubjectViewModel.List;
            subject.Answer = addSubjectViewModel.Answer;
            subject.Grade = addSubjectViewModel.Grade;
            subject.State = addSubjectViewModel.State;

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


        /// <summary>
        /// 文件导出
        /// </summary>
        /// <param name="iMapper"></param>
        /// <returns></returns>
        [HttpGet("ImportExcelFileOnWriteResponse")]
        //[AllowAnonymous]
        public async Task<IActionResult> ImportExcelFileOnWriteResponse([FromServices] IMapper iMapper)
        {
            var subList = await _iSubjectService.QueryAsync();

            var subjects = iMapper.Map<List<SubjectViewModel>>(subList);

            //设置ContentType
            HttpContext.Response.ContentType = "application/vnd.ms-excel;charset=utf-8";
            //生成文件名
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            //设置Excel文件名
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            HttpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={fileName}.xls");

            //获取导出Excel需要的数据源
            List<ExcelDataResource<SubjectViewModel>> dataResources = new List<ExcelDataResource<SubjectViewModel>>
            {
                new ExcelDataResource<SubjectViewModel>()
                {
                    SheetName="题库信息表",
                    TitleIndex=1,
                    SheetDataResource=subjects
                }
            };

            byte[] bt = ExcelOperationHelper<SubjectViewModel>.ToExcelByteArray(dataResources);

            //await HttpContext.Response.BodyWriter.WriteAsync(bt);

            return File(bt, "application/vnd.ms-excel;charset=utf-8", $"{fileName}.xls");
        }

        /// <summary>
        /// 文件导入
        /// consumes["multipart/form-data"]
        /// </summary>
        /// <returns></returns>
        [HttpPost("ImportExcelOnFormSubmit")]
        //[AllowAnonymous]
        public async Task<ApiResult> ImportExcelOnFormSubmit(IFormFile file)
        {
            string ext = Path.GetExtension(file.FileName).ToLower();
            Console.WriteLine(ext);
            //获取上传的Excel文件
            string msg = "";
            List<Subject> subjects = new List<Subject>();
            List<DataTable> dtList = ExcelOperationHelper<Subject>.ExcelStreamToList(file, out msg);

            DataTable dtSubject = dtList[0];
            for (int i = 0; i < dtSubject.Rows.Count; i++)
            {
                //    //获取指定行数据
                DataRow row = dtSubject.Rows[i];
                Subject subject = new Subject();
                subject.Question = row[1].ToString();
                subject.More = row[2].ToString();
                subject.List = row[3].ToString();
                subject.Answer = row[4].ToString();
                subject.Grade = int.Parse(row[5].ToString());
                subject.State=int.Parse(row[6].ToString());
                subject.Adddate = DateTime.Now;
                subjects.Add(subject);
            }

            if (subjects == null) return ApiResultHelper.Error(msg);

            foreach (var subject in subjects)
            {
                int newid = await _iSubjectService.CreateAsync(subject);
                if (newid <= 0) return ApiResultHelper.Error("添加失败");
            }
            List<Subject> subList = await _iSubjectService.QueryAsync();
            return ApiResultHelper.Success(subList);
        }



    }
}
