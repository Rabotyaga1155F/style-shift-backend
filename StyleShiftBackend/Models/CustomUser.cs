using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StyleShiftBackend.Models;

public class CustomUser : IdentityUser
{
    [Key]
    public bool Verification { get; set; } = false;
}