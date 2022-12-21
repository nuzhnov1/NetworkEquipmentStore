using System;
using System.Linq;
using System.Web.Routing;
using System.Web.UI;
using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;

namespace NetworkEquipmentStore.Pages
{
    public partial class CartPage : Page
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
                    Cart cart = user.Cart;

                    // Если клиент решил увеличить количество товара в корзине
                    if (Request.Form["IncrementProduct"] != null)
                    {
                        cart.AddOneProduct(int.Parse(Request.Form["IncrementProduct"]));
                        repository.UpdateUser(user);
                    }
                    // Если клиент решил уменьшить количество товара в корзине
                    else if (Request.Form["DecrementProduct"] != null)
                    {
                        cart.RemoveOneProduct(int.Parse(Request.Form["DecrementProduct"]));
                        repository.UpdateUser(user);
                    }
                    // Если клиент решил удалить товар из корзины
                    else if (Request.Form["RemoveLine"] != null)
                    {
                        cart.RemoveLine(int.Parse(Request.Form["RemoveLine"]));
                        repository.UpdateUser(user);
                    }
                    // Если клиент решил очистить корзину 
                    else if (Request.Form["Clear"] != null)
                    {
                        cart.Clear();
                        repository.UpdateUser(user);
                    }
                }
            }
            else
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        protected void CreateCartRoot()
        {
            User user = SessionHelper.GetUser(Session);

            if (!(user != null && user.Level == PermissionsLevel.CLIENT))
            {
                return;
            }

            Cart cart = user.Cart;

            if (cart.Lines.Count() == 0)
            {
                Response.Write("<span id='empty-label'>Корзина пуста</span>");
                return;
            }
            else
            {
                Response.Write("<span id='top-label'>Содержимое корзины</span>");
                Response.Write($"<p style='font-weight: bold'>Дата и время последнего обновления корзины:<span style='font-weight: normal'> {cart.LastUpdate:F}</span></p>");
            }

            Response.Write("<table>");

            Response.Write("<tr>");
            Response.Write("<th style='width: 20%'>Наименование товара</th>");
            Response.Write("<th style='width: 15%'>Категория товара</th>");
            Response.Write("<th style='width: 10%'>Цена товара</th>");
            Response.Write("<th style='width: 20%'>Количество товара на складе</th>");
            Response.Write("<th style='width: 20%'>Общая цена выбранных товаров</th>");
            Response.Write("<th style='width: 15%'>Действия с товаром</th>");
            Response.Write("</tr>");

            foreach (CartLine cartLine in cart.Lines)
            {
                Product product = cartLine.Product;

                Response.Write("<tr>");
                
                Response.Write($"<td style='width: 20%'>{product.Name}</td>");
                Response.Write($"<td style='width: 15%'>{product.Category.ToWebRepresentation()}</td>");
                Response.Write($"<td style='width: 10%'>{product.Price:c}</td>");
                Response.Write("<td id='quantity-column' style='width: 20%'>");
                Response.Write($"<span>{cartLine.Quantity}</span>");
                Response.Write($"<button id='increment-product' name='IncrementProduct' value='{product.ID}' type='submit'>+</button>");
                Response.Write($"<button id='decrement-product' name='DecrementProduct' value='{product.ID}' type='submit'>-</button>");
                Response.Write("</td>");
                Response.Write($"<td style='width: 20%'>{(product.Price * cartLine.Quantity):c}</td>");
                Response.Write($"<td style='width: 15%'><button id='delete-line' name='RemoveLine' value='{product.ID}' type='submit'>Удалить</button></td>");
                Response.Write("</tr>");
            }

            Response.Write("</table>");

            Response.Write($"<span id='total-label'>Полная стоимость заказа: {cart.TotalCost():c}</span>");

            string checkoutHref = RouteTable.Routes.GetVirtualPath(null, "checkout", null).VirtualPath;
            Response.Write($"<a id='order-ref' href='{checkoutHref}'>Перейти к оформлению заказа</a>");
            Response.Write("<button id='clear-button' name='Clear' type='submit'>Очистить корзину</button>");
        }
    }
}
