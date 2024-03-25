using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace UserRoleAdmin.Controllers;

public class HomeController(UserManager<AppUser> usrMgr) : Controller
{
  private readonly UserManager<AppUser> userManager = usrMgr;

  public ViewResult Index() => View(userManager.Users);
  public ViewResult Create() => View(new CreateUser());
  [HttpPost]
  public async Task<IActionResult> Create(CreateUser model)
  {
    if (ModelState.IsValid)
    {
      AppUser user = new AppUser
      {
        UserName = model.Name,
        Email = model.Email
      };
      IdentityResult result = await userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }
      else
      {
        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }
    }
    return View(model);
  }
}
