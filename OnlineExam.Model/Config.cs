using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OnlineExam.Model
{
    public class Config
    {
        [SugarColumn(ColumnDataType ="varchar(200)")]
        public string Title { get; set; }//考试名称
        public int Num { get; set; }//每题分数
        public int Count { get; set; }//题目数量
        public int Total { get; set; }//考试总分
    }
}
