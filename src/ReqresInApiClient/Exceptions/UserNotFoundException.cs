using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqresInApiClient.Exceptions
{
    public class UserNotFoundException : ApiException
    {
        public UserNotFoundException(string message) : base(message) { }

        public UserNotFoundException(int userId)
            : base($"User with ID {userId} not found") { }
    }
}
