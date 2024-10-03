

using BackendApp.Data;
using BackendApp.Model;

namespace BackendApp.Service;

public interface IAdminUserService
{
    AdminUser? GetAdminByEmail(string email); 
    AdminUser[] AllAdmins();
}

public sealed class AdminUserService 
(ApiContext context)
: IAdminUserService
{
    private readonly ApiContext context = context;
    public AdminUser? GetAdminByEmail(string email)
    {
        return this.context.AdminUsers.FirstOrDefault( admin => admin.Email == email );
    }

    public AdminUser[] AllAdmins() => this.context.AdminUsers.ToArray(); 
}