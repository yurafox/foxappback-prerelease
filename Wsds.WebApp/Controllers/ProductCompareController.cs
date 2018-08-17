using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Infrastructure;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    public class SimilarObjectResponses
    {
        public string tags_id { get; set; }
    }

    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductCompareController : Controller
    {
        private readonly IProductRepository _prodRepo;

        public ProductCompareController(IProductRepository prodRepo)
        {
            _prodRepo = prodRepo;
        }

        [HttpGet("GetSimilarProducts/{id}")]
        public IActionResult GetSimilarProducts(long id)
        {
            return this.GetProductsFromService(id, 71);
        }

        [HttpGet("GetPopularAccessories/{id}")]
        public IActionResult GetPopularAccessories(long id)
        {
            return this.GetProductsFromService(id, 14482);
        }

        private IActionResult GetProductsFromService(long id, long block_id)
        {
            using (var client = new HttpClient())
            {
                int qtyReturnProducs = 10;
                string shop_id = "8D3869C";
                string cookie = "Foxtrot-mobile-app-cliendId-";

                var tModel = HttpContext.GetTokenModel();
                if (tModel != null)
                    cookie = cookie + tModel.ClientId.ToString();
                else
                    cookie = cookie + "notAuth";

                string url = UrlConstants.GetSimilarProductsService + $"?shop_id={shop_id}&user_cookie={cookie}&block_id={block_id.ToString()}&product={id.ToString()}";

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    List<List<SimilarObjectResponses>> listSimilarObjects = JsonConvert.DeserializeObject<List<List<SimilarObjectResponses>>>(responseString);

                    if (listSimilarObjects.Count > 0)
                    {
                        List<Product_DTO> products = new List<Product_DTO>();
                        foreach (SimilarObjectResponses item in listSimilarObjects[0])
                        {
                            long productId = 0;
                            long.TryParse(item.tags_id, out productId);
                            if (productId != 0)
                            {
                                Product_DTO product = _prodRepo.Product(productId);
                                if (product.status == 1 && product.valueQP != null && product.props.Count > 0)
                                {
                                    products.Add(product);

                                    if (products.Count >= qtyReturnProducs)
                                        break;
                                }
                            }
                        }

                        return Ok(products);
                    }
                    else
                        return NotFound();
                }
                else
                    return NotFound();
            }
        }

    }
}

