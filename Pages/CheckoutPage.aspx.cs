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

            if (user != null && user.Level == PermissionsLevel.CLIENT)
            {
                if (IsPostBack)
                {
                    if (Request.Form["SubmitOrder"] != null && IsCheckoutFormValid())
                    {
                        OnSubmitOrder();
                        CheckoutForm.Visible = false;
                        PostbackForm.Visible = true;
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
            Cart cart = SessionHelper.GetCart(Session);

            string name = Name.Value;
            string phone = Phone.Value;
            string email = Email.Value;
            string address = Address.Value;

            Order newOrder = new Order
            {
                ID = 0,
                User = user,
                ProductsInfo = cart.Lines,
                Date = DateTime.Now,
                CustomerName = name,
                CustomerPhone = phone,
                CustomerEmail = email,
                CustomerAddress = address
            };

            try
            {
                repository.InsertOrder(newOrder);
                cart.Clear();
                repository.UpdateCart(cart, user);
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
            catch (InvalidOperationException exception)
            {
                ShowError(exception.Message);
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
            else if (name.Length > 100)
            {
                ShowError("ФИО больше 100 символов");
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
            else if (address.Length > 1000)
            {
                ShowError("адрес содержит больше 1000 символов");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ShowError(string status)
        {
            ErrorLabel.Text = status;
            ErrorLabel.ForeColor = Color.Red;
        }
    }
}
