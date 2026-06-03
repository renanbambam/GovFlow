namespace GovFlow.API.Contracts;

/// <summary>Standard response returned by creation endpoints.</summary>
/// <param name="Id">Identifier of the newly created resource.</param>
public sealed record CreatedResponse(Guid Id);
