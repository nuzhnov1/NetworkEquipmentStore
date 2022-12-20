using System;
using System.Web.UI;
using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Pages.Helpers;
using System.Web.Routing;

namespace NetworkEquipmentStore.Pages
{
    public partial class StorePage : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (Request.Form["Exit"] != null)
                {
                    OnExit();
                }
            }
        }

        protected void CreateTopNavList()
        {
            User user = SessionHelper.GetUser(Session);

            string nullHref = "javascript:void(0)";
            string mainHref = RouteTable.Routes.GetVirtualPath(null, null).VirtualPath;
            string cartHref = RouteTable.Routes.GetVirtualPath(null, "cart", null).VirtualPath;
            string addProductHref = RouteTable.Routes.GetVirtualPath(null, "product", null).VirtualPath;
            string ordersHref = RouteTable.Routes.GetVirtualPath(null, "orders", null).VirtualPath;
            string clientsHref = RouteTable.Routes.GetVirtualPath(null, "clients", null).VirtualPath;
            string authorizationHref = RouteTable.Routes.GetVirtualPath(null, "authorization", null).VirtualPath;


            // Если текущая страница - главная
            if (Page is ProductsPage)
            {
                Response.Write($"<li><a href='{nullHref}' class='selected'>Главная</a></li>");
            }
            else
            {
                Response.Write($"<li><a href='{mainHref}'>Главная</a></li>");
            }

            // Если текущая страница - главная, и пользователь - клиент
            if (Page is CartPage && user != null && user.Level == PermissionsLevel.CLIENT)
            {
                Response.Write($"<li><a href='{nullHref}' class='selected'>Корзина</a></li>");
            }
            // Если пользователь - клиент, но текущая страница другая
            else if (user != null && user.Level == PermissionsLevel.CLIENT)
            {
                Response.Write($"<li><a href='{cartHref}'>Корзина</a></li>");
            }

            // Если текущая страница - страница для добавления/обновления продукта, и пользователь - админ
            if (Page is ProductPage && user != null && user.Level == PermissionsLevel.ADMIN)
            {
                Response.Write($"<li><a href='{nullHref}' class='selected'>Добавить продукт</a></li>");
            }
            // Если пользователь - админ, но текущая страница другая
            else if (user != null && user.Level == PermissionsLevel.ADMIN)
            {
                Response.Write($"<li><a href='{addProductHref}'>Добавить продукт</a></li>");
            }

            // Если текущая страница - страница заказов, и пользователь залогинен
            if (Page is OrdersPage && user != null)
            {
                Response.Write($"<li><a href='{nullHref}' class='selected'>Список заказов</a></li>");
            }
            // Если пользователь залогинен, но страница другая
            else if (user != null)
            {
                Response.Write($"<li><a href='{ordersHref}'>Список заказов</a></li>");
            }

            // Если текущая страница - страница списка клиентов, и пользователь - админ
            if (Page is ClientsPage && user != null && user.Level == PermissionsLevel.ADMIN)
            {
                Response.Write($"<li><a href='{nullHref}' class='selected'>Список клиентов</a></li>");
            }
            // Если пользователь - админ, но текущая страница другая
            else if (user != null && user.Level == PermissionsLevel.ADMIN)
            {
                Response.Write($"<li><a href='{clientsHref}'>Список клиентов</a></li>");
            }

            Response.Write("<li id='dropdown'>");

            // Если текущая страница - страница с регистрацией или авторизацией, и пользователь не залогинен
            if ((Page is AuthorizationPage || Page is RegistrationPage) && user == null)
            {
                Response.Write($"<a href='{nullHref}' class='selected'>Войти</a>");
            }
            // Если текущая страница другая, но пользователь также не залогинен
            else if (user == null)
            {
                Response.Write($"<a href='{authorizationHref}'>Войти</a>");
            }
            else
            {
                if (user.Level == PermissionsLevel.ADMIN)
                {
                    Response.Write($"<span style='color: green'>{user.Name}</span>");
                }
                else
                {
                    Response.Write($"<span style='color: yellow'>{user.Name}</span>");
                }

                Response.Write("<div id='dropdownContent'>");
                Response.Write("<button name='Exit' type=submit>Выйти</button>");
                Response.Write("</div>");
            }
            
            Response.Write("</li>");
        }

        protected void OnExit()
        {
            SessionHelper.RemoveUser(Session);
            Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
        }
    }
}
