using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetworkEquipmentStore.Models.Repository
{
    using NetworkEquipmentStore.Models.DAO;

    public class Repository
    {
        private readonly UserDAO userDAO = new UserDAO();
        private readonly ProductDAO productDAO = new ProductDAO();
        private readonly OrderDAO orderDAO = new OrderDAO();


        public User GetUserByLogin(string login) => userDAO.GetUserByLogin(login);
        public IEnumerable<User> GetAllUsers() => userDAO.GetAllUsers();
        
        public void RegisterUser(User user)
        {
            if (GetUserByLogin(user.Login) != null)
            {
                throw new InvalidOperationException("user with this login is already exist");
            }
            else
            {
                userDAO.InsertUsers(user);
            }
        }

        public void UpdateUserInfo(User user) => userDAO.UpdateUsers(user);
        public void DeleteUser(User user) => userDAO.DeleteUsers(user);


        public IEnumerable<Product> GetAllProducts() => productDAO.GetAllProducts();
        public void InsertProduct(Product product) => productDAO.InsertProducts(product);
        public void UpdateProduct(Product product) => productDAO.UpdateProducts(product);
        public void DeleteProduct(Product product) => productDAO.DeleteProducts(product);


        public IEnumerable<Order> GetAllOrders() => orderDAO.GetAllOrders();
        public IEnumerable<Order> GetAllOrdersOfUser(User user) => orderDAO.GetAllOrdersOfUser(user);
        public IEnumerable<Order> GetNotPaidOrdersOfUser(User user) => orderDAO.GetNotPaidOrdersOfUser(user);
        
        public void InsertOrder(Order order)
        {
            foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
            {
                Product product = productDAO.GetProductByID(productOrderInfo.Product.ID);
                int totalQuantity = product.Quantity;
                int orderQuantity = productOrderInfo.Quantity;

                if (orderQuantity > totalQuantity)
                {
                    throw new InvalidOperationException("products quantity in order is greater than total quantity of this product");
                }
                else
                {
                    if (order.IsPaid)
                    {
                        product.Quantity = totalQuantity - orderQuantity;
                        productDAO.UpdateProducts(product);
                    }
                }
            }

            if (order.ProductsInfo.Count() != 0)
            {
                orderDAO.InsertOrders(order);
            }
        }

        public void UpdateOrder(Order order)
        {
            foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
            {
                Product product = productDAO.GetProductByID(productOrderInfo.Product.ID);
                int totalQuantity = product.Quantity;
                int orderQuantity = productOrderInfo.Quantity;

                if (orderQuantity > totalQuantity)
                {
                    throw new InvalidOperationException("products quantity in order is greater than total quantity of this product");
                }
                else
                {
                    if (order.IsPaid)
                    {
                        product.Quantity = totalQuantity - orderQuantity;
                        productDAO.UpdateProducts(product);
                    }
                }
            }

            if (order.ProductsInfo.Count() != 0)
            {
                orderDAO.UpdateOrders(order);
            }
            else
            {
                orderDAO.DeleteOrders(order);
            }
        }

        public void DeleteOrder(Order order) => orderDAO.DeleteOrders(order);
    }
}
