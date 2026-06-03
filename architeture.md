# GovFlow

```
 ██████╗  ██████╗ ██╗   ██╗███████╗██╗      ██████╗ ██╗    ██╗
██╔════╝ ██╔═══██╗██║   ██║██╔════╝██║     ██╔═══██╗██║    ██║
██║  ███╗██║   ██║██║   ██║█████╗  ██║     ██║   ██║██║ █╗ ██║
██║   ██║██║   ██║╚██╗ ██╔╝██╔══╝  ██║     ██║   ██║██║███╗██║
╚██████╔╝╚██████╔╝ ╚████╔╝ ██║     ███████╗╚██████╔╝╚███╔███╔╝
 ╚═════╝  ╚═════╝   ╚═══╝  ╚═╝     ╚══════╝ ╚═════╝  ╚══╝╚══╝
  G O V F L O W   —   P R O C E S S   M A N A G E M E N T
```

**Enterprise process and workflow management platform.**  
Multi-tenant. Event-driven. Real-time. Built with ASP.NET Core 9 + Clean Architecture.

---

## Overview

GovFlow is a configurable digital process tramitation platform — the kind that powers government agencies, public institutions, and enterprise operations teams. Each organization registers its own **process types**, defines **workflow steps** per type, and opens **process instances** that move through departments and people until resolution.

It is not a simple CRUD. It is a workflow engine with:

- configurable multi-step process flows per organization
- document attachments per process step
- real-time notifications via SignalR
- SLA tracking with automatic escalation via Hangfire
- full audit trail of every state transition
- role-based access with per-organization permission scopes
- dashboard views per role (submitter, analyst, manager, admin)

---

## Stack

| Layer | Technology | Version | Role |
|---|---|---|---|
| Runtime | .NET | 9 | Primary platform |
| Web framework | ASP.NET Core | 9 | HTTP API |
| ORM | Entity Framework Core | 9 | PostgreSQL access |
| Database | PostgreSQL | 16+ | Primary persistence |
| Migrations | EF Core Migrations | built-in | Schema versioning |
| CQRS mediator | MediatR | 12+ | Command/Query handling |
| Validation | FluentValidation | 11+ | Input validation (pipeline behavior) |
| Real-time | SignalR | built-in | Live notifications to clients |
| Background jobs | Hangfire | 1.8+ | SLA timers, escalations, digests |
| Cache | Redis | 7+ | Session state, pub/sub for SignalR backplane |
| Auth | ASP.NET Core Identity + JWT | built-in | Auth + refresh tokens |
| Authorization | Policy-based RBAC | built-in | Role + permission claims |
| Logging | Serilog | 3+ | Structured logs with correlation IDs |
| API Docs | Scalar / Swashbuckle | latest | OpenAPI |
| Containers | Docker + Compose | — | Dev environment |
| CI/CD | GitHub Actions | — | Build, lint, test, publish |
| Tests | xUnit + Testcontainers | — | Unit + Integration |
| Mapping | Mapster | latest | Entity → DTO mapping |

---

## Architecture Principles

```
Clean Architecture (strict one-way dependencies)

┌──────────────────────────────────────────────────────────────────┐
│  API / Presentation     Controllers · Hubs · Middleware · DTOs   │
│  ─────────────────────────────────────────────────────────────   │
│  Application            Commands · Queries · Handlers · DTOs     │
│                         Behaviors (Validation, Logging, Retry)   │
│  ─────────────────────────────────────────────────────────────   │
│  Domain                 Entities · Value Objects · Enums         │
│                         Domain Events · Repository Interfaces    │
│                         Domain Services · Exceptions             │
│  ─────────────────────────────────────────────────────────────   │
│  Infrastructure         EF Core · Redis · Hangfire · SignalR     │
│                         Repository Implementations · Identity    │
└──────────────────────────────────────────────────────────────────┘

Dependency rule: outer → inner only. Domain imports nothing external.
```

- **Clean Architecture** — domain has zero imports from EF Core, ASP.NET, Redis, or Hangfire
- **CQRS via MediatR** — every operation is a Command or Query; handlers are the only business logic outside the domain
- **SOLID throughout** — one responsibility per class, open/closed via behaviors, interfaces at domain layer
- **Repository Pattern** — domain defines interfaces; infrastructure implements them
- **Domain Events** — state changes publish events via MediatR notifications; side effects (notifications, SLA, audit) subscribe independently
- **Pipeline Behaviors** — validation, logging, retry, and transaction are cross-cutting concerns handled as MediatR behaviors, never inside handlers
- **Modular-first** — every file has one job; no god services; no generic "helpers" or "utils"
- **File limit** — target ~200 lines per file; hard limit ~400 lines; split anything larger
- **No overengineering** — abstractions exist when there is a real need, not in anticipation

---

