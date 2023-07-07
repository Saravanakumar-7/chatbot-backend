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
        Task<IEnumerable<Language>> GetAllLanguages();
        Task<Language> GetLanguageById(int id);
        Task<IEnumerable<Language>> GetAllActiveLanguages();
        Task<int?> CreateLanguage(Language language);
        Task<string> UpdateLanguage(Language language);
        Task<string> DeleteLanguage(Language language);
    }
}
