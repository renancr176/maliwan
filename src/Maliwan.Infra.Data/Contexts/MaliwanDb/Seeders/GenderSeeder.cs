using Maliwan.Domain.Maliwan.Entities;
using Maliwan.Domain.Maliwan.Interfaces.Repositories;
using Maliwan.Domain.Maliwan.Interfaces.Validators;
using Maliwan.Infra.Data.Contexts.MaliwanDb.Seeders.Interfaces;

namespace Maliwan.Infra.Data.Contexts.MaliwanDb.Seeders;

public class GenderSeeder : IGenderSeeder
{
    private readonly IGenderRepository _repository;
    private readonly IGenderValidator _validator;

    public GenderSeeder(IGenderRepository repository, IGenderValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task SeedAsync()
    {
        var entities = new List<Gender>()
        {
            new Gender("Masculino", "M"),
            new Gender("Feminino", "F"),
            new Gender("Unisex", "U")
        };

        foreach (var entity in entities)
        {
            if (await _validator.IsValidAsync(entity))
            {
                await _repository.InsertAsync(entity);
                await _repository.SaveChangesAsync();
            }
        }
    }
}