## Bounded Contexts

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              G O V F L O W                              │
│                                                                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────────────┐ │
│  │   IDENTITY      │  │  ORGANIZATION   │  │        PROCESS          │ │
│  │                 │  │                 │  │                         │ │
│  │  Users          │  │  Tenants        │  │  ProcessTypes           │ │
│  │  Roles          │  │  Departments    │  │  WorkflowSteps          │ │
│  │  Permissions    │  │  Members        │  │  ProcessInstances       │ │
│  │  JWT Auth       │  │  Settings       │  │  StepTransitions        │ │
│  │  Refresh tokens │  │                 │  │  Documents              │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────────────┘ │
│                                                                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────────────┐ │
│  │      SLA        │  │  NOTIFICATION   │  │         AUDIT           │ │
│  │                 │  │                 │  │                         │ │
│  │  SLA Policies   │  │  Notifications  │  │  AuditLogs              │ │
│  │  SLA Tracking   │  │  Templates      │  │  Event history          │ │
│  │  Escalations    │  │  SignalR push   │  │  State transitions      │ │
│  │  Hangfire jobs  │  │  Digest emails  │  │  Actor + timestamp      │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Domain Model

### Identity Context

```
User
  Id (Guid)
  Name
  Email (unique)
  PasswordHash
  OrganizationId (FK)
  DepartmentId (FK, nullable)
  Roles: List<UserRole>
  IsActive
  CreatedAt / UpdatedAt

Role
  Id (Guid)
  Name
  OrganizationId (scoped per tenant)
  Permissions: List<RolePermission>

Permission
  Id (Guid)
  Code (string — e.g. "process:create", "process:approve")
  Description

RefreshToken
  Id (Guid)
  Token (string)
  UserId (FK)
  ExpiresAt
  RevokedAt (nullable)
  IsActive (computed)
```

### Organization Context

```
Organization (Tenant)
  Id (Guid)
  Name
  Slug (unique, URL-safe)
  Settings: OrganizationSettings (value object)
  IsActive
  CreatedAt

Department
  Id (Guid)
  Name
  OrganizationId (FK)
  ParentDepartmentId (FK, nullable — tree structure)
  ManagerUserId (FK, nullable)
  IsActive
```

### Process Context

```
ProcessType
  Id (Guid)
  Name
  Description
  OrganizationId (FK)
  WorkflowSteps: List<WorkflowStep> (ordered)
  IsActive
  CreatedAt / UpdatedAt

WorkflowStep
  Id (Guid)
  ProcessTypeId (FK)
  Name
  Description
  Order (int)
  RequiredDocuments: List<string>
  AssignableDepartmentId (FK, nullable)
  SlaHours (nullable — used to create SlaPolicy)

ProcessInstance (the running process)
  Id (Guid)
  ProcessTypeId (FK)
  OrganizationId (FK)
  Title
  Description
  RequesterId (FK → User)
  CurrentStepId (FK → WorkflowStep)
  Status: ProcessStatus enum
  Priority: ProcessPriority enum
  Steps: List<ProcessInstanceStep>
  Documents: List<ProcessDocument>
  Comments: List<ProcessComment>
  OpenedAt
  ClosedAt (nullable)
  DueAt (nullable)

ProcessInstanceStep
  Id (Guid)
  ProcessInstanceId (FK)
  WorkflowStepId (FK)
  AssignedUserId (FK, nullable)
  AssignedDepartmentId (FK, nullable)
  Status: StepStatus enum
  StartedAt
  CompletedAt (nullable)
  Notes (nullable)

ProcessDocument
  Id (Guid)
  ProcessInstanceId (FK)
  ProcessInstanceStepId (FK, nullable)
  UploadedByUserId (FK)
  FileName
  StoredPath
  MimeType
  SizeBytes
  UploadedAt

ProcessComment
  Id (Guid)
  ProcessInstanceId (FK)
  AuthorId (FK → User)
  Content
  IsInternal (bool — visible only to analysts, not submitter)
  CreatedAt

ProcessStatus enum
  Draft | Open | InProgress | OnHold | Resolved | Cancelled | Rejected

StepStatus enum
  Pending | InProgress | Completed | Skipped | Returned

ProcessPriority enum
  Low | Normal | High | Critical
```

### SLA Context

```
SlaPolicy
  Id (Guid)
  ProcessTypeId (FK)
  WorkflowStepId (FK, nullable — null = whole process)
  DurationHours (int)
  EscalationUserId (FK, nullable)
  EscalationDepartmentId (FK, nullable)

SlaTracking
  Id (Guid)
  ProcessInstanceId (FK)
  WorkflowStepId (FK, nullable)
  SlaPolicyId (FK)
  StartsAt
  DueAt
  BreachedAt (nullable)
  ResolvedAt (nullable)
  Status: SlaStatus enum
  HangfireJobId (string — for cancellation)

SlaStatus enum
  Active | Warning | Breached | Resolved
```

### Notification Context

```
Notification
  Id (Guid)
  RecipientId (FK → User)
  Title
  Body
  Type: NotificationType enum
  ResourceId (Guid — the process or step id)
  ResourceType (string)
  IsRead
  CreatedAt

NotificationType enum
  ProcessOpened | StepAssigned | StepCompleted | CommentAdded
  SlaWarning | SlaBreached | ProcessResolved | ProcessCancelled
```

### Audit Context

```
AuditLog
  Id (Guid)
  OrganizationId (FK)
  ActorId (FK → User)
  Action (string — e.g. "process.step.completed")
  ResourceType (string)
  ResourceId (Guid)
  Before (jsonb, nullable)
  After (jsonb, nullable)
  IpAddress (string)
  CorrelationId (string)
  OccurredAt
```

