using System.Security.Permissions;

namespace LevelUpCenter.LookUrClimb.Domain.Models;

public class UserCoach
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Last_name { get; set; }
    
    public string Description { get; set; }
    public string Video { get; set; }
    public string Image { get; set; }
    public float Price { get; set; }
    public string Category { get; set; }
    public string InventoryStatus { get; set; }
    public float Rating { get; set; }
    public int Age { get; set; }
    public string Languaje { get; set; }
    public string Country { get; set; }

    //realtionships
    public int UserId { get; set; }
    public UserType UserType { get; set; }
}