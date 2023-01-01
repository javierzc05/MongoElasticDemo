using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;
using MongoDBExample.Entities;
using MongoDBExample.Services;

namespace MongoDBExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly GenreService _genreService;

        public GenreController(GenreService genreService) =>
            _genreService = genreService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _genreService.GetAsync());
        }

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(typeof(BaseResponseGeneric<GenreDtoResponse>), 200)]
        [ProducesResponseType(typeof(BaseResponseGeneric<GenreDtoResponse>), 404)]
        [Authorize]
        public async Task<ActionResult<Genre>> Get(string id)
        {
            return Ok(await _genreService.GetAsync(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        public async Task<IActionResult> Post(GenreDtoRequest request)
        {
            var newGenre = await _genreService.AddAsync(request);
            // new { id = newGenre.Id } => The route data to use for generating the URL.
            return CreatedAtAction(nameof(Get), new { id = newGenre.Data }, newGenre);
        }

        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        public async Task<IActionResult> Update(string id, GenreDtoRequest request)
        {
            return Ok(await _genreService.UpdateAsync(id, request));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _genreService.DeleteAsync(id));
        }
    }
}
