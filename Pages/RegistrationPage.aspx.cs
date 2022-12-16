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
    public partial class RegistrationPage : Page
    {
        private readonly Repository repository = new Repository();

        public object EncoderHelpder { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пользователь уже в системе
            if (user != null)
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        protected void RegistrationButtonClick(object sender, EventArgs e)
        {
            string username = Username.Value;
            string login = Login.Value;
            string password = Password.Value;
            string passwordHash = EncoderHelper.Base64Encode(password);

            try
            {
                User newUser = new User
                {
                    ID = 0,
                    Name = username,
                    Level = PermissionsLevel.CLIENT,
                    Login = login,
                    PasswordHash = passwordHash
                };
                repository.RegisterUser(newUser);

                // Запрашиваем ID зарегистрированного пользователя:
                newUser = repository.GetUserByLogin(login);

                SessionHelper.AuthorizeUser(Session, newUser);
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
            catch (InvalidOperationException)
            {
                ShowError("пользователь с таким логином уже существует!");
            }
        }

        private void ShowError(string message)
        {
            ErrorLabel.ForeColor = System.Drawing.Color.Red;
            ErrorLabel.Text = "Ошибка: " + message;
            ErrorLabel.Visible = true;
        }
    }
}
