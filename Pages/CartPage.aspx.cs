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
        protected Repository repository = new Repository();
        protected Cart Cart => (Session != null) ? SessionHelper.GetCart(Session) : null;


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            if (user != null && user.Level == PermissionsLevel.CLIENT)
            {
                if (IsPostBack)
                {
                    if (Request.Form["IncrementProduct"] != null)
                    {
                        Cart.AddOneProduct(int.Parse(Request.Form["IncrementProduct"]));
                        repository.UpdateCart(Cart, user);
                    }
                    else if (Request.Form["DecrementProduct"] != null)
                    {
                        Cart.RemoveOneProduct(int.Parse(Request.Form["DecrementProduct"]));
                        repository.UpdateCart(Cart, user);
                    }
                    else if (Request.Form["RemoveLine"] != null)
                    {
                        Cart.RemoveLine(int.Parse(Request.Form["RemoveLine"]));
                        repository.UpdateCart(Cart, user);
                    }
                    else if (Request.Form["Clear"] != null)
                    {
                        Cart.Clear();
                        repository.UpdateCart(Cart, user);
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

            Cart cart = Cart;

            if (cart.Lines.Count() == 0)
            {
                Response.Write("<span id='empty-label'>Корзина пуста</span>");
                return;
            }

            Response.Write("<table id='cart-list'>");

            Response.Write("<tr>");
            Response.Write("<th>Категория товара</th>");
            Response.Write("<th>Цена товара</th>");
            Response.Write("<th>Количество товара на складе</th>");
            Response.Write("<th>Количество товара в корзине</th>");
            Response.Write("</tr>");

            foreach (ProductOrderInfo line in cart.Lines)
            {
                Product product = line.Product;

                Response.Write("<tr>");
                
                Response.Write($"<td>{product.Name}</td>");
                Response.Write($"<td>{product.Category.ToWebRepresentation()}</td>");
                Response.Write($"<td>{product.Price:c}</td>");
                Response.Write("<td id='quantity-column'>");
                Response.Write($"<span>{line.Quantity}</span>");
                Response.Write($"<button name='IncrementProduct' value='{product.ID}' type='submit'>+</button>");
                Response.Write($"<button name='DecrementProduct' value='{product.ID}' type='submit'>-</button>");
                Response.Write("</td>");
                Response.Write($"<td><button name='RemoveLine' value='{product.ID}' type='submit'>Удалить</button></td>");
                Response.Write("</tr>");
            }

            Response.Write("</table>");

            Response.Write("<div id='total-cost'>");
            Response.Write("<span>Полная стоимость заказа:</span>");
            Response.Write($"<span> {cart.TotalCost:c}</span>");
            Response.Write("</div>");

            string checkoutHref = RouteTable.Routes.GetVirtualPath(null, "checkout", null).VirtualPath;
            Response.Write($"<a href='{checkoutHref}'>Перейти к оформлению заказа</a>");
            Response.Write("<button name='Clear'>Очистить корзину</button>");
        }
    }
}
