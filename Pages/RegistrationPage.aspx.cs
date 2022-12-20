using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Routing;
using System.Web.UI;

namespace NetworkEquipmentStore.Pages
{
    public partial class RegistrationPage : Page
    {
        private readonly Repository repository = new Repository();


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            if (user != null)
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        protected void OnRegistration(object sender, EventArgs e)
        {
            if (!IsValidRegisterForm())
            {
                return;
            }

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
                    PasswordHash = passwordHash,
                    IsBanned = false,
                    Cart = new Cart
                    {
                        ID = 0,
                        LastUpdate = DateTime.Now,
                        Lines = new List<CartLine>()
                    }
                };

                newUser = repository.RegisterUser(newUser);
                
                SessionHelper.AuthorizeUser(Session, newUser);
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
            catch (InvalidOperationException exception)
            {
                ShowError(exception.Message);
            }
        }

        private bool IsValidRegisterForm() =>
            IsUsernameValid() && IsLoginValid() &&
            IsPasswordValid() && IsPasswordSame();

        private bool IsUsernameValid()
        {
            string username = Username.Value;

            if (username.Length == 0)
            {
                ShowError("имя пользователя не указано");
                return false;
            }
            else if (username.Length > 20)
            {
                ShowError("имя пользователя больше 20 символов");
                return false;
            }
            else if (IsContainsSpaces(username))
            {
                ShowError("имя пользователя не должно содержать пробелы");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsLoginValid()
        {
            string login = Login.Value;

            if (login.Length < 5)
            {
                ShowError("логин меньше 5 символов");
                return false;
            }
            else if (login.Length > 20)
            {
                ShowError("логин больше 20 символов");
                return false;
            }
            else if (
                !IsContainsLowerCase(login) ||
                !IsContainsUpperCase(login) ||
                !IsContainsNotLetters(login) ||
                IsContainsSpaces(login))
            {
                ShowError("логин должен содержать хотя бы 1 строчную и заглавную буквы, хотя бы 1 не буквенный символ, и не должен содержать пробелы");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsPasswordValid()
        {
            string password = Password.Value;

            if (password.Length < 8)
            {
                ShowError("пароль меньше 8 символов");
                return false;
            }
            else if (password.Length > 20)
            {
                ShowError("пароль больше 20 символов");
                return false;
            }
            else if (
                !IsContainsLowerCase(password) ||
                !IsContainsUpperCase(password) ||
                !IsContainsNotLetters(password) ||
                IsContainsSpaces(password))
            {
                ShowError("пароль должен содержать хотя бы 1 строчную и заглавную буквы, хотя бы 1 не буквенный символ, и не должен содержать пробелы");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsPasswordSame()
        {
            string password = Password.Value;
            string passwordConfirmed = PasswordConfirm.Value;

            if (password != passwordConfirmed)
            {
                ShowError("пароли не совпадают");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsContainsLowerCase(string str) => Regex.IsMatch(str, @".*[a-z].*");
        private bool IsContainsUpperCase(string str) => Regex.IsMatch(str, @".*[A-Z].*");
        private bool IsContainsNotLetters(string str) => Regex.IsMatch(str, @".*[^a-zA-Z].*");
        private bool IsContainsSpaces(string str) => Regex.IsMatch(str, @".*\s.*");

        private void ShowError(string message)
        {
            ErrorLabel.Text = $"Ошибка: {message}!";
            ErrorLabel.ForeColor = System.Drawing.Color.Red;
        }
    }
}
