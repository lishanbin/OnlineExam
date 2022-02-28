using System.Collections.Generic;

namespace OnlineExam.WebApi.ViewModel
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; } //url
        public string Path { get; set; } //path
        public string Component { get; set; } //组件
        public string Name { get; set; }  //菜单名
        public string IconCls { get; set; }  //图标
        public bool KeepAlive { get; set; } //是否保持激活
        public bool RequireAuth { get; set; } //是否要求权限
        public int ParentId { get; set; } //父id
        public bool Enabled { get; set; }  //是否启用
        public List<MenuViewModel> Children { get; set; } = null;

    }
}
