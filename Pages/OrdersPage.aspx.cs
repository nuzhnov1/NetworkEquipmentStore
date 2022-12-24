using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.UI;

namespace NetworkEquipmentStore.Pages
{
    public partial class OrdersPage : Page
    {
        private const int PAGE_COUNT = 5;
        private const int ORDERS_COUNT = 5;


        private readonly Repository repository = new Repository();

        private User _selectedUser = null;
        private User SelectedUser
        {
            get
            {
                if (_selectedUser == null)
                {
                    User currentUser = SessionHelper.GetUser(Session);
                    
                    if (currentUser == null)
                    {
                        _selectedUser = null;
                    }
                    else if (currentUser.Level == PermissionsLevel.CLIENT)
                    {
                        _selectedUser = currentUser;
                    }
                    else
                    {
                        string reqValue = (string)RouteData.Values["username"] ?? Request.QueryString["username"];
                        _selectedUser = (reqValue != null) ? repository.GetUserByName(reqValue) : null;
                    }
                }

                return _selectedUser;
            }
        }

        private int? _selectedPage = null;
        private int SelectedPage
        {
            get
            {
                if (_selectedPage == null)
                {
                    string reqValue = (string)RouteData.Values["page"] ?? Request.QueryString["page"];
                    int page = reqValue != null && int.TryParse(reqValue, out page) ? page : 1;

                    if (page > 0)
                    {
                        _selectedPage = page > MaxPage ? MaxPage : page;
                    }
                    else
                    {
                        Response.RedirectPermanent(GetVirtualPath(SelectedUser, 1));
                        _selectedPage = 1;
                    }
                }

                return (int)_selectedPage;
            }
        }

        private int? _maxPage = null;
        private int MaxPage
        {
            get
            {
                if (_maxPage == null)
                {
                    _maxPage = (int)Math.Ceiling((decimal)repository.GetAllOrdersCount(SelectedUser) / ORDERS_COUNT);
                }

                return (int)_maxPage;
            }
        }

