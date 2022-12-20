using System;
using System.Collections.Generic;
using System.Linq;
using NetworkEquipmentStore.Models.DAO;

namespace NetworkEquipmentStore.Models.Repository
{
    public class Repository
    {
        private readonly UserDAO userDAO = new UserDAO();
        private readonly ProductDAO productDAO = new ProductDAO();
        private readonly OrderDAO orderDAO = new OrderDAO();



        public User GetUserByID(int id) => userDAO.GetUserByID(id);
        public User GetUserByName(string name) => userDAO.GetUserByName(name);
        public User GetUserByLogin(string login) => userDAO.GetUserByLogin(login);
        public IEnumerable<User> GetAllUsers() => userDAO.GetAllUsers();
        
        public User RegisterUser(User user)
        {
            if (GetUserByName(user.Name) != null)
            {
                throw new InvalidOperationException("пользователь с таким именем уже существует");
            }
            else if (GetUserByLogin(user.Login) != null)
            {
                throw new InvalidOperationException("пользователь с таким логином уже существует");
            }
            else
            {
                return userDAO.InsertUser(user);
            }
        }

        public User UpdateUser(User user) => userDAO.UpdateUser(user);
        public void DeleteUser(User user) => userDAO.DeleteUser(user);


        public Product GetProductByID(int productID) => productDAO.GetProductByID(productID);
        public IEnumerable<Product> GetAllProducts() => productDAO.GetAllProducts();
        public int GetAllProductsCountByCategory(ProductCategory category = ProductCategory.NONE) => productDAO.GetAllProductsCountByCategory(category);
        public Product InsertProduct(Product product) => productDAO.InsertProduct(product);
        public Product UpdateProduct(Product product) => productDAO.UpdateProduct(product);
        public void DeleteProductByID(int id) => productDAO.DeleteProductByID(id);


        public IEnumerable<Order> GetAllOrders() => orderDAO.GetAllOrders();
        public int GetAllOrdersCount(User user = null) => orderDAO.GetAllOrdersCount(user);

        public Order InsertOrder(
            User user,
            string customerName, string customerPhone,
            string customerEmail, string customerAddress
        )
        {
            Cart cart = user.Cart;

            // Если заказ пустой, то не вставляем его:
            if (cart.Lines.Count() == 0)
            {
                return null;
            }

            // Проверка правильности заказа:
            foreach (CartLine cartLine in cart.Lines)
            {
                Product product = productDAO.GetProductByID(cartLine.Product.ID);
                int totalQuantity = product.Quantity;
                int cartQuantity = cartLine.Quantity;

                if (cartQuantity > totalQuantity)
                {
                    throw new InvalidOperationException($"количество товаров '{product.Name}' в корзине больше чем имеется на складе");
                }
            }

            // Обновление данных о продуктах при правильности заказа:
            foreach (CartLine cartLine in cart.Lines)
            {
                Product product = productDAO.GetProductByID(cartLine.Product.ID);
                int totalQuantity = product.Quantity;
                int cartQuantity = cartLine.Quantity;

                product.Quantity = totalQuantity - cartQuantity;
                productDAO.UpdateProduct(product);
            }

            return orderDAO.InsertOrder(user.FormOrder(customerName, customerPhone, customerEmail, customerAddress));
        }

        public void DeleteOrderByID(int orderID) => orderDAO.DeleteOrderByID(orderID);
        public void DeleteAllOrders(int? userID = null) => orderDAO.DeleteAllOrders(userID);
    }
}
