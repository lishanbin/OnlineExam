namespace OnlineExam.WebApi.ViewModel
{
    public class ConfigViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }//考试名称
        public int Num { get; set; }//每题分数
        public int Amount { get; set; }//题目数量
        public int Total { get; set; }//考试总分
        public string State { get; set; } //配置状态
    }
}
