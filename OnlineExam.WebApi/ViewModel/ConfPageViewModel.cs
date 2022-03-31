using SqlSugar;

namespace OnlineExam.WebApi.ViewModel
{
    public class ConfPageViewModel
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public RefAsync<int> Total { get; set; }
    }
}
