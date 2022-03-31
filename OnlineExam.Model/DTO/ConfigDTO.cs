using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Model.DTO
{
    public class ConfigDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }//考试名称
        public int Num { get; set; }//每题分数
        public int Count { get; set; }//题目数量
        public int Total { get; set; }//考试总分
        public int State { get; set; } //配置状态
    }
}
