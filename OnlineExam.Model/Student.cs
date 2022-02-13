using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OnlineExam.Model
{
    public class Student:BaseId
    {
        [SugarColumn(ColumnDataType ="varchar(50)")]
        public string Username { get; set; }
        [SugarColumn(ColumnDataType ="varchar(50)")]
        public string Password { get; set; }
        [SugarColumn(ColumnDataType = "varchar(50)")]
        public string Name { get; set; }//姓名
        public int Num { get; set; }//分数
        public int State { get; set; }//1未考、2已考、3重考、4禁考
        public DateTime Adddate { get; set; }
    }
}
