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
        /// 获取所有学生的分页数据
        /// </summary>
        /// <param name="iMapper"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [HttpPost("GetPageStudents")]
        [AllowAnonymous]
        public async Task<ApiResult> GetPageStudents([FromServices] IMapper iMapper, StuPageViewModel stuPageViewModel)
        {
            List<Student> stuList = new List<Student>();

            Expressionable<Student> exp = Expressionable.Create<Student>();
            exp=exp.And(s => true);
            if (!string.IsNullOrWhiteSpace(stuPageViewModel.Name))
            {
                exp = exp.And(s => s.Name.Contains(stuPageViewModel.Name));
            }

            if (!string.IsNullOrWhiteSpace(stuPageViewModel.UserName))
            {
                exp = exp.And(s => s.Username.Contains(stuPageViewModel.UserName));
            }

            if (!string.IsNullOrWhiteSpace(stuPageViewModel.State))
            {
                int state = int.Parse(stuPageViewModel.State);
                exp = exp.And(s => s.State == state);
            }

            if (!string.IsNullOrWhiteSpace(stuPageViewModel.Role))
            {
                int role = int.Parse(stuPageViewModel.Role);
                exp = exp.And(s => s.Role == role);
            }

            Expression<Func<Student,bool>> expression = exp.ToExpression();

            stuList = await _iStudentService.QueryAsync(expression, stuPageViewModel.Page, stuPageViewModel.Size, stuPageViewModel.Total);

            //if (string.IsNullOrWhiteSpace(stuPageViewModel.Name))
            //{
            //    stuList = await _iStudentService.QueryAsync(stuPageViewModel.Page, stuPageViewModel.Size, stuPageViewModel.Total);
            //}
            //else
            //{
            //    stuList = await _iStudentService.QueryAsync(s=>s.Name.Contains(stuPageViewModel.Name),stuPageViewModel.Page, stuPageViewModel.Size, stuPageViewModel.Total);
            //}

            
            if (stuList.Count == 0) return ApiResultHelper.Error("学生信息不存在！");

            try
            {
                var students = iMapper.Map<List<StudentDTO>>(stuList);
                return ApiResultHelper.Success(students, stuPageViewModel.Total);
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
            List<ExcelDataResource<StudentViewModel>> dataResources = new List<ExcelDataResource<StudentViewModel>>
            {
                new ExcelDataResource<StudentViewModel>()
                {
                    SheetName="学生信息表",
                    TitleIndex=1,
                    SheetDataResource=students
                }
            };

            byte[] bt=ExcelOperationHelper<StudentViewModel>.ToExcelByteArray(dataResources);

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

            List<Student> students = new List<Student>();
            List<DataTable> dtList = ExcelOperationHelper<Student>.ExcelStreamToList(file, out msg);

            DataTable dtStudent = dtList[0];
            for (int i = 0; i < dtStudent.Rows.Count; i++)
            {
                //    //获取指定行数据
                DataRow row = dtStudent.Rows[i];
                Student student = new Student();
                student.Username = row[1].ToString();
                student.Password = MD5Helper.MD5Encrypt32("123456");
                student.Name = row[2].ToString();
                student.Num = int.Parse(row[3].ToString());
                student.State = int.Parse(row[4].ToString());
                student.Role = int.Parse(row[5].ToString());
                student.Adddate = DateTime.Now;
                students.Add(student);
            }

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
        /// 添加学生(JSON)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="num"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost("CreateForm")]
        public async Task<ApiResult> CreateForm(StudentFormViewModel studentFormViewModel)
        {
            //数据验证
            Student student = new Student
            {
                Username = studentFormViewModel.Username,
                Password = MD5Helper.MD5Encrypt32(studentFormViewModel.Password),
                Name = studentFormViewModel.Name,
                State = studentFormViewModel.State,
                Num = studentFormViewModel.Num,
                Role=studentFormViewModel.Role
            };
            int newId = await _iStudentService.CreateAsync(student);
            if (newId <= 0) return ApiResultHelper.Error("添加失败");
            var newStu = await _iStudentService.FindAsync(newId);
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
        [HttpPut("EditForm")]
        public async Task<ApiResult> EditForm(StudentFormViewModel studentFormViewModel)
        {
            Console.WriteLine(studentFormViewModel.Role);
            var student = await _iStudentService.FindAsync(studentFormViewModel.Id);
            if (student == null) return ApiResultHelper.Error("没有找到该学生");
            student.Username = studentFormViewModel.Username;
            student.Password =String.IsNullOrWhiteSpace(studentFormViewModel.Password)? student.Password: MD5Helper.MD5Encrypt32(studentFormViewModel.Password);
            student.Name = studentFormViewModel.Name;
            student.State = studentFormViewModel.State;
            student.Num = studentFormViewModel.Num;
            student.Role = studentFormViewModel.Role;
            student.Adddate = System.DateTime.Now;
            bool b = await _iStudentService.EditAsync(student);
            if (!b) return ApiResultHelper.Error("修改失败");
            return ApiResultHelper.Success(student);
        }

        [HttpGet("ExportExcel")]
        [AllowAnonymous]
        public ActionResult ExportExcelAsync()
        {
            try
            {
                IWorkbook workBook=ExcelOperationHelper<Student>.CreateExcelWorkbook();
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
