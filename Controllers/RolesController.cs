using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserRoleAdmin.Controllers
{
  public class RolesController(RoleManager<IdentityRole> roleMgr) : Controller
  {
    private readonly RoleManager<IdentityRole> roleManager = roleMgr;

    public IActionResult Index() => View(roleManager.Roles);
  }
}
