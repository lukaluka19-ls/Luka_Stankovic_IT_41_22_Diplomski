namespace Igraonica.Application.DTOs;

public sealed record PagedQuery(int Page = 1, int PageSize = 20);

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
