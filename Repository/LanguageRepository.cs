using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<PagedList<Language>> GetAllActiveLanguages([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var languageDetails = FindAll()
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LanguageName.Contains(searchParams.SearchValue) ||
                      inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Language>.ToPagedList(languageDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<PagedList<Language>> GetAllLanguages([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var languageDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LanguageName.Contains(searchParams.SearchValue) ||
                inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Language>.ToPagedList(languageDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
