using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEmplacementApp.Data;
using StudentEmplacementApp.DTOs;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;
using StudentEmplacementApp.Repositories;
using System.Collections.Generic;

namespace StudentEmplacementApp.Controllers
{
    /// <summary>
    /// Manages operations related to choices, such as retrieving all choices or a specific choice by code.
    /// </summary>
    [ApiController]
    [Route("api/choices")]
    public class ChoiceController : ControllerBase
    {
        private readonly IChoiceRepository _choiceRepository;
        private readonly IMapper _mapper;

        public ChoiceController(IChoiceRepository choiceRepository, IMapper mapper)
        {
            _choiceRepository = choiceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all choices.
        /// </summary>
        /// <returns>A list of choice objects.</returns>
        /// <response code="200">Returns the list of choices (empty if none found).</response>
        /// <response code="500">If an unexpected error occurs while retrieving choices.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChoiceDto>>> GetAllChoicesAsync()
        {
            try
            {
                var choices = await _choiceRepository.GetAllChoicesAsync();
                var choiceDtos = _mapper.Map<IEnumerable<ChoiceDto>>(choices);
                return Ok(choiceDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving choices.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific choice by its code.
        /// </summary>
        /// <param name="code">The unique code of the choice (must be a positive integer).</param>
        /// <returns>The choice object if found.</returns>
        /// <response code="200">Returns the choice details.</response>
        /// <response code="400">If the code is invalid (e.g., non-positive).</response>
        /// <response code="404">If no choice is found with the specified code.</response>
        /// <response code="500">If an unexpected error occurs while retrieving the choice.</response>
        [HttpGet("{code}")]
        public async Task<ActionResult<ChoiceDto>> GetChoiceByCodeAsync(int code)
        {
            if (code <= 0)
            {
                return BadRequest(new { message = "Choice code must be a positive integer." });
            }

            try
            {
                var choice = await _choiceRepository.FindChoiceByCodeAsync(code);
                if (choice == null)
                {
                    return NotFound(new { message = "Choice not found." });
                }

                var choiceDto = _mapper.Map<ChoiceDto>(choice);
                return Ok(choiceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the choice.", error = ex.Message });
            }
        }
    }
}