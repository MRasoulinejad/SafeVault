using NUnit.Framework;
using SafeVault.Services;

namespace SafeVault.Tests;

[TestFixture]
public class SecurityTests
{
    private UserService _service;

    [SetUp]
    public void Setup()
    {
        _service = new UserService("Data Source=:memory:");
    }

    [Test]
    public void ShouldSanitizeSqlInjectionAttempt()
    {
        var input = "Robert'); DROP TABLE Users;--";
        var result = _service.SanitizeInput(input);
        Assert.That(result.ToLower().Contains("drop table"), Is.False);
    }

    [Test]
    public void ShouldRemoveScriptTags()
    {
        var input = "<script>alert('hacked!')</script>";
        var result = _service.SanitizeInput(input);
        Assert.That(result.Contains("<script>"), Is.False);
        Assert.That(result.Contains("alert"), Is.False); // bonus
    }

    [Test]
    public void ShouldPreventUserInsertionWithMaliciousInput()
    {
        var success = _service.AddUser("attacker<script>", "bad@x.com", "fakehash", "admin");
        var stored = _service.GetUserByUsername("attackerscript");
        Assert.That(success, Is.True);
        Assert.That(stored.Username.Contains("<script>"), Is.False);
    }
}
