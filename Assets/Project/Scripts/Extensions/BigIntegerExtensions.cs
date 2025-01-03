namespace System.Numerics
{
    public static class BigIntegerExtensions
    {
        // Список для сокращений
        private static readonly string[] Suffixes = GenerateSuffixes();
        private static double SUPPORT_VAR_FOR_MULTIPLY = 1000000000d;

        public static BigInteger MultiplyByFloat(this BigInteger value, float multulier)
        {
            return value * new BigInteger(multulier * SUPPORT_VAR_FOR_MULTIPLY) / new BigInteger(SUPPORT_VAR_FOR_MULTIPLY);
        }

        public static string ToShortString(this BigInteger value)
        {
            // Если значение меньше 1000, просто возвращаем его как есть
            if (value < 1000)
            {
                return value.ToString();
            }

            // Перебираем степени тысячи (1000, 1_000_000, 1_000_000_000 и т.д.)
            int magnitude = 0;
            BigInteger divisor = 1;
            while (value / divisor >= 1000)
            {
                divisor *= 1000;
                magnitude++;
            }

            // Делим число на 1000 в соответствующей степени
            BigInteger shortValue = value / divisor;

            // Форматируем сокращенное значение с нужным суффиксом
            string suffix = Suffixes[magnitude - 1]; // -1, потому что для тысяч суффикс первый (K)
            return $"{shortValue}{suffix}";
        }

        // Генерация суффиксов для больших чисел
        private static string[] GenerateSuffixes()
        {
            var suffixes = new string[702]; // Достаточно много для большинства реальных значений
            suffixes[0] = "k";  // Тысячи
            suffixes[1] = "m";  // Миллионы
            suffixes[2] = "b";  // Миллиарды
            suffixes[3] = "t";  // Триллионы

            // Дальше идут комбинации букв: aa, ab, ac и т.д.
            int index = 4;
            for (char first = 'a'; first <= 'z'; first++)
            {
                for (char second = 'a'; second <= 'z'; second++)
                {
                    suffixes[index++] = $"{first}{second}";
                }
            }

            return suffixes;
        }
    }
}
