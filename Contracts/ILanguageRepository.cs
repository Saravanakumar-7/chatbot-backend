using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILanguageRepository : IRepositoryBase<Language>
    {
        Task<PagedList<Language>> GetAllLanguages(PagingParameter pagingParameter, SearchParames searchParames);
        Task<Language> GetLanguageById(int id);
        Task<PagedList<Language>> GetAllActiveLanguages(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateLanguage(Language language);
        Task<string> UpdateLanguage(Language language);
        Task<string> DeleteLanguage(Language language);
    }
}
