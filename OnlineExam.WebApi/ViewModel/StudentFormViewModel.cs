using OnlineExam.WebApi.Utility._Npoi;
using System;

namespace OnlineExam.WebApi.ViewModel
{
    public class StudentFormViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  
        public string Name { get; set; }//姓名
        public int Num { get; set; }//分数
        public int State { get; set; }//1未考、2已考、3重考、4禁考
        public DateTime Adddate { get; set; } = DateTime.Now;
        public int Role { get; set; } = 0;  //0 学生   1 管理员
    }
}
