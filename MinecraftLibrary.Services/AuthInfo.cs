namespace ProtoLib.Services
{
    public struct AuthInfo
    {
        public AccountType AccountType { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public AuthInfo(AccountType accountType, string login, string password)
        {
            AccountType = accountType;
            Login = login;
            Password = password;
        }
    }
}