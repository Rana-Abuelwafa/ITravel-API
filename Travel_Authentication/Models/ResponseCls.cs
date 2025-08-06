namespace Travel_Authentication.Models
{
    public class ResponseCls
    {
        public bool isSuccessed { get; set; }
        public string? errors { get; set; }
        public string? message { get; set; }
        public User? user { get; set; }
    }
}
