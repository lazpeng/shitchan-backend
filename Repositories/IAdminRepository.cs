using shitchan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shitchan.Repositories
{
    public interface IAdminRepository
    {
        Task<bool> ValidateCode(string code);
        Task<bool> UpdateCode(string oldCode, string newCode);
    }
}
