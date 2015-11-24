﻿namespace App.Api {
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Extensions;
    using Microsoft.AspNet.Mvc;
    using Models.Domain;
    using Models.Domain.Repositories;
    using Models.Domain.Services;
    using Models.DTO;

    public class CategoryController : BaseEntityController {
        private readonly CategoryRepository _categoryRepository;

        public CategoryController(EntityOwnerService entityOwnerService, 
                                  CategoryRepository categoryRepository) : base(entityOwnerService) {
            this._categoryRepository = categoryRepository;
        }

        // GET: api/Category
        public IEnumerable<CategoryListing> Get() {
            var q = this._categoryRepository.GetByOwner(this.OwnerId);

            return q.Select(x => new CategoryListing() {
                Description = x.Description,
                Name = x.Name,
                Id = x.Id,
                CanBeDeleted = !x.SheetEntries.Any()
            }).OrderBy(x => x.Name);
        }



        // GET: api/Category/5
        public Category Get(int id) {
            return this._categoryRepository.GetByOwner(this.OwnerId).FirstOrDefault(x=>x.Id ==id).EnsureNotNull();
        }

        // POST: api/Category
        public InsertId Post([FromBody] Category value) {
            value.EnsureNotNull(HttpStatusCode.BadRequest);

            this.EntityOwnerService.AssignOwner(value, this.OwnerId);
            this._categoryRepository.Add(value);
            this._categoryRepository.SaveChanges();

            return value.Id;
        }

        // PUT: api/Category/5
        public InsertId Put(int id, [FromBody] Category value) {
            value.EnsureNotNull(HttpStatusCode.BadRequest);

            Category c = this.Get(id);
            this.EntityOwnerService.EnsureOwner(c, this.OwnerId);

            AutoMapper.Mapper.Map(value, c);
            this._categoryRepository.SaveChanges();

            return id;
        }

        // DELETE: api/Category/5
        public void Delete(int id) {
            Category c = this.Get(id);
            this.EntityOwnerService.EnsureOwner(c, this.OwnerId);

            this._categoryRepository.Delete(c);
            this._categoryRepository.SaveChanges();
        }


        
    }
}