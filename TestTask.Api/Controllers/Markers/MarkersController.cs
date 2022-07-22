using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using TestTask.DataAccess;
using TestTask.DataAccess.Exceptions;
using TestTask.DataAccess.Models;

namespace TestTask.Api.Controllers.Markers;

[Authorize]
public class MarkersController : Controller
{
    private readonly Database _db = new();
    
    [HttpGet("api/markers")]
    [ProducesResponseType(typeof(Marker[]), 200)]
    public IActionResult GetAllMarkers()
    {
        var markers = _db.GetAllMarkers();
        return Ok(markers);
    }
}