---

## Domain Events

Every state change in the domain publishes a MediatR notification. Side effects (notifications, SLA, audit) are handled by dedicated handlers — never inline in the command handler.

```
ProcessInstanceOpenedEvent       → notify requester, create SLA tracking, audit log
ProcessStepAssignedEvent         → notify assignee, update SLA tracking
ProcessStepCompletedEvent        → advance to next step or resolve, audit log
ProcessStepReturnedEvent         → notify requester, reset step, audit log
ProcessCommentAddedEvent         → notify relevant parties
ProcessInstanceResolvedEvent     → close SLA tracking, notify all, audit log
ProcessInstanceCancelledEvent    → cancel SLA tracking, audit log
SlaWarningTriggeredEvent         → notify assignee + escalation target
SlaBreachedEvent                 → notify manager + escalation target, audit log
```

---

## Application Layer — Commands and Queries

### Identity

```
Commands:
  RegisterUserCommand              → creates user + hashes password
  LoginCommand                     → validates credentials → returns AccessToken + RefreshToken
  RefreshTokenCommand              → validates refresh token → issues new pair
  RevokeTokenCommand               → revokes refresh token
  ChangePasswordCommand
  UpdateProfileCommand
  AssignRoleCommand
  RevokeRoleCommand

Queries:
  GetCurrentUserQuery              → profile of authenticated user
  GetUserByIdQuery
  ListUsersQuery                   → paginated, filterable by role/department
```

### Organization

```
Commands:
  CreateOrganizationCommand
  UpdateOrganizationCommand
  CreateDepartmentCommand
  UpdateDepartmentCommand
  AddMemberToDepartmentCommand
  RemoveMemberFromDepartmentCommand

Queries:
  GetOrganizationQuery
  ListDepartmentsQuery             → tree or flat, filterable
  GetDepartmentQuery
  ListOrganizationMembersQuery
```

### Process

```
Commands:
  CreateProcessTypeCommand         → defines template + steps
  UpdateProcessTypeCommand
  ArchiveProcessTypeCommand
  AddWorkflowStepCommand
  ReorderWorkflowStepsCommand
  RemoveWorkflowStepCommand

  OpenProcessInstanceCommand       → creates running process from type
  AssignProcessStepCommand         → assigns a user/dept to a step
  CompleteProcessStepCommand       → marks step done, advances workflow
  ReturnProcessStepCommand         → sends back to previous step
  AddProcessCommentCommand
  AttachProcessDocumentCommand
  ResolveProcessInstanceCommand
  CancelProcessInstanceCommand
  PutProcessOnHoldCommand

Queries:
  ListProcessTypesQuery            → paginated, by org
  GetProcessTypeQuery              → with steps
  ListProcessInstancesQuery        → paginated, filterable by status/priority/assignee/dept
  GetProcessInstanceQuery          → full detail with steps, documents, comments, timeline
  ListMyProcessesQuery             → processes where current user is requester or assignee
  GetProcessTimelineQuery          → ordered history of all state changes
  GetProcessStatisticsQuery        → counts by status, avg resolution time, SLA compliance
```

### SLA

```
Commands:
  CreateSlaPolicyCommand
  UpdateSlaPolicyCommand
  DeleteSlaPolicyCommand

Queries:
  ListSlaPoliciesQuery
  GetSlaTrackingForProcessQuery

Internal (triggered by domain events, not API):
  StartSlaTrackingCommand
  ResolveSlaTrackingCommand
  TriggerSlaWarningCommand         → called by Hangfire job
  TriggerSlaBreachCommand          → called by Hangfire job
```

### Notification

```
Queries:
  ListMyNotificationsQuery         → paginated, unread first
  GetUnreadNotificationCountQuery

Commands:
  MarkNotificationReadCommand
  MarkAllNotificationsReadCommand
```

### Audit

```
Queries:
  ListAuditLogsQuery               → paginated, filterable by actor/resource/action/date
  GetAuditLogQuery
```

---

## Pipeline Behaviors (MediatR)

Order of execution: Request → ValidationBehavior → LoggingBehavior → TransactionBehavior → Handler → Response

```
ValidationBehavior<TRequest, TResponse>
  — runs all FluentValidation validators for TRequest
  — throws ValidationException if any rule fails
  — ValidationException is caught by middleware → 400 Bad Request

LoggingBehavior<TRequest, TResponse>
  — logs command/query name, user, correlation ID before and after
  — logs duration; warns if > 500ms

TransactionBehavior<TRequest, TResponse>
  — wraps Commands (not Queries) in a DB transaction
  — commits on success, rolls back on exception
  — detects ICommand marker interface vs IQuery

PerformanceBehavior<TRequest, TResponse>
  — logs warning when handler takes > 1000ms
```

---

## Infrastructure Layer

### Database (EF Core)

