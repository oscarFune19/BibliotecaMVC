using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data
{
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";
        public const string BibliotecarioRole = "Bibliotecario";
        public const string UsuarioRole = "Usuario";

        public const string RolesConsulta = "Admin,Bibliotecario,Usuario";
        public const string RolesGestion = "Admin,Bibliotecario";

        private const string RolAdministradorAnterior = "Administrador";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            await CrearRolSiNoExisteAsync(roleManager, AdminRole);
            await CrearRolSiNoExisteAsync(roleManager, BibliotecarioRole);
            await CrearRolSiNoExisteAsync(roleManager, UsuarioRole);

            var usuarios = await userManager.Users
                .OrderBy(u => u.Id)
                .ToListAsync();

            // Migra el rol de la Semana 13 de "Administrador" a "Admin".
            if (await roleManager.RoleExistsAsync(RolAdministradorAnterior))
            {
                var administradoresAnteriores =
                    await userManager.GetUsersInRoleAsync(RolAdministradorAnterior);

                foreach (var usuario in administradoresAnteriores)
                {
                    await AsignarRolExclusivoAsync(userManager, usuario, AdminRole);
                }
            }

            // Garantiza que exista al menos un Admin cuando ya hay usuarios.
            var admins = await userManager.GetUsersInRoleAsync(AdminRole);

            if (admins.Count == 0 && usuarios.Count > 0)
            {
                await AsignarRolExclusivoAsync(userManager, usuarios[0], AdminRole);
            }

            // Para la práctica, asigna un Bibliotecario a un usuario existente
            // que no sea Admin, si todavía no hay ninguno.
            var bibliotecarios = await userManager.GetUsersInRoleAsync(BibliotecarioRole);

            if (bibliotecarios.Count == 0)
            {
                foreach (var usuario in usuarios)
                {
                    if (!await userManager.IsInRoleAsync(usuario, AdminRole))
                    {
                        await AsignarRolExclusivoAsync(
                            userManager,
                            usuario,
                            BibliotecarioRole);

                        break;
                    }
                }
            }

            // Cualquier usuario restante queda con el rol Usuario.
            foreach (var usuario in usuarios)
            {
                var esAdmin = await userManager.IsInRoleAsync(usuario, AdminRole);
                var esBibliotecario =
                    await userManager.IsInRoleAsync(usuario, BibliotecarioRole);
                var esUsuario = await userManager.IsInRoleAsync(usuario, UsuarioRole);

                if (!esAdmin && !esBibliotecario && !esUsuario)
                {
                    await AsignarRolExclusivoAsync(
                        userManager,
                        usuario,
                        UsuarioRole);
                }
            }

            // Elimina el rol anterior una vez que sus usuarios fueron migrados.
            if (await roleManager.RoleExistsAsync(RolAdministradorAnterior))
            {
                var rolAnterior =
                    await roleManager.FindByNameAsync(RolAdministradorAnterior);

                if (rolAnterior != null)
                {
                    var resultado = await roleManager.DeleteAsync(rolAnterior);

                    if (!resultado.Succeeded)
                    {
                        throw new InvalidOperationException(
                            "No se pudo eliminar el rol anterior Administrador.");
                    }
                }
            }
        }

        private static async Task CrearRolSiNoExisteAsync(
            RoleManager<IdentityRole> roleManager,
            string nombreRol)
        {
            if (await roleManager.RoleExistsAsync(nombreRol))
                return;

            var resultado =
                await roleManager.CreateAsync(new IdentityRole(nombreRol));

            if (!resultado.Succeeded)
            {
                throw new InvalidOperationException(
                    $"No se pudo crear el rol {nombreRol}.");
            }
        }

        private static async Task AsignarRolExclusivoAsync(
            UserManager<IdentityUser> userManager,
            IdentityUser usuario,
            string rolObjetivo)
        {
            var rolesActuales = await userManager.GetRolesAsync(usuario);

            var rolesGestionados = new[]
            {
                AdminRole,
                BibliotecarioRole,
                UsuarioRole,
                RolAdministradorAnterior
            };

            var rolesAEliminar = rolesActuales
                .Where(r =>
                    rolesGestionados.Contains(r) &&
                    !string.Equals(
                        r,
                        rolObjetivo,
                        StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (rolesAEliminar.Length > 0)
            {
                var resultadoEliminar =
                    await userManager.RemoveFromRolesAsync(
                        usuario,
                        rolesAEliminar);

                if (!resultadoEliminar.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"No se pudieron actualizar los roles de {usuario.UserName}.");
                }
            }

            if (!await userManager.IsInRoleAsync(usuario, rolObjetivo))
            {
                var resultadoAgregar =
                    await userManager.AddToRoleAsync(usuario, rolObjetivo);

                if (!resultadoAgregar.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"No se pudo asignar el rol {rolObjetivo} a {usuario.UserName}.");
                }
            }

            if (!await userManager.IsInRoleAsync(usuario, rolObjetivo))
            {
                throw new InvalidOperationException(
                    $"No se pudo verificar el rol {rolObjetivo} de {usuario.UserName}.");
            }
        }
    }
}
