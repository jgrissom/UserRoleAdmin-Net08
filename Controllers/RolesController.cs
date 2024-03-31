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
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
      IdentityRole role = await roleManager.FindByIdAsync(id);
      if (role != null)
      {
        IdentityResult result = await roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
          return RedirectToAction("Index");
        }
        else
        {
          AddErrorsFromResult(result);
        }
      }
      else
      {
        ModelState.AddModelError("", "No role found");
      }
      return View("Index", roleManager.Roles);
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
