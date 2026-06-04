namespace GovFlow.Application.Organization.Dtos;

public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTime CreatedAt);
