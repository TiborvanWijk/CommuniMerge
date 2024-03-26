using CommuniMerge.Library.Models;
using CommuniMerge.Library.ResultObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services.Interfaces
{
    public interface IAccountService
    {
        Task<RegistrationResult> Register(RegisterModel registerModel);
        Task<bool> Login(string username, string password);
    }
}
