namespace TestTask.DataAccess.Models;

public record Marker(
    int MarkerId,
    string MarkerText,
    string Latitude,
    string Longitude);