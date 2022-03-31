using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Model.DTO
{
    public class SubjectDTO
    {
        public int Id { get; set; }
        public string Question { get; set; }//题目
        public string More { get; set; }//更多内容
        public string List { get; set; }//选项
        public string Answer { get; set; }//答案
        public int Grade { get; set; }//1初级、2中级、3高级
        public int State { get; set; }//1正常、2关闭
        public DateTime Adddate { get; set; }=DateTime.Now;
    }
}
