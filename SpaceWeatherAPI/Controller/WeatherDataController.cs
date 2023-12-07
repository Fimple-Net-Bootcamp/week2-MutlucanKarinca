using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace SpaceWeatherAPI;

[Route("/api/v1/weatherDatas")]
[ApiController]
public class WeatherDataController:ControllerBase
{
    private static List<WeatherData> _weatherDatas= new List<WeatherData>()
    {
        new WeatherData{Id = 1,SpaceObjectId = 1,TemperatureC = "25",Summary = "Warm",Date = new DateTime(2023,12,07)},
        new WeatherData{Id = 2,SpaceObjectId = 2,TemperatureC = "40",Summary = "Hot",Date = new DateTime(2023,12,07)},
        new WeatherData{Id = 3,SpaceObjectId = 2,TemperatureC = "45",Summary = "Hot",Date = new DateTime(2023,12,08)},
        new WeatherData{Id = 4,SpaceObjectId = 2,TemperatureC = "50",Summary = "Hot",Date = new DateTime(2023,11,15)}


    };
    
    [HttpGet]
    public IActionResult GetAll([FromQuery] FilteringParameters filteringParameters, [FromQuery] PagingParameters pagingParameters, [FromQuery] SortingParameters sortingParameters)
    {
   
        var filteredData = _weatherDatas.AsQueryable();

        if (filteringParameters != null)
        {
            if (filteringParameters.SpaceObjectId.HasValue)
            {
                filteredData = filteredData.Where(x => x.SpaceObjectId == filteringParameters.SpaceObjectId);
            }
            if (filteringParameters.Date.HasValue)
            {
               DateTime date = filteringParameters.Date.Value.Date;

               filteredData = filteredData.Where(x => x.Date == date);
            }


        }

        if (sortingParameters != null)
        {
            if (!string.IsNullOrEmpty(sortingParameters.OrderBy))
            {
                filteredData = sortingParameters.OrderBy.ToLower() switch
                {
                    "date" => sortingParameters.SortOrder == "asc" ? 
                        filteredData.OrderBy(x => x.Date) :
                        filteredData.OrderByDescending(x => x.Date),
                    _ => filteredData, // Varsayılan sıralama
                };
            }
        }

        if (pagingParameters != null)
        {
            var page = pagingParameters.PageNumber != null ? pagingParameters.PageNumber : 1;
            var size = pagingParameters.PageSize != null ? pagingParameters.PageSize : 10;


            filteredData = filteredData.Skip((page - 1) * size).Take(size);
        }

        var result = filteredData.ToList();

        return Ok(result);
    }
    
    
    // [HttpGet]
    // public IActionResult GetAll()
    // {
    //     var result = _weatherDatas.OrderBy(x=>x.Date).ToList<WeatherData>();
    //
    //     return Ok(result);
    // }
    
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var weatherData = _weatherDatas.SingleOrDefault(p => p.SpaceObjectId == id);

        if (weatherData == null)
        {
            return NotFound(); 
        }

        return Ok(weatherData);
    }
    
    [HttpGet("spaceObjectsId/{spaceObjectId}")]
    public IActionResult GetBySpaceObjectId(int spaceObjectId)
    {
        var weatherData = _weatherDatas.SingleOrDefault(p => p.SpaceObjectId == spaceObjectId);

        if (weatherData == null)
        {
            return NotFound(); 
        }

        return Ok(weatherData);
    }

    [HttpPost]
    public IActionResult Create([FromBody] WeatherData weatherData)
    {
        if (weatherData == null)
        {
            return BadRequest(); 
        }

        _weatherDatas.Add(weatherData);
    
        return CreatedAtAction(nameof(GetById), new { id = weatherData.SpaceObjectId }, weatherData);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] WeatherData weatherData)
    {
        if (weatherData == null || id != weatherData.Id)
        {
            return BadRequest();
        }
        WeatherData weatherDataToUpdate = _weatherDatas.SingleOrDefault(p => p.Id == weatherData.Id);

        if (weatherDataToUpdate == null)
        {
            return NotFound(); 
        }
        weatherDataToUpdate.TemperatureC = weatherData.TemperatureC;
        weatherDataToUpdate.Summary = weatherData.Summary;
        weatherDataToUpdate.SpaceObjectId = weatherData.SpaceObjectId;
        weatherDataToUpdate.Date = weatherData.Date;

        return Ok(); 
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        WeatherData weatherDataToDelete = _weatherDatas.SingleOrDefault(p => p.Id == id);
        
        if (weatherDataToDelete == null)
        {
            return NotFound(); 
        }
        _weatherDatas.Remove(weatherDataToDelete);
        

        return NoContent(); 
    }
    
    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] JsonPatchDocument<WeatherData> patchDocument)
    {
        if (patchDocument == null)
        {
            return BadRequest();
        }

        var weatherData = _weatherDatas.SingleOrDefault(p => p.Id == id);

        if (weatherData == null)
        {
            return NotFound();
        }

        patchDocument.ApplyTo(weatherData, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }

}