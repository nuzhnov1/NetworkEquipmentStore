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
            routes.MapPageRoute(null, "products/{category}/{page}", "~/Pages/ProductsPage.aspx");
            routes.MapPageRoute(null, "products/{page}", "~/Pages/ProductsPage.aspx");
            routes.MapPageRoute(null, "products", "~/Pages/ProductsPage.aspx");
            routes.MapPageRoute("authorization", "authorization", "~/Pages/AuthorizationPage.aspx");
            routes.MapPageRoute("registration", "registration", "~/Pages/RegistrationPage.aspx");
            routes.MapPageRoute("cart", "cart", "~/Pages/CartPage.aspx");
            routes.MapPageRoute("orders", "orders", "~/Pages/OrdersPage.aspx");
            routes.MapPageRoute("clients", "clients", "~/Pages/ClientsPage.aspx");
            routes.MapPageRoute("product", "product", "~/Pages/ProductPage.aspx");
            routes.MapPageRoute(null, "", "~/Pages/ProductsPage.aspx");
        }
    }
}