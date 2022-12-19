using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkEquipmentStore.Models
{
    public class Cart
    {
        private readonly List<ProductOrderInfo> cartLines = new List<ProductOrderInfo>();
        
        public DateTime LastUpdatedTime { get; set; }
        public IEnumerable<ProductOrderInfo> Lines
        {
            get => cartLines;
            set
            {
                cartLines.Clear();
                cartLines.AddRange(value);
            }
        }
        public decimal TotalCost => Lines.Sum(line => line.Product.Price * line.Quantity);


        public Cart() {}
        public Cart(IEnumerable<ProductOrderInfo> cartLines) => this.cartLines.AddRange(cartLines);


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

        public void AddOneProduct(int productID)
        {
            ProductOrderInfo line = cartLines
                .Where(l => l.Product.ID == productID)
                .FirstOrDefault();
            
            line.Quantity += 1;
        }

        public void RemoveLine(int productID)
        {
            cartLines.RemoveAll(l => l.Product.ID == productID);
        }

        public void RemoveOneProduct(int productID)
        {
            ProductOrderInfo line = cartLines
                .Where(l => l.Product.ID == productID)
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
