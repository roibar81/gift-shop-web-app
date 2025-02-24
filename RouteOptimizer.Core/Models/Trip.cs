namespace RouteOptimizer.Core.Models;

public class Trip
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int PassengerCount { get; set; }
    public string Company { get; set; } = string.Empty;
    public TimeSpan DriverShiftStart { get; set; }
    public TimeSpan DriverShiftEnd { get; set; }
} 