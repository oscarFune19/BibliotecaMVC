using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.UsuarioOCorreo);

            if (user == null)
                user = await _userManager.FindByNameAsync(model.UsuarioOCorreo);

            if (user == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.Recordarme,
                false);

            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new IdentityUser
            {
                UserName = model.Usuario,
                Email = model.Correo
            };

            var resultado = await _userManager.CreateAsync(user, model.Password);

            if (resultado.Succeeded)
            {
                var admins =
                    await _userManager.GetUsersInRoleAsync(IdentitySeeder.AdminRole);

                var rol = admins.Count == 0
                    ? IdentitySeeder.AdminRole
                    : IdentitySeeder.UsuarioRole;

                var resultadoRol =
                    await _userManager.AddToRoleAsync(user, rol);

                if (!resultadoRol.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(
                        "",
                        "No fue posible asignar un rol al usuario.");
                    return View(model);
                }

                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in resultado.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
