using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> Register(string username, string email, string password);
        Task<bool> Login(string username, string password);
    }
}
