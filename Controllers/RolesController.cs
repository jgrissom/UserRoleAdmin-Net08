using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace UserRoleAdmin.Controllers
{
  public class RolesController(RoleManager<IdentityRole> roleMgr,  UserManager<AppUser> userMgr) : Controller
  {
    private readonly RoleManager<IdentityRole> roleManager = roleMgr;
    private readonly UserManager<AppUser> userManager = userMgr;

    public IActionResult Index() => View(roleManager.Roles.ToList());
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
    public async Task<IActionResult> Edit(string id)
    {
      IdentityRole role = await roleManager.FindByIdAsync(id);
      List<AppUser> members = [];
      List<AppUser> nonMembers = [];
      foreach (AppUser user in userManager.Users.ToList())
      {
        var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
        list.Add(user);
      }
      return View(new RoleEdit
      {
        Role = role,
        Members = members,
        NonMembers = nonMembers
      });
    }
    [HttpPost]
    public async Task<IActionResult> Edit(RoleModification model)
    {
      IdentityResult result;
      if (ModelState.IsValid)
      {
        foreach (string userId in model.IdsToAdd ?? [])
        {
          AppUser user = await userManager.FindByIdAsync(userId);
          if (user != null)
          {
            result = await userManager.AddToRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
            {
              AddErrorsFromResult(result);
            }
          }
        }
        foreach (string userId in model.IdsToDelete ?? [])
        {
          AppUser user = await userManager.FindByIdAsync(userId);
          if (user != null)
          {
            result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
            if (!result.Succeeded)
            {
              AddErrorsFromResult(result);
            }
          }
        }
      }
      return await Edit(model.RoleId);
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
