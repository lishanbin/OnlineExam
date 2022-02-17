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
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _iConfigService;

        public ConfigController(IConfigService iConfigService)
        {
            this._iConfigService = iConfigService;
        }

        /// <summary>
        /// 获取考试配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Configs")]
        public async Task<ApiResult> GetConfigs()
        {
            var configs = await _iConfigService.QueryAsync();
            if (configs.Count == 0) return ApiResultHelper.Error("没有配置信息");
            return ApiResultHelper.Success(configs);
        }

        /// <summary>
        /// 添加考试配置信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ApiResult> Create(string title,int num,int count,int total)
        {
            if (string.IsNullOrWhiteSpace("title") || num <= 0 || count <= 0 || total <= 0) return ApiResultHelper.Error("请检查考试配置信息");

            Config config = new Config
            {
                Title = title,
                Num = num,
                Count = count,
                Total = total
            };
            int newid=await _iConfigService.CreateAsync(config);
            if (newid <= 0) return ApiResultHelper.Error("考试配置信息添加失败");
            var newConfig=await _iConfigService.FindAsync(newid);
            return ApiResultHelper.Success(newConfig);
        }

        /// <summary>
        /// 删除考试配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<ApiResult> Delete(int id)
        {
            bool b = await _iConfigService.DeleteAsync(id);
            if (!b) return ApiResultHelper.Error("删除考试配置信息失败");
            return ApiResultHelper.Success(b);
        }

        /// <summary>
        /// 修改考试配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [HttpPut("Edit")]
        public async Task<ApiResult> Edit(int id, string title, int num, int count, int total)
        {
            if (string.IsNullOrWhiteSpace("title") || num <= 0 || count <= 0 || total <= 0) return ApiResultHelper.Error("请检查考试配置信息");

            var config = await _iConfigService.FindAsync(id);
            if (config == null) return ApiResultHelper.Error("要修改的配置信息不存在");
            config.Title = title;
            config.Num = num;
            config.Count = count;
            config.Total = total;
            bool b = await _iConfigService.EditAsync(config);
            if (!b) return ApiResultHelper.Error("考试配置信息修改失败");
            return ApiResultHelper.Success(config);
        }
    }
}
