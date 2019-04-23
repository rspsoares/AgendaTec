using AgendaTech.WebAPI.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace AgendaTech.WebAPI.Controllers
{
    public class HomeController : ApiController
    {
        public IEnumerable<Product> GetItems()
        {
            IList<Product> products = new List<Product>
            {
                new Product
                {
                    productName = "Biscuits",
                    manufacturingYear = 2018,
                    brandName="ParleG"
                },
                new Product
                {
                    productName = "Cars",
                    manufacturingYear = 2018,
                    brandName="BMW"
                },
                new Product
                {
                    productName = "Cars",
                    manufacturingYear = 2018,
                    brandName="Mercedese"
                },
                new Product
                {
                    productName = "Brush",
                    manufacturingYear = 2017,
                    brandName="Colgate"
                }

            };

            return products;
        }

        public string Test(string parm)
        {
            
            return parm;
        }
    }
}

