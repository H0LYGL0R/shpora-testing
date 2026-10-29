using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;

public class ObjectComparison
{
    private static readonly Person VasiliTheThirdTsar =
        new("Vasili III of Russia", 28, 170, 60, null!);

    private static readonly Person IvanTheTerribleTsar =
        new("Ivan IV The Terrible", 54, 170, 70, VasiliTheThirdTsar);

    [Test]
    [Description("Проверка текущего царя")]
    public void CurrentTsar_ShouldBeEqualTo_TsarFromRegistry_Test()
    {
        Person actualTsar = TsarRegistry.GetCurrentTsar();
        Person expectedTsar = IvanTheTerribleTsar;

        actualTsar
            .Should()
            .BeEquivalentTo(expectedTsar, config => config
                .Excluding((IMemberInfo member) => member.Name == nameof(Person.Id)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CurrentTsar_ShouldBeEqualTo_TsarFromRegistry_WithCustomEquality_Test()
    {
        Person actualTsar = TsarRegistry.GetCurrentTsar();
        Person expectedTsar = IvanTheTerribleTsar;

        // Какие недостатки у такого подхода?
        // Это решение нарушает OCP. Если в Person появится новое поле, придётся менять AreEqual.
        // Моё решение обеспечивает расширяемость.
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
