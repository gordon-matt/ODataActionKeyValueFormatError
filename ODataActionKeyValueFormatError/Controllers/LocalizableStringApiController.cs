using Extenso.AspNetCore.OData;
using Extenso.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using ODataActionKeyValueFormatError.Data.Entities;
using ODataActionKeyValueFormatError.Models;

namespace ODataActionKeyValueFormatError.Controllers
{
    public class LocalizableStringApiController : BaseODataController<LocalizableString, Guid>
    {
        public LocalizableStringApiController(IRepository<LocalizableString> repository)
            : base(repository)
        {
            EnsureTestData(); // Populate in-memory db to demo the issue
        }

        protected override Guid GetId(LocalizableString entity) => entity.Id;

        protected override void SetNewId(LocalizableString entity) => entity.Id = Guid.NewGuid();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Wrong Usage", "DF0010:Marks undisposed local variables.", Justification = "Disposable of the repository connection is handled by the `GenericODataController`.")]
        public virtual async Task<IActionResult> GetComparitiveTable(
            [FromODataUri] string cultureCode,
            ODataQueryOptions<ComparitiveLocalizableString> options)
        {
            if (!await AuthorizeAsync(ReadPermission))
            {
                return Unauthorized();
            }
            else
            {
                var connection = GetDisposableConnection();

                // With grouping, we use .Where() and then .FirstOrDefault() instead of just the .FirstOrDefault() by itself
                //  for compatibility with MySQL.
                //  See: http://stackoverflow.com/questions/23480044/entity-framework-select-statement-with-logic
                var query = connection.Query(x => x.CultureCode == null || x.CultureCode == cultureCode)
                            .GroupBy(x => x.TextKey)
                            .Select(grp => new ComparitiveLocalizableString
                            {
                                Key = grp.Key,
                                InvariantValue = grp.Where(x => x.CultureCode == null).FirstOrDefault().TextValue,
                                LocalizedValue = grp.Where(x => x.CultureCode == cultureCode).FirstOrDefault() == null
                                    ? string.Empty
                                    : grp.Where(x => x.CultureCode == cultureCode).FirstOrDefault().TextValue
                            });

                var results = options.ApplyTo(query, IgnoreQueryOptions);
                var response = await Task.FromResult((results as IQueryable<ComparitiveLocalizableString>).ToHashSet());
                return Ok(response);
            }
        }

        private void EnsureTestData()
        {
            int count = Repository.Count();
            if (count == 0)
            {
                Repository.Insert(new[]
                {
                    // Invariant
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/1", TextValue= "1" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/2", TextValue= "2" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/3", TextValue= "3" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/4", TextValue= "4" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/5", TextValue= "5" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/6", TextValue= "6" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/7", TextValue= "7" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/8", TextValue= "8" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/9", TextValue= "9" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = null, TextKey = "Numbers/10", TextValue= "10" },

                    // en-US
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/1", TextValue= "One" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/2", TextValue= "Two" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/3", TextValue= "Three" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/4", TextValue= "Four" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/5", TextValue= "Five" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/6", TextValue= "Six" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/7", TextValue= "Seven" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/8", TextValue= "Eight" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/9", TextValue= "Nine" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "en-US", TextKey = "Numbers/10", TextValue= "Ten" },

                    // vi-VN
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/1", TextValue= "Mot" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/2", TextValue= "Hai" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/3", TextValue= "Ba" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/4", TextValue= "Bon" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/5", TextValue= "Nam" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/6", TextValue= "Sau" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/7", TextValue= "Bay" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/8", TextValue= "Tam" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/9", TextValue= "Chin" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "vi-VN", TextKey = "Numbers/10", TextValue= "Muoi" },

                    // zh-CN
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/1", TextValue= "Yi" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/2", TextValue= "Er" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/3", TextValue= "San" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/4", TextValue= "Si" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/5", TextValue= "Wu" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/6", TextValue= "Liu" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/7", TextValue= "Qi" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/8", TextValue= "Ba" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/9", TextValue= "Jiu" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "zh-CN", TextKey = "Numbers/10", TextValue= "Shi" },

                    // ja-JP
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/1", TextValue= "Ichi" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/2", TextValue= "Ni" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/3", TextValue= "San" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/4", TextValue= "Yon" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/5", TextValue= "Go" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/6", TextValue= "Roku" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/7", TextValue= "Nana" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/8", TextValue= "Hachi" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/9", TextValue= "Kyuu" },
                    new LocalizableString { Id = Guid.NewGuid(), CultureCode = "ja-JP", TextKey = "Numbers/10", TextValue= "Jyuu" },
                });
            }
        }
    }
}