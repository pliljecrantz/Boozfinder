namespace Boozfinder.Models.Responses
{
    public class AuthenticationResponse
    {
        public bool Authenticated { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string Expires { get; set; }
    }
}
