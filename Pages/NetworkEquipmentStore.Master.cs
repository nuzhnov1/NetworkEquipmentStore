using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace NetworkEquipmentStore.Pages
{
    using Models;


    public partial class NetworkEquipmentStore : MasterPage
    {
        private const string USER_ID = "user_id";
        private const string USER_NAME = "user_name";
        private const string USER_LEVEL = "user_level";
        private const string USER_LOGIN = "user_login";
        private const string USER_PASSWORD_HASH = "user_password_hash";


        protected void Page_Load(object sender, EventArgs e)
        {
            // Если гость:
            if (Session[USER_LEVEL] == null)
            {
                Cart.Visible = false;
                Orders.Visible = false;
                Clients.Visible = false;
                LoginLink.Visible = true;
                UsernameLabel.Visible = false;
                DropdownContent.Visible = false;
            }
            // Если клиент:
            else
            {
                PermissionsLevel userLevel = (PermissionsLevel)Session[USER_LEVEL];

                if (userLevel == PermissionsLevel.CLIENT)
                {
                    Cart.Visible = true;
                    Orders.Visible = true;
                    Clients.Visible = false;
                    LoginLink.Visible = false;
                    UsernameLabel.Visible = true;
                    UsernameLabel.ForeColor = Color.Yellow;
                    DropdownContent.Visible = true;
                }
                else
                {
                    Cart.Visible = false;
                    Orders.Visible = true;
                    Clients.Visible = true;
                    LoginLink.Visible = false;
                    UsernameLabel.Visible = true;
                    UsernameLabel.ForeColor = Color.Green;
                    DropdownContent.Visible = true;
                }
            }
        }

        protected void ExitButton_Click(object sender, EventArgs e)
        {
            Session[USER_ID] = null;
            Session[USER_NAME] = null;
            Session[USER_LEVEL] = null;
            Session[USER_LOGIN] = null;
            Session[USER_PASSWORD_HASH] = null;

            Response.RedirectPermanent("/Pages/Products.aspx", false);
        }
    }
}