```
GovFlowDbContext
  DbSet<UserEntity>
  DbSet<RoleEntity>
  DbSet<PermissionEntity>
  DbSet<RefreshTokenEntity>
  DbSet<OrganizationEntity>
  DbSet<DepartmentEntity>
  DbSet<ProcessTypeEntity>
  DbSet<WorkflowStepEntity>
  DbSet<ProcessInstanceEntity>
  DbSet<ProcessInstanceStepEntity>
  DbSet<ProcessDocumentEntity>
  DbSet<ProcessCommentEntity>
  DbSet<SlaPolicyEntity>
  DbSet<SlaTrackingEntity>
  DbSet<NotificationEntity>
  DbSet<AuditLogEntity>

Conventions:
  — all PKs are Guid, generated by NewGuid()
  — soft delete via IsDeleted + global query filter (except AuditLog — never deleted)
  — all timestamps in UTC
  — owned types for value objects (e.g. OrganizationSettings as Owned)
  — ORM models (Entity suffix) are SEPARATE from domain entities
  — each entity config in its own IEntityTypeConfiguration<T> file
```

### Repository Implementations

One file per repository:

```
UserRepository           : IUserRepository
RoleRepository           : IRoleRepository
OrganizationRepository   : IOrganizationRepository
DepartmentRepository     : IDepartmentRepository
ProcessTypeRepository    : IProcessTypeRepository
ProcessInstanceRepository: IProcessInstanceRepository
SlaPolicyRepository      : ISlaPolicyRepository
SlaTrackingRepository    : ISlaTrackingRepository
NotificationRepository   : INotificationRepository
AuditLogRepository       : IAuditLogRepository
```

### Hangfire Jobs

```
SlaWarningJob            — scheduled at (dueAt - 20%) of each SlaTracking
SlaBreachJob             — scheduled at dueAt of each SlaTracking
NotificationDigestJob    — daily digest of unread notifications (recurring, midnight)
```

### SignalR Hub

```
NotificationHub          — authenticated hub; sends to user-specific groups
  Client methods:
    ReceiveNotification(notification)
    ReceiveUnreadCount(count)
  Server-side broadcasting via IHubContext<NotificationHub>
  Backplane: Redis (for horizontal scaling)
```

### File Storage

```
IFileStorageService      — domain interface
LocalFileStorageService  — saves to /uploads/<orgId>/<processId>/ (dev)
  (placeholder for S3/Azure Blob in production)
```

---

## API Layer

### Controllers

```
AuthController           POST /api/v1/auth/register
                         POST /api/v1/auth/login
                         POST /api/v1/auth/refresh
                         POST /api/v1/auth/revoke
                         GET  /api/v1/auth/me

OrganizationsController  POST   /api/v1/organizations
                         GET    /api/v1/organizations/{id}
                         PUT    /api/v1/organizations/{id}
                         GET    /api/v1/organizations/{id}/departments
                         POST   /api/v1/organizations/{id}/departments
                         GET    /api/v1/organizations/{id}/members

ProcessTypesController   GET    /api/v1/process-types
                         POST   /api/v1/process-types
                         GET    /api/v1/process-types/{id}
                         PUT    /api/v1/process-types/{id}
                         DELETE /api/v1/process-types/{id}
                         POST   /api/v1/process-types/{id}/steps
                         PUT    /api/v1/process-types/{id}/steps/reorder
                         DELETE /api/v1/process-types/{id}/steps/{stepId}

ProcessesController      GET    /api/v1/processes               (filterable, paginated)
                         POST   /api/v1/processes               (open new instance)
                         GET    /api/v1/processes/mine          (my open + assigned)
                         GET    /api/v1/processes/{id}          (full detail)
                         GET    /api/v1/processes/{id}/timeline
                         POST   /api/v1/processes/{id}/assign
                         POST   /api/v1/processes/{id}/complete-step
                         POST   /api/v1/processes/{id}/return-step
                         POST   /api/v1/processes/{id}/comments
                         POST   /api/v1/processes/{id}/documents
                         POST   /api/v1/processes/{id}/resolve
                         POST   /api/v1/processes/{id}/cancel
                         POST   /api/v1/processes/{id}/hold

NotificationsController  GET    /api/v1/notifications           (paginated)
                         GET    /api/v1/notifications/unread-count
                         PUT    /api/v1/notifications/{id}/read
                         PUT    /api/v1/notifications/read-all

AuditController          GET    /api/v1/audit                   (admin only, filterable)
                         GET    /api/v1/audit/{id}

UsersController          GET    /api/v1/users                   (paginated, filterable)
                         GET    /api/v1/users/{id}
                         PUT    /api/v1/users/{id}
                         POST   /api/v1/users/{id}/roles
                         DELETE /api/v1/users/{id}/roles/{roleId}

DashboardController      GET    /api/v1/dashboard/stats         (by role)
                         GET    /api/v1/dashboard/sla-overview

HealthController         GET    /health
                         GET    /health/live
                         GET    /health/ready
```

### Middleware

```
CorrelationIdMiddleware          — generates/echoes X-Correlation-ID on every request
TenantResolutionMiddleware       — extracts OrganizationId from JWT claims → sets tenant context
ExceptionHandlingMiddleware      — maps domain exceptions to Problem Details (RFC 7807)
RequestLoggingMiddleware         — logs method, path, status, duration via Serilog
```

### Exception → HTTP Mapping

```
ValidationException              → 400 Bad Request (with field errors)
NotFoundException                → 404 Not Found
ForbiddenException               → 403 Forbidden
ConflictException                → 409 Conflict
DomainException (base)           → 422 Unprocessable Entity
Unhandled Exception              → 500 Internal Server Error
```

