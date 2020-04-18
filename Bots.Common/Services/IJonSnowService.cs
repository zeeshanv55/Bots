namespace Bots.Common.Services
{
    using System.Threading.Tasks;

    public interface IJonSnowService
    {
        Task<string> GetResponse(string prompt);
    }
}
