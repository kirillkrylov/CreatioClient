namespace CreatioClient.Core.Models.Domain
{
    internal interface ICredentials
    {
        string UserName { get; }
        string UserPassword { get;}
    }
    internal class Credentials : ICredentials
    {
        public Credentials(string userName, string userPassword)
        {
            UserName = userName;
            UserPassword = userPassword;
        }

        public string UserName { get;  }
        public string UserPassword { get; }
    }
}
