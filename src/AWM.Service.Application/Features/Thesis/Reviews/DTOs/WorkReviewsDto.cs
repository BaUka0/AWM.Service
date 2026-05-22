namespace AWM.Service.Application.Features.Thesis.Reviews.DTOs;

public class WorkReviewsDto
{
    public IReadOnlyList<WorkReviewDto> Reviews { get; set; } = Array.Empty<WorkReviewDto>();
}
