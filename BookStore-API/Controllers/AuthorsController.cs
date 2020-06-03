using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Get All Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Something Wrong");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Something Wrong");
            }

        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                if (authorDTO == null)
                    return BadRequest(ModelState);
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if(!isSuccess)
                    return StatusCode(500, "Something Wrong");

                return Created("Created ",new { author});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something Wrong");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                if(id<1 || authorDTO==null ||id !=authorDTO.Id)
                    return BadRequest();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return StatusCode(500, "Something Wrong");
                }
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Something Wrong");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if(id<1)
                    return BadRequest();
                var author =await _authorRepository.FindById(id);
                if (author == null)
                {
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return StatusCode(500, "Something Wrong");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something Wrong");
            }
        }

    }
}