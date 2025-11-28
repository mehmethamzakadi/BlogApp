using BlogApp.Domain.Exceptions;
using BlogApp.Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

[TestFixture]
public class EmailTests
{
    [Test]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        var validEmail = "test@example.com";
        var email = Email.Create(validEmail);
        Assert.That(email.Value, Is.EqualTo(validEmail));
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void Create_WithEmptyEmail_ShouldThrowDomainValidationException(string invalidEmail)
    {
        Assert.Throws<DomainValidationException>(() => Email.Create(invalidEmail));
    }

    [Test]
    public void Create_WithNullEmail_ShouldThrowDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() => Email.Create(null!));
    }

    [Test]
    [TestCase("invalid")]
    [TestCase("invalid@")]
    [TestCase("@example.com")]
    public void Create_WithInvalidFormat_ShouldThrowDomainValidationException(string invalidEmail)
    {
        Assert.Throws<DomainValidationException>(() => Email.Create(invalidEmail));
    }

    [Test]
    public void Create_WithTooLongEmail_ShouldThrowDomainValidationException()
    {
        var longEmail = new string('a', 250) + "@test.com"; // 259 characters
        Assert.Throws<DomainValidationException>(() => Email.Create(longEmail));
    }

    [Test]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");
        Assert.That(email1, Is.EqualTo(email2));
    }

    [Test]
    public void Equals_WithDifferentCaseEmail_ShouldReturnTrue()
    {
        var email1 = Email.Create("Test@Example.com");
        var email2 = Email.Create("test@example.com");
        Assert.That(email1, Is.EqualTo(email2));
    }

    [Test]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");
        Assert.That(email1, Is.Not.EqualTo(email2));
    }

    [Test]
    public void ToString_ShouldReturnValue()
    {
        var email = Email.Create("test@example.com");
        Assert.That(email.ToString(), Is.EqualTo("test@example.com"));
    }

    [Test]
    public void ImplicitConversion_ShouldReturnValue()
    {
        var email = Email.Create("test@example.com");
        string emailString = email;
        Assert.That(emailString, Is.EqualTo("test@example.com"));
    }
}
