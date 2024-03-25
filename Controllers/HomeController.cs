using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace UserRoleAdmin.Controllers;

public class HomeController(UserManager<AppUser> usrMgr, IUserValidator<AppUser> userValid, IPasswordValidator<AppUser> passValid, IPasswordHasher<AppUser> passHash) : Controller
{
  private readonly UserManager<AppUser> userManager = usrMgr;
  private IUserValidator<AppUser> userValidator = userValid;
  private IPasswordValidator<AppUser> passwordValidator = passValid;
  private IPasswordHasher<AppUser> passwordHasher = passHash;

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

  [HttpPost]
  public async Task<IActionResult> Delete(string id)
  {
    AppUser user = await userManager.FindByIdAsync(id);
    if (user != null)
    {
      IdentityResult result = await userManager.DeleteAsync(user);
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
      ModelState.AddModelError("", "User Not Found");
    }
    return View("Index", userManager.Users);
  }

  public async Task<IActionResult> Edit(string id)
  {
    AppUser user = await userManager.FindByIdAsync(id);
    if (user != null)
    {
      return View(user);
    }
    else
    {
      return RedirectToAction("Index");
    }
  }

  [HttpPost]
  public async Task<IActionResult> Edit(string id, string email, string password)
  {
    AppUser user = await userManager.FindByIdAsync(id);
    if (user != null)
    {
      user.Email = email;
      IdentityResult validEmail = await userValidator.ValidateAsync(userManager, user);
      if (!validEmail.Succeeded)
      {
        AddErrorsFromResult(validEmail);
      }
      IdentityResult validPass = null;
      if (!string.IsNullOrEmpty(password))
      {
        validPass = await passwordValidator.ValidateAsync(userManager, user, password);
        if (validPass.Succeeded)
        {
          user.PasswordHash = passwordHasher.HashPassword(user, password);
        }
        else
        {
          AddErrorsFromResult(validPass);
        }
      }
      if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
      {
        IdentityResult result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
          return RedirectToAction("Index");
        }
        else
        {
          AddErrorsFromResult(result);
        }
      }
    }
    else
    {
      ModelState.AddModelError("", "User Not Found");
    }
    return View(user);
  }

  private void AddErrorsFromResult(IdentityResult result)
  {
    foreach (IdentityError error in result.Errors)
    {
      ModelState.AddModelError("", error.Description);
    }
  }
}
