namespace QNH.Overheid.KernRegister.Business.Crm
{
    public interface IExportCredentials
    {
        string Username { get; set; }
        string Password { get; set; }
        int AuthId { get; set; }
        int ProcessId { get; set; }
    }
}