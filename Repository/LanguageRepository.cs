using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LanguageRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateLanguage(Language language)
        {
            language.CreatedBy = _createdBy;
            language.CreatedOn = DateTime.Now;
            language.Unit = _unitname;
            var result = await Create(language);
           
            return result.Id;
        }

        public async Task<string> DeleteLanguage(Language language)
        {
            Delete(language);
            string result = $"Language details of {language.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Language>> GetAllActiveLanguages([FromQuery] SearchParames searchParams)
        {
            var languageDetails = FindAll()
                                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LanguageName.Contains(searchParams.SearchValue) ||
                                  inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return languageDetails;
        }


        public async Task<IEnumerable<Language>> GetAllLanguages([FromQuery] SearchParames searchParams)
        {
            var languageDetails = FindAll().OrderByDescending(x => x.Id)
                                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LanguageName.Contains(searchParams.SearchValue) ||
                                  inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return languageDetails;
        }

        public async Task<Language> GetLanguageById(int id)
        {
            var LanguagebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return LanguagebyId;
        }

        public async Task<string> UpdateLanguage(Language language)
        {
            language.LastModifiedBy = _createdBy;
            language.LastModifiedOn = DateTime.Now;
            Update(language);
            string result = $"Language details of {language.Id} is updated successfully!";
            return result;
        }
    }
}
