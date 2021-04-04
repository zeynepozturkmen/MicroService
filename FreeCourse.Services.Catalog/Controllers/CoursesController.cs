using FreeCource.Shared.ControllerBases;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Controllers
{
    [Route("api/courses/")]
    [ApiController]
    internal class CoursesController : CustomBaseController
    {
        private readonly ICourseService _courseService;
        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{Id}")]
        [Route("getById")]
        public async Task<IActionResult> GetById(string Id)
        {
            var response = await _courseService.GetByIdAsync(Id);

            return CreateActionResultInstance(response);
        }

        [Route("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseService.GetAllAsync();

            return CreateActionResultInstance(response);
        }

        [HttpGet("{Id}")]
        [Route("getAllByUserId")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserId(userId);

            return CreateActionResultInstance(response);
        }

        [HttpPost("{Id}")]
        [Route("create")]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            var response = await _courseService.CreateAsync(courseCreateDto);

            return CreateActionResultInstance(response);
        }


        [HttpPut("{Id}")]
        [Route("create")]
        public async Task<IActionResult> Update(CourseUpdateDto courseCreateDto)
        {
            var response = await _courseService.UpdateAsync(courseCreateDto);

            return CreateActionResultInstance(response);
        }


        [HttpDelete("{Id}")]
        [Route("delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            var response = await _courseService.DeleteAsync(Id);

            return CreateActionResultInstance(response);
        }


    }
}
