using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.Model.DTO;
using OnlineExam.WebApi.Utility.ApiResult;
using OnlineExam.WebApi.ViewModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        /// 获取考试配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Config")]
        public async Task<ApiResult> GetConfig()
        {
            var configs = await _iConfigService.QueryAsync(s=>s.State==1);
            if (configs.Count == 0) return ApiResultHelper.Error("没有配置信息");
            return ApiResultHelper.Success(configs[0]);
        }

        /// <summary>
        /// 获取所有考试配置信息的分页数据
        /// </summary>
        /// <param name="iMapper"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [HttpPost("GetPageConfigs")]
        [AllowAnonymous]
        public async Task<ApiResult> GetPageStudents([FromServices] IMapper iMapper, ConfPageViewModel confPageViewModel)
        {
            List<Config> confList = new List<Config>();

            Expressionable<Config> exp = Expressionable.Create<Config>();
            exp = exp.And(s => true);
                        
            Expression<Func<Config, bool>> expression = exp.ToExpression();

            confList = await _iConfigService.QueryAsync(expression, confPageViewModel.Page, confPageViewModel.Size, confPageViewModel.Total);
                       
            if (confList.Count == 0) return ApiResultHelper.Error("考试配置信息不存在！");

            try
            {
                var configs = iMapper.Map<List<ConfigDTO>>(confList);
                return ApiResultHelper.Success(configs, confPageViewModel.Total);
            }
            catch (System.Exception)
            {

                return ApiResultHelper.Error("AutoMapper映射错误");
            }
        }

        /// <summary>
        /// 添加考试配置信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="num"></param>
        /// <param name="amount"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ApiResult> Create(string title,int num,int amount,int total,int state)
        {
            if (string.IsNullOrWhiteSpace("title") || num <= 0 || amount <= 0 || total <= 0) return ApiResultHelper.Error("请检查考试配置信息");

            Config config = new Config
            {
                Title = title,
                Num = num,
                Count = amount,
                Total = total,
                State=state
            };
            int newid=await _iConfigService.CreateAsync(config);
            if (newid <= 0) return ApiResultHelper.Error("考试配置信息添加失败");
            var newConfig=await _iConfigService.FindAsync(newid);
            return ApiResultHelper.Success(newConfig);
        }

        /// <summary>
        ///  添加考试配置信息
        /// </summary>
        /// <param name="configViewModel"></param>
        /// <returns></returns>
        [HttpPost("CreateForm")]
        public async Task<ApiResult> Create(ConfigViewModel configViewModel)
        {
            if (string.IsNullOrWhiteSpace(configViewModel.Title) || configViewModel.Num <= 0 || configViewModel.Amount<=0 || configViewModel.Total <= 0) return ApiResultHelper.Error("请检查考试配置信息");

            Config config = new Config
            {
                Title = configViewModel.Title,
                Num = configViewModel.Num,
                Count = configViewModel.Amount,
                Total = configViewModel.Total,
                State =int.Parse(configViewModel.State)
            };
            int newid = await _iConfigService.CreateAsync(config);
            if (newid <= 0) return ApiResultHelper.Error("考试配置信息添加失败");
            var newConfig = await _iConfigService.FindAsync(newid);
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
        public async Task<ApiResult> Edit(int id, string title, int num, int amount, int total,int state)
        {
            if (string.IsNullOrWhiteSpace("title") || num <= 0 || amount <= 0 || total <= 0) return ApiResultHelper.Error("请检查考试配置信息");

            var config = await _iConfigService.FindAsync(id);
            if (config == null) return ApiResultHelper.Error("要修改的配置信息不存在");
            config.Title = title;
            config.Num = num;
            config.Count = amount;
            config.Total = total;
            config.State = state;
            bool b = await _iConfigService.EditAsync(config);
            if (!b) return ApiResultHelper.Error("考试配置信息修改失败");
            return ApiResultHelper.Success(config);
        }

        /// <summary>
        /// 修改考试配置信息
        /// </summary>
        /// <param name="configViewModel"></param>
        /// <returns></returns>
        [HttpPut("EditForm")]
        public async Task<ApiResult> Edit(ConfigViewModel configViewModel)
        {
            if (string.IsNullOrWhiteSpace(configViewModel.Title) || configViewModel.Num <= 0 || configViewModel.Amount <= 0 || configViewModel.Total <= 0) return ApiResultHelper.Error("请检查考试配置信息");

            var config = await _iConfigService.FindAsync(configViewModel.Id);
            if (config == null) return ApiResultHelper.Error("要修改的配置信息不存在");
            config.Title = configViewModel.Title;
            config.Num = configViewModel.Num;
            config.Count = configViewModel.Amount;
            config.Total = configViewModel.Total;
            config.State = int.Parse(configViewModel.State);
            bool b = await _iConfigService.EditAsync(config);
            if (!b) return ApiResultHelper.Error("考试配置信息修改失败");
            return ApiResultHelper.Success(config);
        }


    }
}
