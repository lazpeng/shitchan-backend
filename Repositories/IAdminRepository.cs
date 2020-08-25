using shitchan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shitchan.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin> Register(string Invite, Admin Target);
        Task<string> Login(Admin Target);
        Task<Admin> ValidateToken(string Token);
        Task<Admin> Get(long Id);
    }
}
