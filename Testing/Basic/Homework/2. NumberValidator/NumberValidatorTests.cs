using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, -1)]
    [TestCase(1, 1)]
    [TestCase(1, 2)]
    [TestCase(2, 2)]
    public void Validator_with_bad_precision_or_scale_throws_error(int precision, int scale)
    {
        Func<NumberValidator> create = () => new NumberValidator(precision, scale);

        create
            .Should()
            .Throw<ArgumentException>();
    }

    [TestCase(1, 0, true)]
    [TestCase(1, 0, false)]
    [TestCase(2, 1, true)]
    [TestCase(17, 16, false)]
    public void Validator_with_good_precision_and_scale_is_created(int precision, int scale, bool onlyPositive)
    {
        Func<NumberValidator> create = () => new NumberValidator(precision, scale, onlyPositive);

        create
            .Should()
            .NotThrow();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("abc")]
    [TestCase("a.sd")]
    [TestCase("12a")]
    [TestCase("1e3")]
    [TestCase("1.")]
    [TestCase(".5")]
    [TestCase("1.2.3")]
    [TestCase("1 2")]
    [TestCase("+")]
    [TestCase("--1")]
    [TestCase("+-1")]
    public void Text_that_is_not_a_number_is_invalid(string? value)
    {
        NumberValidator validator = new(10, 5);

        validator
            .IsValidNumber(value!)
            .Should()
            .BeFalse();
    }

    [TestCase(1, 0, false, "0")]
    [TestCase(17, 2, true, "0")]
    [TestCase(17, 2, true, "0.0")]
    [TestCase(3, 0, false, "123")]
    [TestCase(4, 2, false, "00.00")]
    [TestCase(4, 2, true, "+1.23")]
    [TestCase(4, 2, false, "-1.23")]
    [TestCase(3, 1, false, "12.3")]
    public void Number_that_fits_precision_and_scale_is_valid(int precision, int scale, bool onlyPositive, string value)
    {
        NumberValidator validator = new(precision, scale, onlyPositive);

        validator
            .IsValidNumber(value)
            .Should()
            .BeTrue();
    }

    [TestCase("1.5")]
    [TestCase("1,5")]
    public void Dot_and_comma_are_both_valid_separators(string value)
    {
        NumberValidator validator = new (2, 1);

        validator
            .IsValidNumber(value)
            .Should()
            .BeTrue();
    }

    [TestCase(3, 0, "1234")]
    [TestCase(3, 2, "00.00")]
    [TestCase(3, 2, "12.34")]
    public void Number_with_too_many_digits_is_invalid(int precision, int scale, string value)
    {
        NumberValidator validator = new (precision, scale);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase(3, 2, "+1.23")]
    [TestCase(3, 2, "-1.23")]
    [TestCase(3, 2, "+0.00")]
    [TestCase(1, 0, "-1")]
    public void Sign_counts_as_a_digit(int precision, int scale, string value)
    {
        NumberValidator validator = new (precision, scale);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase(17, 2, "0.000")]
    [TestCase(17, 2, "1,234")]
    [TestCase(17, 0, "1.0")]
    public void Number_with_too_many_digits_after_point_is_invalid(int precision, int scale, string value)
    {
        NumberValidator validator = new (precision, scale);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase("-1")]
    [TestCase("-0")]
    [TestCase("-1.23")]
    public void Negative_number_is_invalid_if_only_positive_is_allowed(string value)
    {
        NumberValidator validator = new (17, 2, onlyPositive: true);

        validator
            .IsValidNumber(value)
            .Should()
            .BeFalse();
    }

    [TestCase("-1")]
    [TestCase("-0")]
    [TestCase("-1.23")]
    public void Negative_number_is_valid_if_negative_is_allowed(string value)
    {
        NumberValidator validator = new (17, 2, onlyPositive: false);

        validator
            .IsValidNumber(value)
            .Should()
            .BeTrue();
    }
}
