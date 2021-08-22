using System;

namespace TrueLayer.Api.Utilities
{
    public static class Check
    {
        public static void NotNull(object? o, string? name = null)
        {
            if (o is null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}