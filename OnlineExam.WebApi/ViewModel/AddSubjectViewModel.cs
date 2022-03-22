using System;

namespace OnlineExam.WebApi.ViewModel
{
    public class AddSubjectViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }//题目
        public string More { get; set; }//更多内容
        public string List { get; set; }//选项
        public string Answer { get; set; }//答案
        public int Grade { get; set; }//1初级、2中级、3高级
        public int State { get; set; }//1正常、2关闭
        public DateTime Adddate { get; set; }
    }
}
