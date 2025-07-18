namespace TextureLibrarian.Models
{
    public class TexturePass
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public TexturePassType Type { get; set; }
    }

    public enum TexturePassType
    {
        BaseColor,
        Normal,
        Roughness,
        Height,
        AO,
        Metallic,
        Specular,
        Displacement,
        Opacity,
        Emission,
        Unknown
    }
}