using Igraonica.Domain.Common;
using Igraonica.Domain.Enums;

namespace Igraonica.Domain.Entities;

public class Package : BaseEntity
{
    public PackageType Type { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal BasePrice { get; private set; }
    public decimal PricePerChild { get; private set; }
    public bool IsActive { get; private set; }

    private Package() { }

    public static Package Create(PackageType type, string name, string description,
        decimal basePrice, decimal pricePerChild)
    {
        if (basePrice < 0 || pricePerChild < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Cena ne sme biti negativna.");

        return new Package
        {
            Type = type,
            Name = name,
            Description = description,
            BasePrice = basePrice,
            PricePerChild = pricePerChild,
            IsActive = true,
        };
    }

    public void UpdatePricing(decimal basePrice, decimal pricePerChild)
    {
        if (basePrice < 0 || pricePerChild < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Cena ne sme biti negativna.");

        BasePrice = basePrice;
        PricePerChild = pricePerChild;
    }

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public decimal CalculatePrice(int numberOfChildren) =>
        BasePrice + PricePerChild * numberOfChildren;
}
