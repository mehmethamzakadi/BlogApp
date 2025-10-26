namespace BlogApp.Application.Common;

/// <summary>
/// Base response interface for entities that track creation time.
/// Following SOLID principles - Interface Segregation Principle (ISP)
/// </summary>
public interface ITimestampedResponse
{
    DateTime CreatedDate { get; }
}

/// <summary>
/// Base response interface for entities with an ID.
/// Following SOLID principles - Interface Segregation Principle (ISP)
/// </summary>
public interface IIdentifiableResponse
{
    Guid Id { get; }
}

/// <summary>
/// Base record for entity responses that include ID and timestamp information.
/// Following SOLID principles:
/// - Single Responsibility Principle (SRP): Only responsible for providing common response properties
/// - Open/Closed Principle (OCP): Open for extension via inheritance, closed for modification
/// </summary>
public abstract record BaseEntityResponse : IIdentifiableResponse, ITimestampedResponse
{
    public Guid Id { get; init; }
    public DateTime CreatedDate { get; init; }
}