        private IEnumerable<Order> _selectedOrders = null;
        private IEnumerable<Order> SelectedOrders
        {
            get
            {
                if (_selectedOrders == null)
                {
                    _selectedOrders = repository.GetAllOrders()
                        .Where(order => SelectedUser == null || order.User.ID == SelectedUser.ID)
                        .OrderBy(order => order.ID)
                        .Skip((SelectedPage - 1) * ORDERS_COUNT)
                        .Take(ORDERS_COUNT);
                }

                return _selectedOrders;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пришёл ответ от пользователя и этот пользователь - админ, то выполняем его запрос
            if (IsPostBack && user != null && user.Level == PermissionsLevel.ADMIN)
            {
                if (int.TryParse(Request.Form["RemoveOrder"], out int selectedOrder))
                {
                    repository.DeleteOrderByID(selectedOrder);
                }
                else if (Request.Form["DeleteAllOrders"] != null)
                {
                    repository.DeleteAllOrders(SelectedUser?.ID);
                }
            }
            // Если пользователь не зарегистрирован, то перенаправляем его на главную страницу
            else if (user == null)
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        private string GetVirtualPath(User user, int page)
        {
            if (user != null)
            {
                return RouteTable.Routes.GetVirtualPath(null, "orders",
                    new RouteValueDictionary() {
                        { "username", user.Name }, { "page", page }
                    }).VirtualPath;
            }
            else
            {
                if (page != 1)
                {
                    return RouteTable.Routes.GetVirtualPath(null, "orders",
                        new RouteValueDictionary() { { "page", page } }).VirtualPath;
                }
                else
                {
                    return RouteTable.Routes.GetVirtualPath(null, "orders", null).VirtualPath;
                }
            }
        }

        protected void CreateTitle()
        {
            User currentUser = SessionHelper.GetUser(Session);

            if (currentUser == null)
            {
                return;
            }
            else if (currentUser.Level == PermissionsLevel.CLIENT)
            {
                Response.Write("<h1>Ваши заказы</h1>");
            }
            else
            {
                User selectedUser = SelectedUser;

                if (selectedUser != null)
                {
                    Response.Write($"<h1>Все заказы клиента '{selectedUser.Name}'</h1>");
                }
                else
                {
                    Response.Write("<h1>Заказы всех пользователей</h1>");
                }
            }
        }

        protected void CreatePagesList()
        {
            User currentUser = SessionHelper.GetUser(Session);

            if (currentUser == null)
            {
                return;
            }

            int selectedPage = SelectedPage;
            int maxPage = MaxPage;
            User selectedUser = SelectedUser;

            if (maxPage == 0)
            {
                return;
            }

            Response.Write("<div id='pages'>");
            Response.Write("<span>Страницы: </span>");

            if (selectedPage > PAGE_COUNT)
            {
                string path = GetVirtualPath(selectedUser, 1);
                Response.Write($"<a href='{path}'>В начало</a>");
            }

            if (selectedPage > 1)
            {
                string path = GetVirtualPath(selectedUser, selectedPage - 1);
                Response.Write($"<a href='{path}'>Назад</a>");
            }

            int startPage = selectedPage - (selectedPage - 1) % PAGE_COUNT;
            int endPage = Math.Min(startPage + PAGE_COUNT, maxPage);
            for (int i = startPage; i <= endPage; i++)
            {
                string path = GetVirtualPath(selectedUser, i);

                if (i == selectedPage)
                {
                    Response.Write($"<a href='javascript:void(0)' class='page-selected'>{i}</a>");
                }
                else
                {
                    Response.Write($"<a href='{path}'>{i}</a>");
                }
            }

            if (selectedPage < maxPage)
            {
                string path = GetVirtualPath(selectedUser, selectedPage + 1);
                Response.Write($"<a href='{path}'>Вперёд</a>");
            }

            if (selectedPage <= maxPage - PAGE_COUNT)
            {
                string path = GetVirtualPath(selectedUser, maxPage);
                Response.Write($"<a href='{path}'>В конец</a>");
            }

            Response.Write("</div>");
        }

        protected void CreateOrdersList()
        {
            User currentUser = SessionHelper.GetUser(Session);

            if (currentUser == null)
            {
                return;
            }

            if (SelectedOrders.Count() == 0)
            {
                Response.Write("<span id='top-label'>Список заказов пуст</span>");
                return;
            }

            foreach (Order order in SelectedOrders)
            {
                Response.Write("<div id='list-item'>");

                if (currentUser.Level == PermissionsLevel.ADMIN && SelectedUser == null)
                {
                    Response.Write($"<h3>Заказ клиента '{order.User.Name}'</h3>");
                }
                else
                {
                    Response.Write($"<h3>Заказ №{order.ID}</h3>");
                }
                
                Response.Write($"<p style='font-weight: bold'>Дата и время заказа:<span style='font-weight: normal'> {order.OrderDate:F}</span></p>");
                
                Response.Write("<p style='font-weight: bold'>Реквизиты заказа:</p>");
                Response.Write("<div id='order-requisites'>");
                Response.Write($"<p style='font-weight: bold'>ФИО заказчика:<span style='font-weight: normal'> {order.CustomerName}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Номер телефона:<span style='font-weight: normal'> {order.CustomerPhone}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Электронная почта:<span style='font-weight: normal'> {order.CustomerEmail}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Адрес доставки:<span style='font-weight: normal'> {order.CustomerAddress}</span></p>");
                Response.Write("</div>");

                Response.Write("<p style='font-weight: bold'>Список товаров:</p>");
                Response.Write("<table>");
                Response.Write("<tr>");
                Response.Write("<th style='width: 30%'>Наименование товара</th>");
                Response.Write("<th style='width: 10%'>Категория товара</th>");
                Response.Write("<th style='width: 20%'>Цена товара на момент покупки</th>");
                Response.Write("<th style='width: 20%'>Количество купленного товара</th>");
                Response.Write("<th style='width: 20%'>Общая цена товаров</th>");
                Response.Write("</tr>");
                foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
                {
                    Response.Write("<tr>");
                    Response.Write($"<td style='width: 30%'>{productOrderInfo.ProductName}</td>");
                    Response.Write($"<td style='width: 10%'>{productOrderInfo.ProductCategory.ToWebRepresentation()}</td>");
                    Response.Write($"<td style='width: 20%'>{productOrderInfo.ProductPrice:c}</td>");
                    Response.Write($"<td style='width: 20%'>{productOrderInfo.Quantity}</td>");
                    Response.Write($"<td style='width: 20%'>{(productOrderInfo.ProductPrice * productOrderInfo.Quantity):c}</td>");
                    Response.Write("</tr>");
                }
                Response.Write("</table>");
                Response.Write($"<p style='font-weight: bold'>Общая стоимость заказа:<span style='font-weight: normal'> {order.TotalCost():c}</span></p>");

                if (currentUser.Level == PermissionsLevel.ADMIN)
                {
                    Response.Write($"<button name='RemoveOrder' value='{order.ID}' type='submit'>Удалить заказ</button>");
                }

                Response.Write("</div>");
            }
        }

        protected void CreateClearButton()
        {
            User currentUser = SessionHelper.GetUser(Session);

            if (currentUser != null && currentUser.Level == PermissionsLevel.ADMIN && SelectedOrders.Count() != 0)
            {
                if (SelectedUser == null)
                {
                    Response.Write("<button id='delete-all-orders' name='DeleteAllOrders' type='submit'>Удалить все заказы</button>");
                }
                else
                {
                    Response.Write($"<button id='delete-all-orders' name='DeleteAllOrders' type='submit'>Удалить все заказы клиента '{SelectedUser.Name}'</button>");
                }
            }
        }
    }
}
