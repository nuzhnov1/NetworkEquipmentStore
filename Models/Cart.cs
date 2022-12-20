using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkEquipmentStore.Models
{
    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    public class Cart
    {
        public int ID { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<CartLine> Lines { get; set; }
    }

    public static class CartExtensions
    {
        public static decimal TotalCost(this Cart cart) => cart.Lines.Sum(line => line.Product.Price * line.Quantity);

        public static bool IsInCart(this Cart cart, Product product) => cart.Lines
            .Where(l => l.Product.ID == product.ID)
            .Count() > 0;

        public static void AddLine(this Cart cart, CartLine cartLine)
        {
            CartLine line = cart.Lines
                .Where(l => l.Product.ID == cartLine.Product.ID)
                .FirstOrDefault();

            if (line != null)
            {
                cart.Lines.Remove(line);
            }

            cart.Lines.Add(cartLine);
        }

        public static void AddOneProduct(this Cart cart, int productID)
        {
            CartLine line = cart.Lines
                .Where(l => l.Product.ID == productID)
                .FirstOrDefault();

            line.Quantity += 1;
        }

        public static void RemoveLine(this Cart cart, int productID)
        {
            cart.Lines.RemoveAll(l => l.Product.ID == productID);
        }

        public static void RemoveOneProduct(this Cart cart, int productID)
        {
            CartLine line = cart.Lines
                .Where(l => l.Product.ID == productID)
                .FirstOrDefault();

            line.Quantity -= 1;

            if (line.Quantity == 0)
            {
                cart.Lines.Remove(line);
            }
        }

        public static void Clear(this Cart cart)
        {
            cart.Lines.Clear();
        }
    }
}
