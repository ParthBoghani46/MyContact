using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyContact.Models;

namespace MyContact.Repositories
{
    public interface IUserInterface
    {
        Task<int> Register(t_User userData);
        Task<t_User> Login(Login user);
        Task<t_User> GetUser(int userid);
    }
}