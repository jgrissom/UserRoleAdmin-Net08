using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UserRoleAdmin.TagHelpers
{
  [HtmlTargetElement("td", Attributes = "identity-role")]
  public class RoleUsers(UserManager<AppUser> userMgr, RoleManager<IdentityRole> roleMgr) : TagHelper
  {
    private readonly UserManager<AppUser> userManager = userMgr;
    private readonly RoleManager<IdentityRole> roleManager = roleMgr;

    [HtmlAttributeName("identity-role")]
    public string Role { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      Contract.Ensures(Contract.Result<Task>() != null);
      IdentityRole role = await roleManager.FindByIdAsync(Role);
      var u = roleManager.Roles;
      List<string> names = [];
      if (role != null)
      {
        foreach (var user in userManager.Users.ToList())
        {
          if (user != null && await userManager.IsInRoleAsync(user, role.Name))
          {
            names.Add(user.UserName);
          }
        }
      }

      output.Content.SetContent(names.Count == 0 ? "No Users" : string.Join(", ", names));
    }
  }
}
