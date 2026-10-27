using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;

public class ObjectComparison
{
    // ReSharper disable once InconsistentNaming
    private static readonly Person _VasiliTheThirdTsar = 
        new ("Vasili III of Russia", 28, 170, 60, null!);   
    
    // ReSharper disable once InconsistentNaming
    private static readonly Person _IvanTheTerribleTsar = 
        new ("Ivan IV The Terrible", 54, 170, 70, _VasiliTheThirdTsar);
    
    [Test]
    [Description("Проверка текущего царя")]
    public void Current_tsar_is_equal_to_expected() //Тесты проименовал по совету Владимира Хорикова
                                                    //(значимый деятель C#-мира, обладает Microsoft MVP
                                                    //(т.е. его большой вклад признан Microsoft))
                                                    //из книги "Принципы юнит-тестирования".
                                                    //Преподаватель, когда я спросил его о таком стиле именования,
                                                    //ответил, что у них "пишут названия тестов на русском".
                                                    //Хотел бы получить ваш комментарий. 
    {
        Person actualTsar = TsarRegistry.GetCurrentTsar();
        Person expectedTsar = _IvanTheTerribleTsar;

        actualTsar.Should()
            .BeEquivalentTo(expectedTsar, config => config
            .Excluding((IMemberInfo member) => member.Name == nameof(Person.Id)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void Current_tsar_is_equal_to_expected_with_custom_equality()
    {
        Person actualTsar = TsarRegistry.GetCurrentTsar();
        Person expectedTsar = _IvanTheTerribleTsar;

        // Какие недостатки у такого подхода?
        //Это решение нарушает OCP. Если в Person появится новое поле, придётся менять AreEqual.
        //Моё решение обеспечивает расширяемость.
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
