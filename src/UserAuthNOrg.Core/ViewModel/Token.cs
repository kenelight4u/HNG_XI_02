namespace UserAuthNOrg.Core.ViewModel
{
    public record Token
    {
        public string AccessToken { get; set; }
    }

    public record SuccessfulUserCreation
    {
        public string AccessToken { get; set; }

        public ViewUser User { get; set; }
    }
}
