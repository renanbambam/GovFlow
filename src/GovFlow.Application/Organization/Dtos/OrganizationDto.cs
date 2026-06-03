namespace GovFlow.Application.Organization.Dtos;

/// <summary>Read model for an organization.</summary>
public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTime CreatedAt);
