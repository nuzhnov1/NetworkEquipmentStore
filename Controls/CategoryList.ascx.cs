using NetworkEquipmentStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.UI;

namespace NetworkEquipmentStore.Controls
{
    public partial class CategoryList : UserControl
    {
        protected readonly IEnumerable<ProductCategory> Categories = Enum.GetValues(typeof(ProductCategory)).OfType<ProductCategory>();


        protected void CreateCategoryLink(ProductCategory category)
        {
            string selectedCategory = (string)Page.RouteData.Values["category"] ?? Request.QueryString["category"];
            string urlCategory = category.ToUrlRepresentation();
            string htmlCategory = category.ToWebRepresentation();
            string path;

            if (category == ProductCategory.NONE)
            {
                path = RouteTable.Routes.GetVirtualPath(null, null).VirtualPath;
            }
            else
            {
                path = RouteTable.Routes.GetVirtualPath(null, null,
                    new RouteValueDictionary() { { "category", urlCategory }, { "page", 1 } }).VirtualPath;
            }


            if ((category == ProductCategory.NONE && selectedCategory == null) || urlCategory == selectedCategory)
            {
                Response.Write($"<a href='javascript:void(0)' class='category-selected'>{htmlCategory}</a>");
            }
            else
            {
                Response.Write($"<a href='{path}'>{htmlCategory}</a>");
            }
        }
    }
}
