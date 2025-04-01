using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// IUserEntityRepository.cs
namespace ClinAgenda.src.Core.Interfaces
{
    public interface IUserEntityRepository
    {
        Task<bool> LinkUserToEntityAsync(int userId, string entityType, int entityId);
        Task<(string entityType, int entityId)?> GetUserEntityAsync(int userId);
        Task<int?> GetEntityUserIdAsync(string entityType, int entityId);
        Task<bool> UnlinkUserFromEntityAsync(int userId);
    }
}