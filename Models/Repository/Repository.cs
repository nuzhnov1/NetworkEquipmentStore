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
        private readonly CartDAO cartDAO = new CartDAO();
        private readonly OrderDAO orderDAO = new OrderDAO();



        public User GetUserByLogin(string login) => userDAO.GetUserByLogin(login);
        public IEnumerable<User> GetAllUsers() => userDAO.GetAllUsers();
        
        public User RegisterUser(User user)
        {
            if (GetUserByLogin(user.Login) != null)
            {
                throw new InvalidOperationException("пользователь с таким логином уже существует");
            }
            else
            {
                user = userDAO.InsertUser(user);
                cartDAO.InsertCart(user);

                return user;
            }
        }

        public void DeleteUser(User user)
        {
            cartDAO.DeleteCart(user);
            userDAO.DeleteUser(user);
        }


        public Product GetProductByID(int productID) => productDAO.GetProductByID(productID);
        public IEnumerable<Product> GetAllProducts() => productDAO.GetAllProducts();
        public int GetAllProductsCountByCategory(ProductCategory category = ProductCategory.NONE) => productDAO.GetAllProductsCountByCategory(category);
        public Product InsertProduct(Product product) => productDAO.InsertProduct(product);
        public Product UpdateProduct(Product product) => productDAO.UpdateProduct(product);
        public void DeleteProductByID(int id) => productDAO.DeleteProductByID(id);


        public Cart GetUserCart(User user) => cartDAO.GetUserCart(user.ID);
        public Cart UpdateCart(Cart cart, User user) => cartDAO.UpdateCart(cart, user);


        public IEnumerable<Order> GetAllOrders() => orderDAO.GetAllOrders();
        public IEnumerable<Order> GetAllOrdersOfUser(User user) => orderDAO.GetAllOrdersOfUser(user.ID);
        
        public Order InsertOrder(Order order)
        {
            // Если заказ пустой, то не вставляем его:
            if (order.ProductsInfo.Count() == 0)
            {
                return null;
            }

            // Проверка правильности заказа:
            foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
            {
                Product product = productDAO.GetProductByID(productOrderInfo.Product.ID);
                int totalQuantity = product.Quantity;
                int orderQuantity = productOrderInfo.Quantity;

                if (orderQuantity > totalQuantity)
                {
                    throw new InvalidOperationException($"количество товаров '{product.Name}' в заказе больше чем имеется на складе");
                }
            }

            // Обновление данных о продуктах при правильности заказа:
            foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
            {
                Product product = productDAO.GetProductByID(productOrderInfo.Product.ID);
                int totalQuantity = product.Quantity;
                int orderQuantity = productOrderInfo.Quantity;

                product.Quantity = totalQuantity - orderQuantity;
                productDAO.UpdateProduct(product);
            }

            return orderDAO.InsertOrder(order);
        }

        public void DeleteOrderByID(int orderID) => orderDAO.DeleteOrderByID(orderID);
    }
}
