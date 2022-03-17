using SqlSugar;

namespace OnlineExam.WebApi.ViewModel
{
    public class StuPageViewModel
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public RefAsync<int> Total { get; set; }
        public string Name { get; set; }=string.Empty;
        public string State { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