---

## Authentication & Authorization

### JWT Strategy

```
Access Token:   15 minutes expiry, signed RS256 or HS256
Refresh Token:  7 days, stored in DB (RefreshToken table), revocable
Claims:         sub (userId), org (organizationId), roles[], email, jti
```

### Policies

```
"RequireOrgMember"       — user belongs to the organization (from tenant context)
"RequireAnalyst"         — has role Analyst or Manager or Admin
"RequireManager"         — has role Manager or Admin
"RequireAdmin"           — has role Admin
"RequirePermission:<code>" — has specific permission code in claims
```

### Permission Codes

```
process:create           process:read             process:assign
process:complete-step    process:return-step      process:resolve
process:cancel           process:comment          process:document-upload
process-type:manage      user:manage              department:manage
audit:read               sla:manage
```

---

## Project Structure

```
govflow/
│
├── src/
│   │
│   ├── GovFlow.Domain/
│   │   ├── Common/
│   │   │   ├── Entity.cs                    # base entity (Id, CreatedAt, UpdatedAt)
│   │   │   ├── AggregateRoot.cs             # raises domain events
│   │   │   ├── ValueObject.cs               # base value object (equality by value)
│   │   │   ├── IDomainEvent.cs              # marker interface (: INotification)
│   │   │   └── Errors/
│   │   │       ├── DomainException.cs
│   │   │       ├── NotFoundException.cs
│   │   │       ├── ForbiddenException.cs
│   │   │       └── ConflictException.cs
│   │   │
│   │   ├── Identity/
│   │   │   ├── Entities/
│   │   │   │   ├── User.cs
│   │   │   │   ├── Role.cs
│   │   │   │   ├── Permission.cs
│   │   │   │   └── RefreshToken.cs
│   │   │   ├── ValueObjects/
│   │   │   │   ├── Email.cs
│   │   │   │   └── PasswordHash.cs
│   │   │   ├── Enums/
│   │   │   │   └── SystemRole.cs            # Admin | Manager | Analyst | Requester
│   │   │   └── Repositories/
│   │   │       ├── IUserRepository.cs
│   │   │       └── IRoleRepository.cs
│   │   │
│   │   ├── Organization/
│   │   │   ├── Entities/
│   │   │   │   ├── Organization.cs
│   │   │   │   └── Department.cs
│   │   │   ├── ValueObjects/
│   │   │   │   ├── OrganizationSlug.cs
│   │   │   │   └── OrganizationSettings.cs
│   │   │   └── Repositories/
│   │   │       ├── IOrganizationRepository.cs
│   │   │       └── IDepartmentRepository.cs
│   │   │
│   │   ├── Process/
│   │   │   ├── Entities/
│   │   │   │   ├── ProcessType.cs
│   │   │   │   ├── WorkflowStep.cs
│   │   │   │   ├── ProcessInstance.cs       # aggregate root
│   │   │   │   ├── ProcessInstanceStep.cs
│   │   │   │   ├── ProcessDocument.cs
│   │   │   │   └── ProcessComment.cs
│   │   │   ├── ValueObjects/
│   │   │   │   └── ProcessNumber.cs         # human-readable ID e.g. "PROC-2026-00042"
│   │   │   ├── Enums/
│   │   │   │   ├── ProcessStatus.cs
│   │   │   │   ├── ProcessPriority.cs
│   │   │   │   └── StepStatus.cs
│   │   │   ├── Events/
│   │   │   │   ├── ProcessInstanceOpenedEvent.cs
│   │   │   │   ├── ProcessStepAssignedEvent.cs
│   │   │   │   ├── ProcessStepCompletedEvent.cs
│   │   │   │   ├── ProcessStepReturnedEvent.cs
│   │   │   │   ├── ProcessCommentAddedEvent.cs
│   │   │   │   ├── ProcessInstanceResolvedEvent.cs
│   │   │   │   └── ProcessInstanceCancelledEvent.cs
│   │   │   └── Repositories/
│   │   │       ├── IProcessTypeRepository.cs
│   │   │       └── IProcessInstanceRepository.cs
│   │   │
│   │   ├── Sla/
│   │   │   ├── Entities/
│   │   │   │   ├── SlaPolicy.cs
│   │   │   │   └── SlaTracking.cs
│   │   │   ├── Enums/
│   │   │   │   └── SlaStatus.cs
│   │   │   ├── Events/
│   │   │   │   ├── SlaWarningTriggeredEvent.cs
│   │   │   │   └── SlaBreachedEvent.cs
│   │   │   └── Repositories/
│   │   │       ├── ISlaPolicyRepository.cs
│   │   │       └── ISlaTrackingRepository.cs
│   │   │
│   │   ├── Notification/
│   │   │   ├── Entities/
│   │   │   │   └── Notification.cs
│   │   │   ├── Enums/
│   │   │   │   └── NotificationType.cs
│   │   │   └── Repositories/
│   │   │       └── INotificationRepository.cs
│   │   │
│   │   └── Audit/
│   │       ├── Entities/
│   │       │   └── AuditLog.cs
│   │       └── Repositories/
│   │           └── IAuditLogRepository.cs
│   │
│   ├── GovFlow.Application/
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── ICurrentUserService.cs   # UserId, OrgId, Roles from HTTP context
│   │   │   │   ├── IFileStorageService.cs
│   │   │   │   ├── IEmailService.cs
│   │   │   │   └── INotificationDispatcher.cs
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   ├── TransactionBehavior.cs
│   │   │   │   └── PerformanceBehavior.cs
│   │   │   ├── Mappings/
│   │   │   │   └── MappingConfig.cs         # Mapster config (one file per context optional)
│   │   │   └── Models/
│   │   │       ├── PagedResult.cs
│   │   │       └── PaginationParams.cs
│   │   │
│   │   ├── Identity/
│   │   │   ├── Commands/
│   │   │   │   ├── RegisterUser/
│   │   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   │   ├── RegisterUserCommandHandler.cs
│   │   │   │   │   └── RegisterUserCommandValidator.cs
│   │   │   │   ├── Login/
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   ├── LoginCommandHandler.cs
│   │   │   │   │   └── LoginCommandValidator.cs
│   │   │   │   ├── RefreshToken/
│   │   │   │   ├── RevokeToken/
│   │   │   │   ├── ChangePassword/
│   │   │   │   └── AssignRole/
│   │   │   ├── Queries/
│   │   │   │   ├── GetCurrentUser/
│   │   │   │   │   ├── GetCurrentUserQuery.cs
│   │   │   │   │   └── GetCurrentUserQueryHandler.cs
│   │   │   │   └── ListUsers/
│   │   │   └── Dtos/
│   │   │       ├── UserDto.cs
│   │   │       └── AuthTokenDto.cs
│   │   │
│   │   ├── Organization/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateOrganization/
│   │   │   │   ├── CreateDepartment/
│   │   │   │   └── AddMemberToDepartment/
│   │   │   ├── Queries/
│   │   │   │   ├── GetOrganization/
│   │   │   │   ├── ListDepartments/
│   │   │   │   └── ListOrganizationMembers/
│   │   │   └── Dtos/
│   │   │       ├── OrganizationDto.cs
│   │   │       └── DepartmentDto.cs
│   │   │
│   │   ├── Process/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateProcessType/
│   │   │   │   ├── OpenProcessInstance/
│   │   │   │   │   ├── OpenProcessInstanceCommand.cs
│   │   │   │   │   ├── OpenProcessInstanceCommandHandler.cs
│   │   │   │   │   └── OpenProcessInstanceCommandValidator.cs
│   │   │   │   ├── AssignProcessStep/
│   │   │   │   ├── CompleteProcessStep/
│   │   │   │   ├── ReturnProcessStep/
│   │   │   │   ├── AddProcessComment/
│   │   │   │   ├── AttachProcessDocument/
│   │   │   │   ├── ResolveProcessInstance/
│   │   │   │   └── CancelProcessInstance/
│   │   │   ├── Queries/
│   │   │   │   ├── GetProcessInstance/
│   │   │   │   ├── ListProcessInstances/
│   │   │   │   ├── ListMyProcesses/
│   │   │   │   ├── GetProcessTimeline/
│   │   │   │   └── GetProcessStatistics/
│   │   │   ├── EventHandlers/
│   │   │   │   ├── ProcessOpenedEventHandler.cs     # → creates SLA, audit
│   │   │   │   ├── StepAssignedEventHandler.cs      # → notification, SLA update
│   │   │   │   ├── StepCompletedEventHandler.cs     # → advance workflow, notification
│   │   │   │   └── ProcessResolvedEventHandler.cs   # → close SLA, final notification
│   │   │   └── Dtos/
│   │   │       ├── ProcessInstanceDto.cs
│   │   │       ├── ProcessInstanceDetailDto.cs
│   │   │       ├── ProcessTypeDto.cs
│   │   │       └── ProcessTimelineEntryDto.cs
│   │   │
│   │   ├── Sla/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateSlaPolicy/
│   │   │   │   ├── StartSlaTracking/
│   │   │   │   └── ResolveSlaTracking/
│   │   │   ├── EventHandlers/
│   │   │   │   ├── SlaWarningEventHandler.cs
│   │   │   │   └── SlaBreachEventHandler.cs
│   │   │   └── Dtos/
│   │   │       └── SlaTrackingDto.cs
│   │   │
│   │   └── Notification/
│   │       ├── Queries/
│   │       │   ├── ListMyNotifications/
│   │       │   └── GetUnreadNotificationCount/
│   │       ├── Commands/
│   │       │   ├── MarkNotificationRead/
│   │       │   └── MarkAllNotificationsRead/
│   │       └── Dtos/
│   │           └── NotificationDto.cs
│   │
│   ├── GovFlow.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── GovFlowDbContext.cs
│   │   │   ├── UnitOfWork.cs
│   │   │   ├── Configurations/              # one IEntityTypeConfiguration<T> per entity
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── OrganizationConfiguration.cs
│   │   │   │   ├── DepartmentConfiguration.cs
│   │   │   │   ├── ProcessTypeConfiguration.cs
│   │   │   │   ├── WorkflowStepConfiguration.cs
│   │   │   │   ├── ProcessInstanceConfiguration.cs
│   │   │   │   ├── ProcessInstanceStepConfiguration.cs
│   │   │   │   ├── ProcessDocumentConfiguration.cs
│   │   │   │   ├── ProcessCommentConfiguration.cs
│   │   │   │   ├── SlaPolicyConfiguration.cs
│   │   │   │   ├── SlaTrackingConfiguration.cs
│   │   │   │   ├── NotificationConfiguration.cs
│   │   │   │   └── AuditLogConfiguration.cs
│   │   │   └── Repositories/
│   │   │       ├── UserRepository.cs
│   │   │       ├── RoleRepository.cs
│   │   │       ├── OrganizationRepository.cs
│   │   │       ├── DepartmentRepository.cs
│   │   │       ├── ProcessTypeRepository.cs
│   │   │       ├── ProcessInstanceRepository.cs
│   │   │       ├── SlaPolicyRepository.cs
│   │   │       ├── SlaTrackingRepository.cs
│   │   │       ├── NotificationRepository.cs
│   │   │       └── AuditLogRepository.cs
│   │   │
│   │   ├── Identity/
│   │   │   ├── JwtTokenService.cs           # generates + validates JWT
│   │   │   ├── PasswordService.cs           # BCrypt wrapper
│   │   │   └── CurrentUserService.cs        # reads IHttpContextAccessor → ICurrentUserService
│   │   │
│   │   ├── SignalR/
│   │   │   ├── NotificationHub.cs
│   │   │   └── SignalRNotificationDispatcher.cs  # implements INotificationDispatcher
│   │   │
│   │   ├── Hangfire/
│   │   │   ├── HangfireJobService.cs        # schedules SlaWarningJob + SlaBreachJob
│   │   │   ├── SlaWarningJob.cs             # publishes SlaWarningTriggeredEvent
│   │   │   ├── SlaBreachJob.cs              # publishes SlaBreachedEvent
│   │   │   └── NotificationDigestJob.cs     # daily digest email
│   │   │
│   │   ├── Storage/
│   │   │   └── LocalFileStorageService.cs   # implements IFileStorageService
│   │   │
│   │   ├── Email/
│   │   │   └── SmtpEmailService.cs          # implements IEmailService
│   │   │
│   │   └── DependencyInjection.cs           # InfrastructureServiceCollection extension
│   │
│   └── GovFlow.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── OrganizationsController.cs
│       │   ├── ProcessTypesController.cs
│       │   ├── ProcessesController.cs
│       │   ├── UsersController.cs
│       │   ├── NotificationsController.cs
│       │   ├── AuditController.cs
│       │   ├── DashboardController.cs
│       │   └── HealthController.cs
│       ├── Hubs/
│       │   └── NotificationHub.cs           # maps to /hubs/notifications
│       ├── Middleware/
│       │   ├── CorrelationIdMiddleware.cs
│       │   ├── TenantResolutionMiddleware.cs
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       ├── Filters/
│       │   └── ValidationFilter.cs
│       ├── Extensions/
│       │   ├── ServiceCollectionExtensions.cs
│       │   └── WebApplicationExtensions.cs
│       ├── Program.cs
│       └── appsettings.json / appsettings.Development.json
│
├── tests/
│   ├── GovFlow.Domain.Tests/
│   │   ├── Process/
│   │   │   ├── ProcessInstanceTests.cs      # domain logic, state transitions
│   │   │   └── WorkflowStepTests.cs
│   │   └── Sla/
│   │       └── SlaPolicyTests.cs
│   │
│   ├── GovFlow.Application.Tests/
│   │   ├── Process/
│   │   │   ├── OpenProcessInstanceCommandHandlerTests.cs
│   │   │   ├── CompleteProcessStepCommandHandlerTests.cs
│   │   │   └── ListProcessInstancesQueryHandlerTests.cs
│   │   ├── Identity/
│   │   │   ├── LoginCommandHandlerTests.cs
│   │   │   └── RegisterUserCommandHandlerTests.cs
│   │   └── Behaviors/
│   │       └── ValidationBehaviorTests.cs
│   │
│   └── GovFlow.Integration.Tests/
│       ├── Fixtures/
│       │   └── IntegrationTestWebAppFactory.cs  # Testcontainers: Postgres + Redis
│       ├── Process/
│       │   ├── ProcessApiTests.cs
│       │   └── ProcessWorkflowTests.cs      # full flow: open → assign → complete → resolve
│       └── Identity/
│           └── AuthApiTests.cs
│
├── migrations/                              # EF Core migrations folder
├── docker/
│   └── Dockerfile                           # multi-stage: build → publish → runtime
├── docker-compose.yml                       # postgres + redis + hangfire dashboard + app
├── docker-compose.test.yml
├── .github/
│   └── workflows/
│       └── ci.yml                           # restore → build → test → publish
├── .editorconfig
├── GovFlow.sln
└── README.md
```

