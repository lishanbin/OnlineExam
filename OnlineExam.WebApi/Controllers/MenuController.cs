using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.IService;
using OnlineExam.Model;
using OnlineExam.WebApi.Utility.ApiResult;
using OnlineExam.WebApi.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineExam.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _iMenuService;

        public MenuController(IMenuService iMenuService)
        {
            this._iMenuService = iMenuService;
        }

        /// <summary>
        /// 获取所有菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Menus")]
        public async Task<ApiResult> GetMenus()
        {
            List<MenuViewModel> list = new List<MenuViewModel>();
            var menuList = await _iMenuService.QueryAsync(y => y.ParentId == 1);
            if (menuList.Count==0)
            {
                return ApiResultHelper.Error("还没有菜单信息");
            }
            foreach (var menu in menuList)
            {
                MenuViewModel menuViewModel = new MenuViewModel();
                menuViewModel.ParentId = menu.ParentId;
                menuViewModel.Name = menu.Name;
                menuViewModel.Id = menu.Id;
                menuViewModel.Url = menu.Url;
                menuViewModel.Component = menu.Component;
                menuViewModel.Path = menu.Path;
                menuViewModel.IconCls = menu.IconCls;
                menuViewModel.KeepAlive= menu.KeepAlive;
                menuViewModel.RequireAuth= menu.RequireAuth;
                menuViewModel.Enabled= menu.Enabled;

                List<MenuViewModel> subMenus = new List<MenuViewModel>();
                var subMenuList = await _iMenuService.QueryAsync(s => s.ParentId == menu.Id);
                if (subMenuList.Count == 0)
                {
                    menuViewModel.Children = null;
                }
                else
                {
                    foreach (var subMenu in subMenuList)
                    {
                        MenuViewModel subMenuViewModel = new MenuViewModel();
                        subMenuViewModel.ParentId = subMenu.ParentId;
                        subMenuViewModel.Name = subMenu.Name;
                        subMenuViewModel.Id = subMenu.Id;
                        subMenuViewModel.Url = subMenu.Url;
                        subMenuViewModel.Component = subMenu.Component;
                        subMenuViewModel.Path = subMenu.Path;
                        subMenuViewModel.IconCls = subMenu.IconCls;
                        subMenuViewModel.KeepAlive = subMenu.KeepAlive;
                        subMenuViewModel.RequireAuth = subMenu.RequireAuth;
                        subMenuViewModel.Enabled = subMenu.Enabled;
                        subMenus.Add(subMenuViewModel);
                    }
                    menuViewModel.Children=subMenus;
                }

                list.Add(menuViewModel);
            }
            return ApiResultHelper.Success(list);
        }

        /// <summary>
        /// 添加菜单信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <param name="component"></param>
        /// <param name="name"></param>
        /// <param name="iconCls"></param>
        /// <param name="keepAlive"></param>
        /// <param name="requireAuth"></param>
        /// <param name="parentId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ApiResult> Create(string url,string path,string component,string name,string iconCls,bool keepAlive,bool requireAuth,int parentId,bool enabled)
        {
            Menu menu = new Menu
            {
                ParentId = parentId,
                Name = name,
                IconCls = iconCls,
                KeepAlive = keepAlive,
                RequireAuth = requireAuth,
                Enabled = enabled,
                Url = url,
                Path = path,
                Component = component
            };
            int id = await _iMenuService.CreateAsync(menu);
            if (id <= 0) return ApiResultHelper.Error("菜单信息添加失败！");
            Menu newMenu=await _iMenuService.FindAsync(id);
            return ApiResultHelper.Success(newMenu);
        }

    }
}
