namespace Sources.Common
{
    public class FancyNameProvider
    {
        private static readonly string[] FancyNames =
        {
            "Gordon Freeman",
            "Duke Nukem",
            "Lara Croft",
            "Samus Aran",
            "Link",
            "Master Chief",
            "Cloud Strife",
            "Geralt of Rivia",
            "Kratos",
            "Solid Snake"
        };

        public string GetName(int index)
        {
            index %= FancyNames.Length;
            
            return FancyNames[index];
        }
    }
}