﻿using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;
using System;
using System.Web.Routing;
using System.Web.UI;

namespace NetworkEquipmentStore.Pages
{
    public partial class AuthorizationPage : Page
    {
        private readonly Repository repository = new Repository();


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пользователь итак авторизирован, то перенаправляем его на главную страницу
            if (user != null)
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }

            RegisterLink.HRef = RouteTable.Routes.GetVirtualPath(null, "registration", null).VirtualPath;
        }

        protected void OnLogin(object sender, EventArgs e)
        {
            string login = Login.Value;
            string password = Password.Value;
            string passwordHash = EncoderHelper.Base64Encode(password);

            User user = repository.GetUserByLogin(login);

            if (user != null)
            {
                if (passwordHash == user.PasswordHash && !user.IsBanned)
                {
                    SessionHelper.AuthorizeUser(Session, user);
                    Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
                }
                else if (passwordHash != user.PasswordHash)
                {
                    ShowError("неверный пароль");
                }
                else
                {
                    ShowError("вы забанены");
                }
            }
            else
            {
                ShowError("неверный логин");
            }
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = $"Ошибка: {message}!";
            ErrorLabel.ForeColor = System.Drawing.Color.Red;
        }
    }
}
