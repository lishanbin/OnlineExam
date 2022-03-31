using OnlineExam.WebApi.Utility._Npoi;
using System;

namespace OnlineExam.WebApi.ViewModel
{
    public class SubjectViewModel
    {
        [Title(Title ="试题编号")]
        public int Id { get; set; }
        [Title(Title ="试题题目")]
        public string Question { get; set; }//题目
        [Title(Title = "更多内容")]
        public string More { get; set; }//更多内容
        [Title(Title = "试题选项")]
        public string List { get; set; }//选项
        [Title(Title = "试题答案")]
        public string Answer { get; set; }//答案
        [Title(Title = "试题难度")]
        public int Grade { get; set; }//1初级、2中级、3高级
        [Title(Title = "试题状态")]
        public int State { get; set; }//1正常、2关闭
        public DateTime Adddate { get; set; } = DateTime.Now;
    }
}
