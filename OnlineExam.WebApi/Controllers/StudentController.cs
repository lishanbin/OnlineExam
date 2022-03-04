using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.Model.DTO;
using OnlineExam.WebApi.Utility._MD5;
using OnlineExam.WebApi.Utility._Npoi;
using OnlineExam.WebApi.Utility.ApiResult;
using OnlineExam.WebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace OnlineExam.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _iStudentService;

        public StudentController(IStudentService iStudentService)
        {
            this._iStudentService = iStudentService;
        }

        /// <summary>
        /// 获取所有的学生
        /// </summary>
        /// <returns></returns>
        [HttpGet("Students")]
        public async Task<ApiResult> GetStudents([FromServices]IMapper iMapper)
        {
           var stuList=await  _iStudentService.QueryAsync();
            if (stuList.Count == 0) return ApiResultHelper.Error("没有更多的学生");

            try
            {
                var students=iMapper.Map<List<StudentDTO>>(stuList);
                return ApiResultHelper.Success(students);
            }
            catch (System.Exception)
            {

                return ApiResultHelper.Error("AutoMapper映射错误");
            }
            
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
            var stuList = await _iStudentService.QueryAsync();

            var students = iMapper.Map<List<StudentViewModel>>(stuList);

            //设置ContentType
            HttpContext.Response.ContentType = "application/vnd.ms-excel;charset=utf-8";
            //生成文件名
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            //设置Excel文件名
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            HttpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={fileName}.xls");
            
            //获取导出Excel需要的数据源
            List<ExcelDataResource> dataResources = new List<ExcelDataResource>
            {
                new ExcelDataResource()
                {
                    SheetName="学生信息表",
                    TitleIndex=1,
                    SheetDataResource=students
                }
            };

            byte[] bt=ExcelOperationHelper.ToExcelByteArray(dataResources);

            //await HttpContext.Response.BodyWriter.WriteAsync(bt);

            return File(bt, "application/vnd.ms-excel;charset=utf-8",$"{fileName}.xls");
        }

        /// <summary>
        /// 文件导入
        /// consumes["multipart/form-data"]
        /// </summary>
        /// <returns></returns>
        [HttpPost("ImportExcelOnFormSubmit")]
        [AllowAnonymous]
        public async Task<ApiResult> ImportExcelOnFormSubmit(IFormFile file)
        {
            string ext=Path.GetExtension(file.FileName).ToLower();
            Console.WriteLine(ext);
            //获取上传的Excel文件
            string msg = "";
            List<Student> students = ExcelOperationHelper.ExcelStreamToList(file, out msg);
            if (students == null) return ApiResultHelper.Error(msg);

            foreach (var student in students)
            {
                int newid = await _iStudentService.CreateAsync(student);
                if (newid <= 0) return ApiResultHelper.Error("添加失败");
            }
            List<Student> stuList = await _iStudentService.QueryAsync();
            return ApiResultHelper.Success(stuList);
        }

        /// <summary>
        /// 获取学生信息
        /// </summary>
        /// <param name="iMapper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Student")]
        public async Task<ApiResult> Student([FromServices]IMapper iMapper,int id)
        {
            var student=await _iStudentService.FindAsync(id);
            if (student == null) return ApiResultHelper.Error("学生信息不存在");
            var studentDTO=iMapper.Map<StudentDTO>(student);
            return ApiResultHelper.Success(studentDTO);
        }

        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="num"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ApiResult> Create(string username,string password,string name,int num,int state)
        {
            //数据验证
            Student student = new Student
            {
                Username = username,
                Password = MD5Helper.MD5Encrypt32(password),
                Name = name,
                State = state,
                Num = num
            };
            int newId=await _iStudentService.CreateAsync(student);
            if (newId<=0) return ApiResultHelper.Error("添加失败");
            var newStu=await _iStudentService.FindAsync(newId);
            return ApiResultHelper.Success(newStu);
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<ApiResult> Delete(int id)
        {
            bool b = await _iStudentService.DeleteAsync(id);
            if (!b) return ApiResultHelper.Error("删除失败");
            return ApiResultHelper.Success(b);
        }

        /// <summary>
        /// 编辑学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="num"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut("Edit")]
        public async Task<ApiResult> Edit(int id,string username, string password, string name, int num, int state)
        {
            var student = await _iStudentService.FindAsync(id);
            if (student == null) return ApiResultHelper.Error("没有找到该学生");
            student.Username = username;
            student.Password = MD5Helper.MD5Encrypt32(password);
            student.Name = name;
            student.State = state;
            student.Num = num;
            student.Adddate = System.DateTime.Now;
            bool b=await _iStudentService.EditAsync(student);
            if (!b) return ApiResultHelper.Error("修改失败");
            return ApiResultHelper.Success(student);
        }

        [HttpGet("ExportExcel")]
        [AllowAnonymous]
        public ActionResult ExportExcelAsync()
        {
            try
            {
                IWorkbook workBook=ExcelOperationHelper.CreateExcelWorkbook();
                using (FileStream file=new FileStream("C:\\Users\\lishanbin\\Desktop\\Exam\\text.xls",FileMode.Create))
                {
                    workBook.Write(file);
                    return Ok("下载成功！");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


    }
}
