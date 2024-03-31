using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace UserRoleAdmin.Controllers
{
  public class RolesController(RoleManager<IdentityRole> roleMgr) : Controller
  {
    private readonly RoleManager<IdentityRole> roleManager = roleMgr;

    public IActionResult Index() => View(roleManager.Roles);
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create([Required]string name)
    {
      if (ModelState.IsValid)
      {
        IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
        if (result.Succeeded)
        {
          return RedirectToAction("Index");
        }
        else
        {
          AddErrorsFromResult(result);
        }
      }
      return View(name);
    }

    private void AddErrorsFromResult(IdentityResult result)
    {
      foreach (IdentityError error in result.Errors)
      {
        ModelState.AddModelError("", error.Description);
      }
    }
  }
}
