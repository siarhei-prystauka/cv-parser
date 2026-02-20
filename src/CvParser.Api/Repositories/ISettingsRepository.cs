using CvParser.Api.Models;

namespace CvParser.Api.Repositories;

public interface ISettingsRepository
{
    Task<ApplicationSetting> GetAsync();
    Task UpdateAsync(ApplicationSetting setting);
}
