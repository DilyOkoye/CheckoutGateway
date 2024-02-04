namespace AcquiringBank.Simulator.Helpers
{
    public static class RandomizeDeclineMessages
    {
        public static class EnumUtils
        {
            private static readonly Random Random = new();

            public static T? GetRandomEnumValue<T>() where T : Enum
            {
                var enumValues = Enum.GetValues(typeof(T));
                return (T)enumValues.GetValue(Random.Next(enumValues.Length))!;
            }
        }

    }
}
