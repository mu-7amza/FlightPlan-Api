using System.Threading.Tasks;

namespace FlightPlanApi.Authentication
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }
}