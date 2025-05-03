namespace Sources.Common
{
    public class FancyNameProvider
    {
        private static readonly string[] FancyNames = new[]
        {
            "Gordon Freeman",
            "Duke Nukem",
            "Lara Croft",
            "Mario",
            "Link",
        };

        public string GetName(int index)
        {
            index %= FancyNames.Length;
            return FancyNames[index];
        }
    }
}