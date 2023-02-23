using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
    {
        public LanguageRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateLanguage(Language language)
        {
            language.CreatedBy = "Admin";
            language.CreatedOn = DateTime.Now;
            language.Unit = "Bangalore";
            var result = await Create(language);
           
            return result.Id;
        }

        public async Task<string> DeleteLanguage(Language language)
        {
            Delete(language);
            string result = $"Language details of {language.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Language>> GetAllActiveLanguages()
        {
            var AllActivelanguages = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivelanguages;
        }
    

        public async Task<IEnumerable<Language>> GetAllLanguages()
        {

        var GetallLanguage = await FindAll().ToListAsync();

        return GetallLanguage;
        }

        public async Task<Language> GetLanguageById(int id)
        {
            var LanguagebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return LanguagebyId;
        }

        public async Task<string> UpdateLanguage(Language language)
        {
            language.LastModifiedBy = "Admin";
            language.LastModifiedOn = DateTime.Now;
            Update(language);
            string result = $"Language details of {language.Id} is updated successfully!";
            return result;
        }
    }
}
