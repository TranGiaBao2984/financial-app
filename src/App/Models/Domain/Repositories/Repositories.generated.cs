﻿


// <autogenerated>
//   This file was generated using Repositories.tt.
//   Any changes made manually will be lost next time the file is regenerated.
// </autogenerated>


namespace App.Models.Domain.Repositories {
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public sealed partial class SheetEntryRepository {
        private readonly DbContext _dbContext;
        private readonly DbSet<App.Models.Domain.SheetEntry> _entitySet;

        public SheetEntryRepository(DbContext dbContext) {
            this._dbContext = dbContext;
            this._entitySet = dbContext.Set<App.Models.Domain.SheetEntry>();
        }

        [CanBeNull]
        public App.Models.Domain.SheetEntry FindById(int id) {
            return this._entitySet.Where(x => x.Id == id).Include(x => x.Sheet).Include(x => x.Sheet.Owner).Include(x => x.Category).FirstOrDefault();
        }

        [CanBeNull]
        public Task<App.Models.Domain.SheetEntry> FindByIdAsync(int id) {
            return this._entitySet.FirstOrDefaultAsync(x => x.Id == id);
        }

        [NotNull]
        public IQueryable<App.Models.Domain.SheetEntry> GetAll() {
            return this._entitySet.Include(x => x.Sheet).Include(x => x.Category);
        }

        

        public void Add(App.Models.Domain.SheetEntry item) {
            this._entitySet.Add(item);
        }

		public void Delete(App.Models.Domain.SheetEntry item) {
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

		public void DeleteById(int id) {
			App.Models.Domain.SheetEntry item = this.FindById(id);
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

        public int SaveChanges() {
            return this._dbContext.SaveChanges();
        }

        public Task SaveChangesAsync() {
            return this._dbContext.SaveChangesAsync();
        }
    }

    public sealed partial class RecurringSheetEntryRepository {
        private readonly DbContext _dbContext;
        private readonly DbSet<App.Models.Domain.RecurringSheetEntry> _entitySet;

        public RecurringSheetEntryRepository(DbContext dbContext) {
            this._dbContext = dbContext;
            this._entitySet = dbContext.Set<App.Models.Domain.RecurringSheetEntry>();
        }

        [CanBeNull]
        public App.Models.Domain.RecurringSheetEntry FindById(int id) {
            return this._entitySet.Where(x => x.Id == id).Include(x => x.Category).Include(x => x.Owner).FirstOrDefault();
        }

        [CanBeNull]
        public Task<App.Models.Domain.RecurringSheetEntry> FindByIdAsync(int id) {
            return this._entitySet.Where(x => x.Id == id).Include(x => x.Category).Include(x => x.Owner).FirstOrDefaultAsync();
        }

        [NotNull]
        public IQueryable<App.Models.Domain.RecurringSheetEntry> GetAll() {
            return this._entitySet.Include(x => x.Category);
        }

        

        public void Add(App.Models.Domain.RecurringSheetEntry item) {
            this._entitySet.Add(item);
        }

		public void Delete(App.Models.Domain.RecurringSheetEntry item) {
			if (item != null) {
                this._dbContext.Database.ExecuteSqlCommand("UPDATE dbo.SheetEntry SET TemplateId = NULL WHERE TemplateId = @p0", item.Id);
				this._entitySet.Remove(item);
			}
		}

		public void DeleteById(int id) {
			App.Models.Domain.RecurringSheetEntry item = this.FindById(id);
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

        public int SaveChanges() {
            return this._dbContext.SaveChanges();
        }

        public Task SaveChangesAsync() {
            return this._dbContext.SaveChangesAsync();
        }

        public IEnumerable<RecurringSheetEntry> GetByOwner(int ownerId) {
            return this._entitySet.Where(x => x.Owner.Id == ownerId);
        }
    }

        
    public sealed partial class CategoryRepository {
        private readonly DbContext _dbContext;
        private readonly DbSet<App.Models.Domain.Category> _entitySet;

        public CategoryRepository(DbContext dbContext) {
            this._dbContext = dbContext;
            this._entitySet = dbContext.Set<App.Models.Domain.Category>();
        }

        [CanBeNull]
        public App.Models.Domain.Category FindById(int id) {
            return this._entitySet.FirstOrDefault(x => x.Id == id);
        }

        [CanBeNull]
        public Task<App.Models.Domain.Category> FindByIdAsync(int id) {
            return this._entitySet.FirstOrDefaultAsync(x => x.Id == id);
        }

        [NotNull]
        public IQueryable<App.Models.Domain.Category> GetAll() {
            return this._entitySet.Include(x => x.Owner);
        }

        
        [NotNull]
        public IQueryable<App.Models.Domain.Category> GetByOwner(App.Models.Domain.AppOwner owner) {
            return this._entitySet.Where(x => x.Owner.Id == owner.Id);
        }

        [NotNull]
        public IQueryable<App.Models.Domain.Category> GetByOwner(int ownerId) {
            return this._entitySet.Where(x => x.Owner.Id == ownerId).Include(x => x.Owner);
        }

                

        public void Add(App.Models.Domain.Category item) {
            this._entitySet.Add(item);
        }

		public void Delete(App.Models.Domain.Category item) {
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

		public void DeleteById(int id) {
			App.Models.Domain.Category item = this.FindById(id);
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

        public int SaveChanges() {
            return this._dbContext.SaveChanges();
        }

        public Task SaveChangesAsync() {
            return this._dbContext.SaveChangesAsync();
        }
    }

        
    public sealed partial class SheetRepository {
        private readonly DbContext _dbContext;
        private readonly DbSet<App.Models.Domain.Sheet> _entitySet;

        public SheetRepository(DbContext dbContext) {
            this._dbContext = dbContext;
            this._entitySet = dbContext.Set<App.Models.Domain.Sheet>();
        }

        [CanBeNull]
        public App.Models.Domain.Sheet FindById(int id) {
            return this._entitySet.FirstOrDefault(x => x.Id == id);
        }

        [CanBeNull]
        public Task<App.Models.Domain.Sheet> FindByIdAsync(int id) {
            return this._entitySet.FirstOrDefaultAsync(x => x.Id == id);
        }

        [NotNull]
        public IQueryable<App.Models.Domain.Sheet> GetAll() {
            return this._entitySet.Include(x => x.Owner).Include(x => x.CalculationOptions);
        }

        
        [NotNull]
        public IQueryable<App.Models.Domain.Sheet> GetByOwner(App.Models.Domain.AppOwner owner) {
            return this._entitySet.Where(x => x.Owner.Id == owner.Id);
        }

        [NotNull]
        public IQueryable<App.Models.Domain.Sheet> GetByOwner(int ownerId) {
            return this._entitySet.Where(x => x.Owner.Id == ownerId);
        }

                

        public void Add(App.Models.Domain.Sheet item) {
            this._entitySet.Add(item);
        }

		public void Delete(App.Models.Domain.Sheet item) {
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

		public void DeleteById(int id) {
			App.Models.Domain.Sheet item = this.FindById(id);
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

        public int SaveChanges() {
            return this._dbContext.SaveChanges();
        }

        public Task SaveChangesAsync() {
            return this._dbContext.SaveChangesAsync();
        }
    }

        
    public sealed partial class AppOwnerRepository {
        private readonly DbContext _dbContext;
        private readonly DbSet<App.Models.Domain.AppOwner> _entitySet;

        public AppOwnerRepository(DbContext dbContext) {
            this._dbContext = dbContext;
            this._entitySet = dbContext.Set<App.Models.Domain.AppOwner>();
        }

        [CanBeNull]
        public App.Models.Domain.AppOwner FindById(int id) {
            return this._entitySet.FirstOrDefault(x => x.Id == id);
        }

        [CanBeNull]
        public Task<App.Models.Domain.AppOwner> FindByIdAsync(int id) {
            return this._entitySet.FirstOrDefaultAsync(x => x.Id == id);
        }

        [NotNull]
        public IQueryable<App.Models.Domain.AppOwner> GetAll() {
            return this._entitySet;
        }

        

        public void Add(App.Models.Domain.AppOwner item) {
            this._entitySet.Add(item);
        }

		public void Delete(App.Models.Domain.AppOwner item) {
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

		public void DeleteById(int id) {
			App.Models.Domain.AppOwner item = this.FindById(id);
			if (item != null) {
				this._entitySet.Remove(item);
			}
		}

        public int SaveChanges() {
            return this._dbContext.SaveChanges();
        }

        public Task SaveChangesAsync() {
            return this._dbContext.SaveChangesAsync();
        }
    }

    
	internal static class RepositoryRegistry {
		internal static void InsertIn(IServiceCollection c) {
							c.AddTransient<SheetEntryRepository>();
							c.AddTransient<CategoryRepository>();
							c.AddTransient<SheetRepository>();
							c.AddTransient<AppOwnerRepository>();
							c.AddTransient<RecurringSheetEntryRepository>();
					}
	}
}

