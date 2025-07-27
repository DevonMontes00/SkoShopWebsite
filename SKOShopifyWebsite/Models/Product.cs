using System.Text.Json.Serialization;

namespace SKOShopifyWebsite.Models
{
    public class ProductEdge
    {
        public Product Node { get; set; }
    }

    public class ProductConnection
    {
        public List<ProductEdge> Edges { get; set; }
    }

    public class ImageEdge
    {
        [JsonPropertyName("node")]
        public ShopifyImage Node { get; set; }
    }

    public class ImageConnection
    {
        [JsonPropertyName("edges")]
        public List<ImageEdge> Edges { get; set; }
    }

    public class ShopifyImage
    {
        [JsonPropertyName("src")]
        public string Src { get; set; }
    }

    public class MoneyV2
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }
    public class PriceRange
    {
        [JsonPropertyName("minVariantPrice")]
        public MoneyV2 MinVariantPrice { get; set; }
        [JsonPropertyName("maxVariantPrice")]
        public MoneyV2 MaxVariantPrice { get; set; }
    }

    public class VariantNode
    {
        public MoneyV2 PriceV2 { get; set; }
    }

    public class VariantEdge
    {
        public VariantNode Node { get; set; }
    }

    public class VariantConnection
    {
        public List<VariantEdge> Edges { get; set; }
    }

    public class ProductOption
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("values")]
        public List<string> Values { get; set; }
    }

    public class SelectedOption
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Product
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("handle")]
        public string Handle { get; set; }
        [JsonPropertyName("images")]
        public ImageConnection Images { get; set; }
        [JsonPropertyName("priceRange")]
        public PriceRange PriceRange { get; set; }
        public VariantConnection Variants { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("options")]
        public List<ProductOption> Options { get; set; }
    }
}
