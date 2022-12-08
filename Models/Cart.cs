using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetworkEquipmentStore.Models
{
    public class Cart
    {
        private readonly List<ProductOrderInfo> cartLines = new List<ProductOrderInfo>();
        public IEnumerable<ProductOrderInfo> Lines => cartLines;

        
        public bool IsInCart(Product product) => cartLines
            .Where(l => l.Product.ID == product.ID)
            .Count() > 0;

        public void AddLine(Product product, int quantity)
        {
            ProductOrderInfo line = cartLines
                .Where(l => l.Product.ID == product.ID)
                .FirstOrDefault();

            if (line != null)
            {
                cartLines.Remove(line);
            }

            cartLines.Add(new ProductOrderInfo
            {
                Product = product,
                Quantity = quantity
            });
        }

        public void AddOneProduct(Product product)
        {
            ProductOrderInfo line = cartLines
                .Where(l => l.Product.ID == product.ID)
                .FirstOrDefault();
            
            line.Quantity += 1;
        }

        public void RemoveLine(Product product)
        {
            cartLines.RemoveAll(l => l.Product.ID == product.ID);
        }

        public void RemoveOneProduct(Product product)
        {
            ProductOrderInfo line = cartLines
                .Where(l => l.Product.ID == product.ID)
                .FirstOrDefault();

            line.Quantity -= 1;

            if (line.Quantity == 0)
            {
                cartLines.Remove(line);
            }
        }

        public void Clear()
        {
            cartLines.Clear();
        }
    }
}
