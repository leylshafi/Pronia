namespace Pronia.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string SecondaryImageUrl { get; set; }
        public DateTime CreatedTime { get; set; }

        
    }
}