---

## Event Flow (Process Lifecycle)

```
[POST /api/v1/processes]
  → OpenProcessInstanceCommand
      → OpenProcessInstanceCommandHandler
          → ProcessInstance.Open() — raises ProcessInstanceOpenedEvent
          → saves to DB via IProcessInstanceRepository
          → publishes domain events via MediatR
              → ProcessOpenedEventHandler
                  → StartSlaTrackingCommand  → creates SlaTracking, schedules Hangfire jobs
                  → AuditLogRepository.Add() → records open action
                  → INotificationDispatcher  → creates Notification + pushes via SignalR

[POST /api/v1/processes/{id}/assign]
  → AssignProcessStepCommand
      → step.Assign(userId) → raises ProcessStepAssignedEvent
          → StepAssignedEventHandler
              → notification to assignee
              → SlaTracking updated for this step

[POST /api/v1/processes/{id}/complete-step]
  → CompleteProcessStepCommand
      → step.Complete() → raises ProcessStepCompletedEvent
          → StepCompletedEventHandler
              → if more steps: advance to next (auto-assign if configured)
              → if last step: resolve process → ProcessInstanceResolvedEvent
              → cancel old SLA Hangfire job, start new one for next step

[Hangfire SlaBreachJob fires at dueAt]
  → publishes SlaBreachedEvent
      → SlaBreachEventHandler
          → SlaTracking.MarkBreached()
          → notification to assignee + manager
          → audit log
          → SignalR push
```

