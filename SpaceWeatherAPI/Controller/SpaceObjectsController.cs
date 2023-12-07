using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace SpaceWeatherAPI;

[Route("/api/v1/spaceObjects")]
[ApiController]
public class SpaceObjectsController:ControllerBase
{
    private static List<SpaceObject> _planets = new List<SpaceObject>()
    {
        new SpaceObject{Id = 1,Name = "Earth",Type = "SpaceObject"},
        new SpaceObject{Id = 2,Name="Moon",Type="Natural Satellite"}

    };
    
   
    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _planets.OrderBy(x=>x.Id).ToList<SpaceObject>();

        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var planet = _planets.SingleOrDefault(p => p.Id == id);

        if (planet == null)
        {
            return NotFound(); 
        }

        return Ok(planet);
    }

    [HttpPost]
    public IActionResult Create([FromBody] SpaceObject spaceObject)
    {
        if (spaceObject == null)
        {
            return BadRequest(); 
        }

        _planets.Add(spaceObject);

        return CreatedAtAction(nameof(GetById), new { id = spaceObject.Id }, spaceObject);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] SpaceObject spaceObject)
    {
        if (spaceObject == null || id != spaceObject.Id)
        {
            return BadRequest(); 
        }
        SpaceObject spaceObjectToUpdate = _planets.SingleOrDefault(p => p.Id == spaceObject.Id);

        if (spaceObjectToUpdate == null)
        {
            return NotFound(); 
        }
        spaceObjectToUpdate.Name = spaceObject.Name;
        spaceObjectToUpdate.Type = spaceObject.Type;

        return Ok(); 
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        SpaceObject spaceObjectToDelete = _planets.SingleOrDefault(p => p.Id == id);
        
        if (spaceObjectToDelete == null)
        {
            return NotFound(); 
        }
        _planets.Remove(spaceObjectToDelete);
        

        return NoContent(); 
    }
    
    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] JsonPatchDocument<SpaceObject> patchDocument)
    {
        if (patchDocument == null)
        {
            return BadRequest();
        }

        var spaceObject = _planets.SingleOrDefault(p => p.Id == id);

        if (spaceObject == null)
        {
            return NotFound();
        }

        patchDocument.ApplyTo(spaceObject, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }
}