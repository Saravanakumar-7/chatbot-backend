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
            var result = await Create(language);
            language.Unit = "Bangalore";
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
            var languageList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return languageList;
        }
    

        public async Task<IEnumerable<Language>> GetAllLanguages()
        {

        var languageList = await FindAll().ToListAsync();

        return languageList;
        }

        public async Task<Language> GetLanguageById(int id)
        {
            var language = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return language;
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
