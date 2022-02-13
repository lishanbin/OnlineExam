using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OnlineExam.Model
{
    public class Subject:BaseId
    {
        [SugarColumn(ColumnDataType ="varchar(200)")]
        public string Question { get; set; }//题目
        [SugarColumn(ColumnDataType ="text")]
        public string More { get; set; }//更多内容
        [SugarColumn(ColumnDataType = "text")]
        public string List { get; set; }//选项
        [SugarColumn(ColumnDataType ="varchar(50)")]
        public string Answer { get; set; }//答案
        public int Grade { get; set; }//1初级、2中级、3高级
        public int State { get; set; }//1正常、2关闭
        public DateTime Adddate { get; set; }
    }
}
