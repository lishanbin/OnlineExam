using OnlineExam.WebApi.ViewModel;
using System.Collections.Generic;

namespace OnlineExam.WebApi.Utility._Npoi
{
    public class ExcelDataResource<T> where T : class,new()
    {
        /// <summary>
        /// 保存到表的名称（sheet ?）
        /// </summary>
        public string SheetName { get; set; }
        /// <summary>
        /// 标题所在行
        /// </summary>
        public int TitleIndex { get; set; }
        /// <summary>
        /// 每一个Sheet的数据
        /// </summary>
        public List<T> SheetDataResource { get; set; }
    }

    public class UserInfo
    {
        [Title(Title ="用户Id")] //对应标题
        public int UserId { get; set; }
        [Title(Title ="用户名称")]
        public string UserName { get; set; }
        [Title(Title ="用户年龄")]
        public int Age { get; set; }
        [Title(Title ="用户类型")]
        public int UserType { get; set; }
        [Title(Title ="描述")]
        public string Description { get; set; }
    }


}
