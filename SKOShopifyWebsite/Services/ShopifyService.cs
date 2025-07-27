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
                }}
                images(first: 3) {{
                    edges {{
                    node {{
                        src
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

    }
}
