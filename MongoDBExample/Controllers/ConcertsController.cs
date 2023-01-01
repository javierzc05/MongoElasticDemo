using Microsoft.AspNetCore.Mvc;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;
using MongoDBExample.Services;

namespace MongoDBExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConcertController : ControllerBase
    {
        private readonly ConcertService _concertService;

        public ConcertController(ConcertService concertService) =>
            _concertService = concertService;

         
        // [HttpGet]
        // [ProducesResponseType(typeof(BaseResponseGeneric<List<ConcertDtoResponse>>), 200)]
        // public async Task<IActionResult> Get(string? filter, int page = 1, int rows = 10)
        // {
        //     return Ok(await _concertService.GetAsync(filter, page, rows));
        // }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseGeneric<List<ConcertDtoResponse>>), 200)]
        public async Task<IActionResult> Get(string? filter, string? beginDate, string? endDate, int page = 1, int rows = 10)
        {
            return Ok(await _concertService.GetAsync(filter, beginDate, endDate, page, rows));
        }

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(typeof(BaseResponseGeneric<GenreDtoResponse>), 200)]
        [ProducesResponseType(typeof(BaseResponseGeneric<GenreDtoResponse>), 404)]
        public async Task<ActionResult<Genre>> Get(string id)
        {
            return Ok(await _concertService.GetAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        public async Task<IActionResult> Post(ConcertDtoRequest request)
        {
            var newConcert = await _concertService.AddAsync(request);
            // new { id = newGenre.Id } => The route data to use for generating the URL.
            return CreatedAtAction(nameof(Get), new { id = newConcert.Data }, newConcert);
        }

        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        public async Task<IActionResult> Put(string id, ConcertDtoRequest request)
        {
            return Ok(await _concertService.UpdateAsync(id, request));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _concertService.DeleteAsync(id));
        }
    }
}