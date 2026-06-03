using GovFlow.Application.Common.Exceptions;
using GovFlow.Domain.Common;
using GovFlow.Domain.Organization;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.CreateProcessType;

public sealed class CreateProcessTypeCommandHandler : IRequestHandler<CreateProcessTypeCommand, Guid>
{
    private readonly IProcessTypeRepository _processTypes;
    private readonly IOrganizationRepository _organizations;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProcessTypeCommandHandler(
        IProcessTypeRepository processTypes,
        IOrganizationRepository organizations,
        IUnitOfWork unitOfWork)
    {
        _processTypes = processTypes;
        _organizations = organizations;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProcessTypeCommand request, CancellationToken cancellationToken)
    {
        if (await _organizations.GetByIdAsync(request.OrganizationId, cancellationToken) is null)
            throw NotFoundException.For("Organization", request.OrganizationId);

        var processType = ProcessType.Create(request.Name, request.Description, request.OrganizationId);

        foreach (var step in request.Steps)
        {
            processType.AddStep(
                step.Name,
                step.Description,
                step.AssignableDepartmentId,
                step.SlaHours,
                step.RequiredDocuments);
        }

        await _processTypes.AddAsync(processType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return processType.Id;
    }
}
