using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using SKOShopifyWebsite.Models;

namespace SKOShopifyWebsite.Services
{
    public class ShopifyService
    {
        private readonly HttpClient _client;

        public ShopifyService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Product>> GetProductsAsync(int first = 10)
        {
            var query = $@"
            {{
              products(first: {first}) {{
                edges {{
                  node {{
                    title
                    handle
                    description
                    options(first:5)
                    {{
                        name
                        values
                    }}
                    priceRange {{
                        minVariantPrice {{
                            amount
                            currencyCode
                        }}
                        maxVariantPrice {{
                            amount
                            currencyCode
                        }}
                    }}
                    images(first: 5) {{
                      edges {{
                        node {{
                          src
                        }}
                      }}
                    }}
                  }}
                }}
              }}
            }}";

            var requestBody = JsonSerializer.Serialize(new { query });
            var response = await _client.PostAsync("", new StringContent(
                requestBody, Encoding.UTF8, "application/json"
            ));

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            // navigate JSON to products.edges[].node
            var edges = doc.RootElement
                          .GetProperty("data")
                          .GetProperty("products")
                          .GetProperty("edges")
                          .EnumerateArray();

            var products = new List<Product>();
            foreach (var edge in edges)
            {
                var node = edge.GetProperty("node").ToString();
                products.Add(JsonSerializer.Deserialize<Product>(node)!);
            }
            return products;
        }

        public async Task<Product> GetProductByHandleAsync(string handle)
        {
            var query = $@"
            {{
                productByHandle(handle: ""{handle}"") {{
                    title
                    handle
                    description
                    options(first: 5) {{
                        name
                        values
                    }}
                    priceRange {{
                        minVariantPrice {{
                            amount
                            currencyCode
                        }}
                    }}
                    images(first: 10) {{
                        edges {{
                            node {{
                                src
                            }}
                        }}
                    }}
                    variants(first: 20) {{
                        edges {{
                            node {{
                                id
                                priceV2 {{
                                    amount
                                    currencyCode
                                }}
                                selectedOptions {{
                                    name
                                    value
                                }}
                            }}
                        }}
                    }}
                }}
            }}";

            var requestBody = JsonSerializer.Serialize(new { query });
            var response = await _client.PostAsync("", new StringContent(
                requestBody, Encoding.UTF8, "application/json"
            ));

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var productElem = doc.RootElement
                                 .GetProperty("data")
                                 .GetProperty("productByHandle");

            var product = JsonSerializer.Deserialize<Product>(productElem.GetRawText());
            return product;
        }

        public string CurrentCartId { get; private set; }
        public string CheckoutUrl { get; private set; }

        public async Task<CartResult> AddItemToCartAsync(string variantId)
        {
            var query = @"
            mutation cartCreate($lines: [CartLineInput!]!) {
                cartCreate(input: { lines: $lines }) {
                cart {
                    id
                    checkoutUrl
                }
                userErrors {
                    field
                    message
                }
                }
            }";

            var variables = new
            {
                lines = new[]
                {
            new {
                quantity = 1,
                merchandiseId = variantId
            }
        }
            };

            var body = new
            {
                query,
                variables
            };

            var json = JsonSerializer.Serialize(body);
            var response = await _client.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var cartElem = doc.RootElement
                .GetProperty("data")
                .GetProperty("cartCreate")
                .GetProperty("cart");

            CurrentCartId = cartElem.GetProperty("id").GetString();
            CheckoutUrl = cartElem.GetProperty("checkoutUrl").GetString();

            return new CartResult
            {
                CartId = CurrentCartId,
                CheckoutUrl = CheckoutUrl
            };
        }

        public class CartResult
        {
            public string CartId { get; set; }
            public string CheckoutUrl { get; set; }
        }

        public async Task<Cart> GetCartAsync(string cartId)
        {
            var query = @"
            query GetCart($cartId: ID!) {
              cart(id: $cartId) {
                id
                checkoutUrl
                lines(first: 10) {
                  edges {
                    node {
                      quantity
                      merchandise {
                        ... on ProductVariant {
                          id
                          title
                          image {
                            src
                          }
                          product {
                            title
                          }
                          priceV2 {
                            amount
                            currencyCode
                          }
                        }
                      }
                    }
                  }
                }
              }
            }";

            var variables = new { cartId };

            var body = new { query, variables };
            var json = JsonSerializer.Serialize(body);

            var response = await _client.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var cartElem = doc.RootElement
                .GetProperty("data")
                .GetProperty("cart");

            return JsonSerializer.Deserialize<Cart>(cartElem.GetRawText());
        }

        public async Task<CartResult> CreateCartAsync()
        {
            var query = @"
            mutation {
                cartCreate {
                    cart {
                        id
                        checkoutUrl
                    }
                    userErrors {
                        field
                        message
                    }
                }
            }";

            var requestBody = new
            {
                query
            };

            var json = JsonSerializer.Serialize(requestBody);
            var response = await _client.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var root = doc.RootElement.GetProperty("data").GetProperty("cartCreate");

            // Optional: Handle userErrors
            if (root.TryGetProperty("userErrors", out var errors) && errors.GetArrayLength() > 0)
            {
                var error = errors[0];
                throw new Exception($"Cart create error: {error.GetProperty("message").GetString()}");
            }

            var cart = root.GetProperty("cart");
            var cartId = cart.GetProperty("id").GetString();
            var checkoutUrl = cart.GetProperty("checkoutUrl").GetString();

            return new CartResult
            {
                CartId = cartId!,
                CheckoutUrl = checkoutUrl!
            };
        }

    }
}
