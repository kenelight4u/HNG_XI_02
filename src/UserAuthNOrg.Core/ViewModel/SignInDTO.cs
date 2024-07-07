namespace UserAuthNOrg.Core.ViewModel
{
    public record SignInDTO
    {
        public string Email { get; set; }
        
        public string Password { get; set; }
    }

    public class EditDTO
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
       
        public string PhoneNumber { get; set; }
    }
}
