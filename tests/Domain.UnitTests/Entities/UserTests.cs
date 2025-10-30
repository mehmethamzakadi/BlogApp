using BlogApp.Domain.Entities;

namespace Domain.UnitTests.Entities;

[TestFixture]
public class UserTests
{
    [Test]
    public void Create_WithValidData_ShouldSucceed()
    {
        var userName = "testuser";
        var email = "test@example.com";
        var passwordHash = "hashedpassword";

        var user = User.Create(userName, email, passwordHash);

        Assert.That(user.UserName, Is.EqualTo(userName));
        Assert.That(user.Email, Is.EqualTo(email));
        Assert.That(user.PasswordHash, Is.EqualTo(passwordHash));
        Assert.That(user.IsDeleted, Is.False);
    }

    [Test]
    public void Update_WithValidData_ShouldUpdateFields()
    {
        var user = User.Create("olduser", "old@example.com", "hash");
        var newUserName = "newuser";
        var newEmail = "new@example.com";

        user.Update(newUserName, newEmail);

        Assert.That(user.UserName, Is.EqualTo(newUserName));
        Assert.That(user.Email, Is.EqualTo(newEmail));
    }

    [Test]
    public void Delete_ShouldAddDomainEvent()
    {
        var user = User.Create("testuser", "test@example.com", "hash");
        user.ClearDomainEvents();

        user.Delete();

        Assert.That(user.DomainEvents.Count, Is.EqualTo(1));
    }

    [Test]
    public void NormalizedFields_ShouldBeUpperCase()
    {
        var user = User.Create("TestUser", "Test@Example.com", "hash");

        Assert.That(user.NormalizedUserName, Is.EqualTo("TESTUSER"));
        Assert.That(user.NormalizedEmail, Is.EqualTo("TEST@EXAMPLE.COM"));
    }
}
