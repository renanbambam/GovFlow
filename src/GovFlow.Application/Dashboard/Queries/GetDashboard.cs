using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Dashboard.Dtos;
using MediatR;

namespace GovFlow.Application.Dashboard.Queries;

public sealed record GetDashboardQuery : IRequest<DashboardDto>;

public sealed class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IDashboardReadRepository _dashboard;

    public GetDashboardQueryHandler(IDashboardReadRepository dashboard) => _dashboard = dashboard;

    public Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        => _dashboard.GetAsync(cancellationToken);
}
