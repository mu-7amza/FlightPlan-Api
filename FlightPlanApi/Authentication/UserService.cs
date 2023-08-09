using System;
using System.Threading.Tasks;

namespace FlightPlanApi.Authentication
{
    public class UserService : IUserService
    {
        public Task<User> Authenticate(string username, string password)
        {
            if (username != "admin" || password != "password")
            {
                return Task.FromResult<User>(null);
            }

            var user = new User
            {
                Username = username,
                Id = Guid.NewGuid().ToString("N")
            };

            return Task.FromResult(user);
        }
    }
}