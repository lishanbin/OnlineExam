using SqlSugar;

namespace OnlineExam.WebApi.ViewModel
{
    public class SubPageViewModel
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public RefAsync<int> Total { get; set; }
        public string Question { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
