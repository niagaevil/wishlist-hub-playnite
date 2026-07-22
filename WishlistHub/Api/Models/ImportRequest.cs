namespace WishlistHub.Api.Models
{
    public class ImportRequest
    {
        public string Version => "v1";

        public string Token { get; set; }

        public string Data { get; set; }
    }
}
