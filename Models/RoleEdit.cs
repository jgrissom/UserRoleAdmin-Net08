using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class RoleEdit
{
  public IdentityRole Role { get; set; }
  public IEnumerable<AppUser> Members { get; set; }
  public IEnumerable<AppUser> NonMembers { get; set; }
}
public class RoleModification
{
  [Required]
  public string RoleName { get; set; }
  public string RoleId { get; set; }
  public string[] IdsToAdd { get; set; }
  public string[] IdsToDelete { get; set; }
}
