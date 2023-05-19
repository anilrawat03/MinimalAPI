namespace MinimalAPI.Common
{
    public class APIResponse
    {
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public APIResponse(string Message, dynamic Data)
        {
            this.Message = Message;
            this.Data = Data;
        }
        public APIResponse() { }
    }
}
