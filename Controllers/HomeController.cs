using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace UserRoleAdmin.Controllers;

public class HomeController(UserManager<AppUser> usrMgr) : Controller
{
  private readonly UserManager<AppUser> userManager = usrMgr;

  public ViewResult Index() => View(userManager.Users);
  public ViewResult Create() => View(new CreateUser());
}
