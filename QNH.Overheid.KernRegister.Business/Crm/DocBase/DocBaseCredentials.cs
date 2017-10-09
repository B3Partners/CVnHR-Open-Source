namespace QNH.Overheid.KernRegister.Business.Crm.DocBase
{
    public class DocBaseCredentials : IExportCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int AuthId {get;set;}
        public int ProcessId {get;set;}

        public DocBaseCredentials(string username, string password, int authId, int processId)
        {
            Username = username;
            Password = password;
            AuthId = authId;
            ProcessId = processId;
        }
    }
}