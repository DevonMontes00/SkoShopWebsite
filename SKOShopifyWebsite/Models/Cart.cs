using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace SKOShopifyWebsite.Models
{
    public class Cart
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; }

        [JsonPropertyName("lines")]
        public CartLineConnection Lines { get; set; }
    }

    public class CartLineConnection
    {
        [JsonPropertyName("edges")]
        public List<CartLineEdge> Edges { get; set; }
    }

    public class CartLineEdge
    {
        [JsonPropertyName("node")]
        public CartLine Node { get; set; }
    }

    public class CartLine
    {
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("merchandise")]
        public ProductVariant Merchandise { get; set; }
    }

    public class ProductVariant
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("image")]
        public VariantImage Image { get; set; }

        [JsonPropertyName("product")]
        public CartProduct Product { get; set; }

        [JsonPropertyName("priceV2")]
        public MoneyV2 PriceV2 { get; set; }
    }

    public class VariantImage
    {
        [JsonPropertyName("src")]
        public string Src { get; set; }
    }

    public class CartProduct
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
