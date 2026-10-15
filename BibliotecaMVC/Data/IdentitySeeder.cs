using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data
{
    public static class IdentitySeeder
    {
        public const string AdministradorRole = "Administrador";
        public const string UsuarioRole = "Usuario";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roleManager.RoleExistsAsync(AdministradorRole))
            {
                await roleManager.CreateAsync(new IdentityRole(AdministradorRole));
            }

            if (!await roleManager.RoleExistsAsync(UsuarioRole))
            {
                await roleManager.CreateAsync(new IdentityRole(UsuarioRole));
            }

            var usuarios = await userManager.Users
                .OrderBy(u => u.Id)
                .ToListAsync();

            var administradores = await userManager.GetUsersInRoleAsync(AdministradorRole);

            if (administradores.Count == 0 && usuarios.Count > 0)
            {
                var primerUsuario = usuarios[0];

                if (await userManager.IsInRoleAsync(primerUsuario, UsuarioRole))
                {
                    await userManager.RemoveFromRoleAsync(primerUsuario, UsuarioRole);
                }

                await userManager.AddToRoleAsync(primerUsuario, AdministradorRole);
            }

            foreach (var usuario in usuarios)
            {
                var esAdministrador = await userManager.IsInRoleAsync(usuario, AdministradorRole);
                var esUsuario = await userManager.IsInRoleAsync(usuario, UsuarioRole);

                if (!esAdministrador && !esUsuario)
                {
                    await userManager.AddToRoleAsync(usuario, UsuarioRole);
                }
            }
        }
    }
}
