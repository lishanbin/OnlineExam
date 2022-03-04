using OnlineExam.WebApi.Utility._Npoi;
using System;

namespace OnlineExam.WebApi.ViewModel
{
    public class StudentViewModel
    {
        [Title(Title ="学生编号")]
        public int Id { get; set; }
        [Title(Title ="用户名")]
        public string Username { get; set; }
        [Title(Title ="学生姓名")]
        public string Name { get; set; }//姓名
        [Title(Title ="分数")]
        public int Num { get; set; }//分数
        [Title(Title ="考试状态")]
        public int State { get; set; }//1未考、2已考、3重考、4禁考
        public DateTime Adddate { get; set; } = DateTime.Now;
        [Title(Title ="角色")]
        public int Role { get; set; } = 0;  //0 学生   1 管理员
    }
}
