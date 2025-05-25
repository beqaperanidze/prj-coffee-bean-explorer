namespace CoffeeBeanExplorer.Configuration;

public class RateLimitSettings
{
    public int PermitLimit { get; set; }
    public int WindowMinutes { get; set; }
    public int QueueLimit { get; set; }
}
