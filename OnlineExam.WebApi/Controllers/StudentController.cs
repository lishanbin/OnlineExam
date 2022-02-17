﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.WebApi.Utility.ApiResult;
using System.Threading.Tasks;

namespace OnlineExam.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ApiResult> GetStudents()
        {
           var stuList=await  _iStudentService.QueryAsync();
            if (stuList.Count == 0) return ApiResultHelper.Error("没有更多的学生");

            return ApiResultHelper.Success(stuList);
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
                Password = password,
                Name = name,
                State = state,
                Num = num
            };
            int newId=await _iStudentService.CreateAsync(student);
            if (newId<=0) return ApiResultHelper.Error("添加失败");
            var newStu=await _iStudentService.FindAsync(newId);
            return ApiResultHelper.Success(newStu);
        }

    }
}