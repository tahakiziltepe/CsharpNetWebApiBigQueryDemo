namespace CsharpNetWebApiBigQueryDemo.API.Models
{
    public class BaseResponseModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<PageViews> Data { get; set; }
    }
}
