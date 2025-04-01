// EntityAuthorizationAttribute.cs
using System.Security.Claims;
using ClinAgenda.src.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClinAgenda.src.WebAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class EntityAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _entityType;
        private readonly string _entityIdParameterName;

        public EntityAuthorizationAttribute(string entityType, string entityIdParameterName = "id")
        {
            _entityType = entityType;
            _entityIdParameterName = entityIdParameterName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Verificar se o usuário está autenticado
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Verificar se o usuário tem a role "Admin" (eles podem acessar qualquer recurso)
            if (context.HttpContext.User.IsInRole("Admin"))
                return;

            // Obter o ID do usuário atual
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Obter o parâmetro de ID da entidade
            if (!context.RouteData.Values.TryGetValue(_entityIdParameterName, out var entityIdObj) || 
                !int.TryParse(entityIdObj?.ToString(), out int entityId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            // Obter o repositório através do DI
            var userEntityRepository = context.HttpContext.RequestServices.GetService<IUserEntityRepository>();
            if (userEntityRepository == null)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            // Verificar se o usuário está vinculado à entidade requisitada
            var userEntity = await userEntityRepository.GetUserEntityAsync(userId);
            if (userEntity == null || userEntity.Value.entityType != _entityType || userEntity.Value.entityId != entityId)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}