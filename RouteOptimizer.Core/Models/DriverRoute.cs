namespace RouteOptimizer.Core.Models;

public class DriverRoute
{
    public string DriverId { get; set; } = string.Empty;
    public List<Trip> Trips { get; set; } = new();
    public TimeSpan TotalDrivingTime { get; set; }
    public double TotalDistance { get; set; }
    public TimeSpan ShiftStart { get; set; }
    public TimeSpan ShiftEnd { get; set; }
} 