using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NetworkEquipmentStore.Pages
{
    public partial class Authorization : Page
    {
        private readonly Repository repository = new Repository();


        private void ShowError(string message)
        {
            ErrorLabel.ForeColor = System.Drawing.Color.Red;
            ErrorLabel.Text = "Ошибка: " + message;
            ErrorLabel.Visible = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пользователь уже в системе
            if (user != null)
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }

            RegisterLink.HRef = RouteTable.Routes.GetVirtualPath(null, "registration", null).VirtualPath;
            MainLink.HRef = RouteTable.Routes.GetVirtualPath(null, null).VirtualPath;
        }

        protected void LoginButtonClick(object sender, EventArgs e)
        {
            string login = Login.Value;
            string password = Password.Value;
            string passwordHash = EncoderHelper.Base64Encode(password);

            User user = repository.GetUserByLogin(login);

            if (user != null)
            {
                if (passwordHash == user.PasswordHash)
                {
                    SessionHelper.AuthorizeUser(Session, user);
                    Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
                }
                else
                {
                    ShowError("неверный пароль!");
                }
            }
            else
            {
                ShowError("неверный логин!");
            }
        }
    }
}
