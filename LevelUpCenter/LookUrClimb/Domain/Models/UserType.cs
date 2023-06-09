namespace LevelUpCenter.LookUrClimb.Domain.Models;

public class UserType
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string TypeOfUser { get; set; }

    //relationships
    public IList<Publication> Publications = new List<Publication>();
    public IList<Game> Games = new List<Game>();
    public IList<UserCoach> UserCoaches = new List<UserCoach>();
}