namespace GovFlow.Domain.Process.Enums;

public enum ProcessEventType
{
    ProcessOpened = 1,
    StepStarted = 2,
    StepCompleted = 3,
    StepReturned = 4,
    ProcessResolved = 5,
    ProcessCancelled = 6,
    ProcessOnHold = 7,
    ProcessResumed = 8,
    SlaBreached = 9
}
