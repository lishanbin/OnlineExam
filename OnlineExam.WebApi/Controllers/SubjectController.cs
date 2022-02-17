using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.WebApi.Utility.ApiResult;
using System.Threading.Tasks;

namespace OnlineExam.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _iSubjectService;

        public SubjectController(ISubjectService iSubjectService)
        {
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
        public async Task<ApiResult> Create(string question,string more,string list,string answer,int grade,int state)
        {
            if (string.IsNullOrWhiteSpace(question)||string.IsNullOrWhiteSpace(list)||string.IsNullOrWhiteSpace(answer))
            {
                return ApiResultHelper.Error("题库信息不完整");
            }

            Subject subject = new Subject
            {
                Question = question,
                More = more,
                List = list,
                Answer = answer,
                Grade = grade,
                State = state
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

    }
}
