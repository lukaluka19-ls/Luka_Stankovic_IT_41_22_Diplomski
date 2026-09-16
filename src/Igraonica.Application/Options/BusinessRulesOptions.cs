namespace Igraonica.Application.Options;

public class BusinessRulesOptions
{
    public const string SectionName = "BusinessRules";

    public int WeekdayCapacity { get; set; } = 1;
    public int WeekendCapacity { get; set; } = 3;
    public int ConfirmationDeadlineDays { get; set; } = 5;
    public int CancellationNoticeDays { get; set; } = 15;
    public int MaxAdvanceBookingDays { get; set; } = 180;
    public int MinCancellationReasonLength { get; set; } = 10;
}
