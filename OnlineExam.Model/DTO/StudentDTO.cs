using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Model.DTO
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }//姓名
        public int Num { get; set; }//分数
        public int State { get; set; }//1未考、2已考、3重考、4禁考
        public DateTime Adddate { get; set; } = DateTime.Now;
    }
}
