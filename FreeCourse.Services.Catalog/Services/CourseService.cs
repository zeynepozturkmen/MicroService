﻿using AutoMapper;
using FreeCource.Shared.Dtos;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    internal class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _categoryCollection= database.GetCollection<Category>(databaseSettings.CategoryCollectionName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);

            _mapper = mapper;
        }


        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync();

            if (courses.Any())
            {
                //MongoDb iliskisel veri tabanı olmadıgı icin kurs üzerinden,category'e erisilemiyor
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstOrDefaultAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }

            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstOrDefaultAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }

        //Kullanıcının kurslarını dönen method
        public async Task<Response<List<CourseDto>>> GetAllByUserId(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

            if (courses.Any())
            {
                //MongoDb iliskisel veri tabanı olmadıgı icin kurs üzerinden,category'e erisilemiyor
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstOrDefaultAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto course)
        {
            var newCourse = _mapper.Map<Course>(course);

            newCourse.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }

        //Güncellenen kayıttaki veriler user uı'da oldugu icin veri dönmeye gerek yok
        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto) {

            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);

            if (result is null)
            {
                return Response<NoContent>.Fail("Course not found", 400);
            }

            return Response<NoContent>.Success(204);

        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount>0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Course not found", 400);
            }
        }

    }
}
