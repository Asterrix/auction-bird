using Application.Features.Authentication.Commands.Mapper;
using Application.Features.Authentication.Commands.SignUp;
using FluentValidation.Results;
using JetBrains.Annotations;
using Xunit;

namespace Application.UnitTests.Features.Authentication.Commands.SignUp;

[TestSubject(typeof(SignUpCommandValidator))]
public class SignUpCommandValidatorTest
{
    private readonly SignUpCommandValidator _validator = new();

    private readonly SignUpDto _testCase = new(
        "ClientId",
        "John",
        "Doe",
        "john@doe.com",
        "$Password123",
        true,
        true
    );

    [Fact(DisplayName = "First Name is required.")]
    public void FirstNameIsRequired()
    {
        SignUpDto dto = _testCase with { FirstName = "" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "First Name is required.");
    }

    [Fact(DisplayName = "First Name must be at least 3 characters long.")]
    public void FirstNameMustBeAtLeast3CharactersLong()
    {
        SignUpDto dto = _testCase with { FirstName = "ab" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "First Name must be at least 3 characters long.");
    }

    [Fact(DisplayName = "First Name must not exceed 20 characters.")]
    public void FirstNameMustNotExceed20Characters()
    {
        const byte charCount = 21;
        SignUpDto dto = _testCase with { FirstName = new string('x', charCount) };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "First Name must not exceed 20 characters.");
    }

    [Fact(DisplayName = "First Name must consist of only alphabetical characters.")]
    public void FirstNameMustConsistOfOnlyAlphabeticalCharacters()
    {
        SignUpDto dto = _testCase with { FirstName = "123456" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == "First Name must consist of only alphabetical characters.");
    }

    [Fact(DisplayName = "Last Name is required.")]
    public void LastNameIsRequired()
    {
        SignUpDto dto = _testCase with { LastName = "" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Last Name is required.");
    }

    [Fact(DisplayName = "Last Name must be at least 3 characters long.")]
    public void LastNameMustBeAtLeast3CharactersLong()
    {
        SignUpDto dto = _testCase with { LastName = "ab" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Last Name must be at least 3 characters long.");
    }

    [Fact(DisplayName = "Last Name must not exceed 32 characters.")]
    public void LastNameMustNotExceed32Characters()
    {
        const byte charCount = 33;
        SignUpDto dto = _testCase with { LastName = new string('x', charCount) };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Last Name must not exceed 32 characters.");
    }

    [Fact(DisplayName = "Last Name must consist of only alphabetical characters.")]
    public void LastNameMustConsistOfOnlyAlphabeticalCharacters()
    {
        SignUpDto dto = _testCase with { LastName = "123456" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == "Last Name must consist of only alphabetical characters.");
    }

    [Fact(DisplayName = "Email is required.")]
    public void EmailIsRequired()
    {
        SignUpDto dto = _testCase with { Email = "" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Email is required.");
    }

    [Fact(DisplayName = "Email is not in the correct format.")]
    public void EmailIsNotInTheCorrectFormat()
    {
        SignUpDto dto = _testCase with { Email = "abc" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Email is not in the correct format.");
    }

    [Fact(DisplayName = "Email must be at least 7 characters long.")]
    public void EmailMustBeAtLeast7CharactersLong()
    {
        SignUpDto dto = _testCase with { Email = "a@b.em" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Email must be at least 7 characters long.");
    }

    [Fact(DisplayName = "Email must not exceed 64 characters.")]
    public void EmailMustNotExceed64Characters()
    {
        const byte charCount = 65;
        SignUpDto dto = _testCase with { Email = new string('x', charCount) };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Email must not exceed 64 characters.");
    }

    [Fact(DisplayName = "Password is required.")]
    public void PasswordIsRequired()
    {
        SignUpDto dto = _testCase with { Password = "" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Password is required.");
    }

    [Fact(DisplayName = "Password must be at least 8 characters long.")]
    public void PasswordMustBeAtLeast8CharactersLong()
    {
        SignUpDto dto = _testCase with { Password = "12345$X" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Password must be at least 8 characters long.");
    }

    [Fact(DisplayName = "Password must not exceed 32 characters.")]
    public void PasswordMustNotExceed32Characters()
    {
        const byte charCount = 33;
        SignUpDto dto = _testCase with { Password = new string('x', charCount) };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Password must not exceed 32 characters.");
    }

    [Fact(DisplayName = "Password must contain at least one uppercase letter.")]
    public void PasswordMustContainAtLeastOneUppercaseLetter()
    {
        SignUpDto dto = _testCase with { Password = "password123" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Password must contain at least one lowercase letter.")]
    public void PasswordMustContainAtLeastOneLowercaseLetter()
    {
        SignUpDto dto = _testCase with { Password = "PASSWORD123" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Password must contain at least one digit.")]
    public void PasswordMustContainAtLeastOneDigit()
    {
        SignUpDto dto = _testCase with { Password = "Password" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Password must contain at least one special character.")]
    public void PasswordMustContainAtLeastOneSpecialCharacter()
    {
        SignUpDto dto = _testCase with { Password = "Password123" };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
    }
    
    [Fact(DisplayName = "User must accept the terms and conditions to sign up.")]
    public void UserMustAcceptTheTermsAndConditionsToSignUp()
    {
        SignUpDto dto = _testCase with { TermsAndConditionsAccepted = false };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "You must accept the terms and conditions to sign up.");
    }

    [Fact(DisplayName = "User must be at least 18 years old.")]
    public void UserMustBeAtLeast18YearsOld()
    {
        SignUpDto dto = _testCase with { IsOver18 = false };

        ValidationResult result = _validator.Validate(new SignUpCommand(dto));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "You must be at least 18 years old to sign up.");
    }

    [Fact(DisplayName = "All properties are valid.")]
    public void AllPropertiesAreValid()
    {
        ValidationResult result = _validator.Validate(new SignUpCommand(_testCase));

        Assert.True(result.IsValid);
    }
}