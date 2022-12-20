using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Routing;
using System.Web.UI;

namespace NetworkEquipmentStore.Pages
{
    public partial class ClientsPage : Page
    {
        private readonly Repository repository = new Repository();


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            if (user != null && user.Level == PermissionsLevel.ADMIN)
            {
                if (IsPostBack)
                {
                    if (Request.Form["BanUser"] != null)
                    {
                        string selectedUsername = Request.Form["BanUser"];
                        User selectedUser = repository.GetUserByName(selectedUsername);

                        if (selectedUser != null)
                        {
                            selectedUser.IsBanned = true;
                            repository.UpdateUser(selectedUser);
                        }
                    }
                    else if (Request.Form["UnBanUser"] != null)
                    {
                        string selectedUsername = Request.Form["UnBanUser"];
                        User selectedUser = repository.GetUserByName(selectedUsername);

                        if (selectedUser != null)
                        {
                            selectedUser.IsBanned = false;
                            repository.UpdateUser(selectedUser);
                        }
                    }
                    else if (Request.Form["RemoveUser"] != null)
                    {
                        string selectedUsername = Request.Form["RemoveUser"];
                        User selectedUser = repository.GetUserByName(selectedUsername);

                        if (selectedUser != null)
                        {
                            repository.DeleteUser(selectedUser);
                        }
                    }
                }
            }
            else
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        protected void CreateUsersRoot()
        {
            User currentUser = SessionHelper.GetUser(Session);

            if (!(currentUser != null && currentUser.Level == PermissionsLevel.ADMIN))
            {
                return;
            }

            IEnumerable<User> users = repository.GetAllUsers();

            Response.Write("<table id='users-list'>");

            Response.Write("<tr>");
            Response.Write("<th>Имя пользователя</th>");
            Response.Write("<th>Уровень прав доступа</th>");
            Response.Write("<th>Забанен ли?</th>");
            Response.Write("</tr>");

            foreach (User user in users)
            {
                string isBannedString = (user.IsBanned) ? "Да" : "Нет";

                Response.Write("<tr>");
                Response.Write($"<td>{user.Name}</td>");
                Response.Write($"<td>{user.Level.ToWebRepresentation()}</td>");
                Response.Write($"<td>{isBannedString}</td>");
                
                if (user.Level == PermissionsLevel.CLIENT)
                {
                    string ordersPath = RouteTable.Routes.GetVirtualPath(null, "orders",
                        new RouteValueDictionary() { { "username", user.Name } }).VirtualPath;

                    Response.Write($"<td><a href='{ordersPath}'>Заказы клиента</a></td>");

                    if (user.IsBanned)
                    {
                        Response.Write($"<td><button name='UnBanUser' value='{user.Name}' type='submit'>Разбанить</button></td>");
                    }
                    else
                    {
                        Response.Write($"<td><button name='BanUser' value='{user.Name}' type='submit'>Забанить</button></td>");
                    }

                    Response.Write($"<td><button name='RemoveUser' value='{user.Name}' type='submit'>Удалить</button></td>");
                }

                Response.Write("</tr>");
            }

            Response.Write("</table>");
        }
    }
}
