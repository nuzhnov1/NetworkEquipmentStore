using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web.UI;
using System.Web.Routing;

namespace NetworkEquipmentStore.Pages
{
    public partial class CheckoutPage : Page
    {
        private readonly Repository repository = new Repository();


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пользователь - не клиент, то перенаправляем его на главную страницу
            if (user != null && user.Level == PermissionsLevel.CLIENT)
            {
                if (IsPostBack)
                {
                    if (Request.Form["SubmitOrder"] != null && IsCheckoutFormValid())
                    {
                        OnSubmitOrder();
                    }
                }
                else
                {
                    CheckoutForm.Visible = true;
                    PostbackForm.Visible = false;
                }
            }
            else
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        protected void OnSubmitOrder()
        {
            User user = SessionHelper.GetUser(Session);

            string name = Name.Value;
            string phone = Phone.Value;
            string email = Email.Value;
            string address = Address.Value;

            try
            {
                if (user.Cart.Lines.Count > 0)
                {
                    repository.InsertOrder(user, name, phone, email, address);
                    user.Cart.Clear();
                    repository.UpdateUser(user);

                    CheckoutForm.Visible = false;
                    PostbackForm.Visible = true;
                }
                else
                {
                    ShowError("корзина для заказа пуста");

                    CheckoutForm.Visible = true;
                    PostbackForm.Visible = false;
                }
            }
            catch (InvalidOperationException exception)
            {
                ShowError(exception.Message);

                CheckoutForm.Visible = true;
                PostbackForm.Visible = false;
            }
        }

        private bool IsCheckoutFormValid() =>
            IsNameValid() && IsPhoneValid() &&
            IsEmailValid() && IsAddressValid();

        private bool IsNameValid()
        {
            string name = Name.Value;

            if (name.Length == 0)
            {
                ShowError("не указано ФИО");
                return false;
            }
            else if (name.Length > 60)
            {
                ShowError("ФИО больше 60 символов");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsPhoneValid()
        {
            string phone = Phone.Value;

            if (!Regex.IsMatch(phone, @"7\d{10}"))
            {
                ShowError("неверный номер телефона");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsEmailValid()
        {
            string email = Email.Value;

            if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                ShowError("неверный адрес электронной почты");
                return false;
            }
            else if (email.Length > 128)
            {
                ShowError("длина адреса электронной почты больше 128 символов");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsAddressValid()
        {
            string address = Address.Value;

            if (address.Length == 0)
            {
                ShowError("адрес не указан");
                return false;
            }
            else if (address.Length > 512)
            {
                ShowError("адрес содержит больше 512 символов");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = $"Ошибка: {message}!";
            ErrorLabel.ForeColor = Color.Red;
        }
    }
}
