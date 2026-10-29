using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1)]
    [TestCase(0)]
    public void Constructor_ShouldThrowArgumentException_WhenPrecisionIsNotPositive_Test(int precision)
    {
        Func<NumberValidator> createAction = () => new NumberValidator(precision);

        createAction
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("precision must be a positive number");
    }

    [TestCase(1, -1)]
    [TestCase(1, 1)]
    [TestCase(1, 2)]
    public void Constructor_ShouldThrowArgumentException_WhenScaleIsOutOfRange_Test(int precision, int scale)
    {
        Func<NumberValidator> createAction = () => new NumberValidator(precision, scale);

        createAction
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("scale must be a non-negative number less than precision");
    }

    [TestCase(1, 0)]
    [TestCase(2, 1)]
    public void Constructor_ShouldNotThrow_WhenPrecisionAndScaleAreValid_Test(int precision, int scale)
    {
        Func<NumberValidator> createAction = () => new NumberValidator(precision, scale);

        createAction
            .Should()
            .NotThrow();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("a.sd")]
    [TestCase(".5")]
    [TestCase("1.")]
    [TestCase("1.2.3")]
    [TestCase("1;2")]
    [TestCase("+")]
    [TestCase("--1")]
    [TestCase("1-")]
    public void Value_ShouldBeInvalid_WhenItIsNotNumber_Test(string? value)
    {
        NumberValidator validator = new(10, 5);

        validator
            .IsValidNumber(value!)
            .Should()
            .BeFalse();
    }

    [TestCase(1, 0, "0")]
    [TestCase(3, 2, "1.23")]
    [TestCase(4, 2, "00.00")]
    [TestCase(4, 2, "+1.23")]
    [TestCase(4, 2, "-1.23")]
    public void Number_ShouldBeValid_WhenItFitsPrecisionAndScale_Test(int precision, int scale, string value)
    {
        NumberValidator validator = new(precision, scale);

        validator
            .IsValidNumber(value)
            .Should()
            .BeTrue();
    }

    [TestCase("1.5")]
    [TestCase("1,5")]
    public void Number_ShouldBeValid_WhenSeparatorIsDotOrComma_Test(string value)
    {
        NumberValidator validator = new(2, 1);

        validator
            .IsValidNumber(value)
            .Should()
            .BeTrue();
    }

    [TestCase(1, 0, "12")]
    [TestCase(3, 2, "00.00")]
    public void Number_ShouldBeInvalid_WhenDigitsExceedPrecision_Test(int precision, int scale, string value)
    {
        NumberValidator validator = new(precision, scale);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase("+1.23")]
    [TestCase("-1.23")]
    public void Number_ShouldBeInvalid_WhenSignAndDigitsExceedPrecision_Test(string value)
    {
        NumberValidator validator = new(3, 2);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase(3, 1, "0.00")]
    [TestCase(2, 0, "1.0")]
    public void Number_ShouldBeInvalid_WhenFractionDigitsExceedScale_Test(int precision, int scale, string value)
    {
        NumberValidator validator = new(precision, scale);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase("-1")]
    [TestCase("-0")]
    public void NegativeNumber_ShouldBeInvalid_WhenOnlyPositiveAllowed_Test(string value)
    {
        NumberValidator validator = new(2, 0, true);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase("0")]
    [TestCase("+1")]
    public void NonNegativeNumber_ShouldBeValid_WhenOnlyPositiveAllowed_Test(string value)
    {
        NumberValidator validator = new(2, 0, true);

        validator
            .IsValidNumber(value)
            .Should()
            .BeTrue();
    }
}
