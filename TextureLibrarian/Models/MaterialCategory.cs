using System.Collections.Generic;

namespace TextureLibrarian.Models
{
    public class MaterialCategory
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> Keywords { get; set; }

        public MaterialCategory()
        {
            Keywords = new List<string>();
        }

        public static List<MaterialCategory> GetDefaultCategories()
        {
            return new List<MaterialCategory>
            {
                new MaterialCategory { Name = "Metal", DisplayName = "Metal", Keywords = new List<string> { "metal", "steel", "iron", "copper", "brass", "aluminum" } },
                new MaterialCategory { Name = "Stone", DisplayName = "Stone", Keywords = new List<string> { "stone", "rock", "granite", "marble" } },
                new MaterialCategory { Name = "Fabric", DisplayName = "Fabric / Textile", Keywords = new List<string> { "fabric", "textile", "cloth", "cotton", "wool" } },
                new MaterialCategory { Name = "Concrete", DisplayName = "Concrete", Keywords = new List<string> { "concrete", "cement" } },
                new MaterialCategory { Name = "Brick", DisplayName = "Brick", Keywords = new List<string> { "brick", "masonry" } },
                new MaterialCategory { Name = "Plastic", DisplayName = "Plastic", Keywords = new List<string> { "plastic", "polymer" } },
                new MaterialCategory { Name = "Leather", DisplayName = "Leather", Keywords = new List<string> { "leather", "hide" } },
                new MaterialCategory { Name = "Ground", DisplayName = "Ground / Soil", Keywords = new List<string> { "ground", "soil", "dirt", "earth" } },
                new MaterialCategory { Name = "Grass", DisplayName = "Grass", Keywords = new List<string> { "grass", "lawn", "turf" } },
                new MaterialCategory { Name = "Sand", DisplayName = "Sand", Keywords = new List<string> { "sand", "beach", "desert" } },
                new MaterialCategory { Name = "Tile", DisplayName = "Tile", Keywords = new List<string> { "tile", "ceramic", "porcelain" } },
                new MaterialCategory { Name = "Paper", DisplayName = "Paper / Cardboard", Keywords = new List<string> { "paper", "cardboard", "carton" } },
                new MaterialCategory { Name = "Glass", DisplayName = "Glass", Keywords = new List<string> { "glass", "transparent" } },
                new MaterialCategory { Name = "Painted", DisplayName = "Painted Surfaces", Keywords = new List<string> { "paint", "painted", "coating" } },
                new MaterialCategory { Name = "Rusted", DisplayName = "Rusted / Corroded", Keywords = new List<string> { "rust", "rusted", "corroded", "oxidized" } },
                new MaterialCategory { Name = "Asphalt", DisplayName = "Asphalt", Keywords = new List<string> { "asphalt", "tarmac", "road" } },
                new MaterialCategory { Name = "Water", DisplayName = "Water", Keywords = new List<string> { "water", "liquid", "sea", "ocean" } },
                new MaterialCategory { Name = "Snow", DisplayName = "Snow / Ice", Keywords = new List<string> { "snow", "ice", "frozen" } },
                new MaterialCategory { Name = "Organic", DisplayName = "Organic / Bark / Leaves", Keywords = new List<string> { "bark", "leaves", "organic", "wood", "tree" } },
                new MaterialCategory { Name = "Uncategorized", DisplayName = "Uncategorized", Keywords = new List<string>() }
            };
        }
    }
}