---

## Real-time Notification Flow

```
Domain Event fired
  └─▶ EventHandler calls INotificationDispatcher.SendAsync(userId, notification)
          └─▶ SignalRNotificationDispatcher
                  ├─▶ NotificationRepository.Add()       (persist)
                  └─▶ IHubContext<NotificationHub>
                          └─▶ Clients.User(userId).SendAsync("ReceiveNotification", dto)
                                  └─▶ connected browser clients receive in real-time

Redis backplane enables multi-instance SignalR routing
```

---

## Configuration

```json
// appsettings.json (relevant keys)
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=govflow;Username=govflow;Password=...",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "govflow",
    "Audience": "govflow-clients",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "HangfireSettings": {
    "DashboardEnabled": true,
    "DashboardPath": "/hangfire",
    "WorkerCount": 5
  },
  "FileStorage": {
    "BasePath": "/uploads"
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

---

## Quick Start

```bash
# Prerequisites: .NET 9 SDK, Docker

git clone https://github.com/renanbambam/govflow
cd govflow

# Start infrastructure
docker compose up postgres redis -d

# Run migrations
dotnet ef database update --project src/GovFlow.Infrastructure --startup-project src/GovFlow.API

# Run
dotnet run --project src/GovFlow.API

# API Docs
open http://localhost:5000/scalar

