using Igraonica.Domain.Common;

namespace Igraonica.Domain.Entities;

public class TimeSlot : BaseEntity
{
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private TimeSlot() { }

    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime, string name)
    {
        if (endTime <= startTime)
            throw new ArgumentException("Vreme zavrsetka mora biti posle vremena pocetka.");

        return new TimeSlot
        {
            StartTime = startTime,
            EndTime = endTime,
            Name = name,
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
