using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Pages.Helpers;
using System.Web.Routing;

namespace NetworkEquipmentStore.Pages
{
    public partial class Store : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если гость:
            if (user == null)
            {
                Cart.Visible = false;
                Orders.Visible = false;
                Clients.Visible = false;
                Authorization.Visible = true;
                UsernameLabel.Visible = false;
                DropdownContent.Visible = false;
            }
            // Если клиент:
            else
            {
                if (user.Level == PermissionsLevel.CLIENT)
                {
                    Cart.Visible = true;
                    Orders.Visible = true;
                    Clients.Visible = false;
                    Authorization.Visible = false;
                    UsernameLabel.Visible = true;
                    UsernameLabel.ForeColor = Color.Yellow;
                    UsernameLabel.Text = user.Name;
                    DropdownContent.Visible = true;
                }
                else
                {
                    Cart.Visible = false;
                    Orders.Visible = true;
                    Clients.Visible = true;
                    Authorization.Visible = false;
                    UsernameLabel.Visible = true;
                    UsernameLabel.ForeColor = Color.Green;
                    UsernameLabel.Text = user.Name;
                    DropdownContent.Visible = true;
                }
            }

            Cart.HRef = RouteTable.Routes.GetVirtualPath(null, "cart", null).VirtualPath;
            Orders.HRef = RouteTable.Routes.GetVirtualPath(null, "orders", null).VirtualPath;
            Clients.HRef = RouteTable.Routes.GetVirtualPath(null, "clients", null).VirtualPath;
            Authorization.HRef = RouteTable.Routes.GetVirtualPath(null, "authorization", null).VirtualPath;
        }

        protected void ExitButtonClick(object sender, EventArgs e)
        {
            SessionHelper.RemoveUser(Session);
            Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
        }
    }
}
