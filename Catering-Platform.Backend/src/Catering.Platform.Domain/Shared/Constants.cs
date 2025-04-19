namespace Catering.Platform.Domain.Shared;

public class Constants
{
    public static readonly int MAX_LOW_TEXT_LENGTH = 256;
    public static readonly int MAX_HIGH_TEXT_LENGTH = 2000;

    // лучше public static readonly т.к. consts меняется только при повторной компиляции всего solution и зависимых проектов
    // public static readonly подставит ссылку на объект, const быстрее чем readonly выбор зависит от контекста
    // const можно использовать внтури сборки или приватные внутри класса
}