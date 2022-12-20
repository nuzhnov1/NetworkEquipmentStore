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

            if (user != null && user.Level == PermissionsLevel.CLIENT)
            {
                if (IsPostBack)
                {
                    Cart cart = user.Cart;

                    if (Request.Form["IncrementProduct"] != null)
                    {
                        cart.AddOneProduct(int.Parse(Request.Form["IncrementProduct"]));
                        repository.UpdateUser(user);
                    }
                    else if (Request.Form["DecrementProduct"] != null)
                    {
                        cart.RemoveOneProduct(int.Parse(Request.Form["DecrementProduct"]));
                        repository.UpdateUser(user);
                    }
                    else if (Request.Form["RemoveLine"] != null)
                    {
                        cart.RemoveLine(int.Parse(Request.Form["RemoveLine"]));
                        repository.UpdateUser(user);
                    }
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
                Response.Write("<span id='top-label'>Корзина пуста</span>");
                return;
            } else
            {
                Response.Write("<span id='top-label'>Содержимое корзины:</span>");
                Response.Write($"<p style='font-weight: bold'>Дата и время последнего обновления корзины:<span style='font-weight: normal'> {cart.LastUpdate:F}</span></p>");
            }

            Response.Write("<table id='cart-list'>");

            Response.Write("<tr>");
            Response.Write("<th>Категория товара</th>");
            Response.Write("<th>Цена товара</th>");
            Response.Write("<th>Количество товара на складе</th>");
            Response.Write("<th>Количество товара в корзине</th>");
            Response.Write("</tr>");

            foreach (CartLine cartLine in cart.Lines)
            {
                Product product = cartLine.Product;

                Response.Write("<tr>");
                
                Response.Write($"<td>{product.Name}</td>");
                Response.Write($"<td>{product.Category.ToWebRepresentation()}</td>");
                Response.Write($"<td>{product.Price:c}</td>");
                Response.Write("<td id='quantity-column'>");
                Response.Write($"<span>{cartLine.Quantity}</span>");
                Response.Write($"<button name='IncrementProduct' value='{product.ID}' type='submit'>+</button>");
                Response.Write($"<button name='DecrementProduct' value='{product.ID}' type='submit'>-</button>");
                Response.Write("</td>");
                Response.Write($"<td><button name='RemoveLine' value='{product.ID}' type='submit'>Удалить</button></td>");
                Response.Write("</tr>");
            }

            Response.Write("</table>");

            Response.Write("<div id='total-cost'>");
            Response.Write($"<span>Полная стоимость заказа: {cart.TotalCost():c}</span>");
            Response.Write("</div>");

            string checkoutHref = RouteTable.Routes.GetVirtualPath(null, "checkout", null).VirtualPath;
            Response.Write($"<a href='{checkoutHref}'>Перейти к оформлению заказа</a>");
            Response.Write("<button name='Clear'>Очистить корзину</button>");
        }
    }
}
