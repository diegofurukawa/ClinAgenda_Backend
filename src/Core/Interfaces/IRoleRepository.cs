using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Core.Entities;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByIdAsync(int id);
        Task<Role?> GetRoleByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<int> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id);
    }    
}