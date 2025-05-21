using Microsoft.AspNetCore.Mvc;
using StudentEmplacementApp.DTOs;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace StudentEmplacementApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmplacementController : ControllerBase
    {
        private readonly EmplacementService _emplacementService;
        private readonly IStudentRepository _studentRepository;

        public EmplacementController(EmplacementService emplacementService , IStudentRepository studentRepository)
        {
            _emplacementService = emplacementService;
            _studentRepository = studentRepository;
        }



    }
}
