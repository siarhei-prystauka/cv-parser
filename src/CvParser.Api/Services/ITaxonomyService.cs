using CvParser.Api.Models.Responses;

namespace CvParser.Api.Services;

public interface ITaxonomyService
{
    IReadOnlyList<TaxonomySkill> GetTaxonomy();
}
