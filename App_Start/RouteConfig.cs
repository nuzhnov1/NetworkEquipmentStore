using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace NetworkEquipmentStore.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute(null, "products/{category}/{page}", "~/Pages/Products.aspx");
            routes.MapPageRoute(null, "products/{page}", "~/Pages/Products.aspx");
            routes.MapPageRoute(null, "products", "~/Pages/Products.aspx");
            routes.MapPageRoute("authorization", "authorization", "~/Pages/Authorization.aspx");
            routes.MapPageRoute("registration", "registration", "~/Pages/Registration.aspx");
            routes.MapPageRoute("cart", "cart", "~/Pages/Cart.aspx");
            routes.MapPageRoute("orders", "orders", "~/Pages/Orders.aspx");
            routes.MapPageRoute("clients", "clients", "~/Pages/Clients.aspx");
            routes.MapPageRoute("product", "product", "~/Pages/Product.aspx");
            routes.MapPageRoute(null, "", "~/Pages/Products.aspx");
        }
    }
}