# Hangfire Dashboard (dev only)
open http://localhost:5000/hangfire
```

---

## CI/CD (GitHub Actions)

```yaml
# .github/workflows/ci.yml
jobs:
  build-and-test:
    - dotnet restore
    - dotnet build --no-restore
    - dotnet test --no-build (unit + application tests, no Docker)
  
  integration-test:
    services: postgres, redis   # GitHub Actions service containers
    - dotnet test GovFlow.Integration.Tests
  
  publish:
    - docker build
    - docker push ghcr.io/renanbambam/govflow:$SHA
```

---

## Roadmap

### Phase 1 — MVP
| Feature | Status |
|---|---|
| Clean Architecture with 6 bounded contexts | 🔜 |
| CQRS via MediatR + FluentValidation pipeline | 🔜 |
| JWT auth + refresh tokens | 🔜 |
| Policy-based RBAC with permission codes | 🔜 |
| Multi-tenant (organization scoping) | 🔜 |
| Process types with configurable workflow steps | 🔜 |
| Process instances with full state machine | 🔜 |
| Document attachments | 🔜 |
| Process comments (public + internal) | 🔜 |
| SLA tracking with Hangfire timers | 🔜 |
| Real-time notifications via SignalR | 🔜 |
| Full audit trail on all state changes | 🔜 |
| Dashboard stats by role | 🔜 |
| EF Core + PostgreSQL + Migrations | 🔜 |
| Domain + Application + Integration tests | 🔜 |
| Docker + Compose | 🔜 |
| GitHub Actions CI | 🔜 |

### Phase 2
| Feature | Status |
|---|---|
| Email digest (Hangfire recurring job) | ⏳ |
| Process number generator (PROC-2026-XXXXX) | ⏳ |
| File storage swap to S3/Azure Blob | ⏳ |
| Prometheus metrics endpoint | ⏳ |
| OpenTelemetry tracing | ⏳ |

---

## Architecture Decisions

| Decision | Choice | Reason |
|---|---|---|
| ORM | EF Core (not Dapper) | Code-first migrations; type-safe queries; idiomatic .NET |
| CQRS library | MediatR | Canonical .NET pattern; clean handler isolation |
| Validation | FluentValidation in MediatR behavior | Decoupled; reusable; testable outside HTTP |
| Real-time | SignalR (not polling) | Native .NET; websocket with fallback; easy auth integration |
| Background jobs | Hangfire (not Quartz) | Dashboard UI; persistent jobs; SLA cancellation by ID |
| Mapping | Mapster (not AutoMapper) | Faster; source-gen; no reflection overhead |
| Auth | JWT (not sessions) | API-first; stateless; mobile-compatible |
| Multi-tenancy | Claim-based (OrgId in token) + query filters | Simpler than schema-per-tenant; enough for this scale |
| Domain events | MediatR INotification (in-process) | Simple; consistent; testable; Redis pub/sub when scaling |

---
