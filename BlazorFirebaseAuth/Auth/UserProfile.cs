namespace BlazorFirebaseAuth.Auth
{
    public class UserProfile
    {
        public string Sub { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
    }
}
