using MediatR;

namespace BlogApp.Domain.Common;

/// <summary>
/// Domain event'ler için marker interface
/// Domain event'ler, domain uzmanlarının önemsediği domain'de gerçekleşen bir şeyi temsil eder
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
