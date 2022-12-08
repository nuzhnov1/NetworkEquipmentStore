using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NetworkEquipmentStore.Pages
{
    using Models;
    using Models.Repository;
    using NetworkEquipmentStore.Pages.Helpers;
    using System.Web.Routing;


    public partial class Products : Page
    {
        protected const int PAGE_COUNT = 5;
        protected const int PRODUCT_COUNT = 5;


        private readonly Repository repository = new Repository();

        private ProductCategory CurrentCategory
        {
            get
            {
                string reqValue = (string)RouteData.Values["category"] ?? Request.QueryString["category"];
                return reqValue.ToProductCategoryFromUrl();
            }
        }

        private int CurrentPage
        {
            get
            {
                string reqValue = (string)RouteData.Values["page"] ?? Request.QueryString["page"];
                int page = reqValue != null && int.TryParse(reqValue, out page) ? page : 1;

                if (page > 0)
                {
                    return page > MaxPage ? MaxPage : page;
                }
                else
                {
                    ProductCategory category = CurrentCategory;
                    string path;

                    if (category != ProductCategory.NONE)
                    {
                        path = RouteTable.Routes.GetVirtualPath(null, null,
                            new RouteValueDictionary() { { "category", category }, { "page", 1 } }).VirtualPath;
                    }
                    else
                    {
                        path = RouteTable.Routes.GetVirtualPath(null, null).VirtualPath;
                    }

                    Response.RedirectPermanent(path);
                    return 1;
                }
            }
        }

        protected int MaxPage => (int)Math.Ceiling((decimal)repository.GetAllProductsCountByCategory(CurrentCategory) / PRODUCT_COUNT);

        private IEnumerable<Product> CurrentProducts => repository.GetAllProducts()
            .Where(item => CurrentCategory == ProductCategory.NONE || item.Category == CurrentCategory)
            .OrderBy(item => item.ID)
            .Skip((CurrentPage - 1) * PRODUCT_COUNT)
            .Take(PRODUCT_COUNT);

        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            if (IsPostBack && user != null)
            {
                if (user.Level == PermissionsLevel.CLIENT)
                {
                    if (int.TryParse(Request.Form["AddToCart"], out int selectedProductID))
                    {
                        Cart cart = SessionHelper.GetCart(Session);
                        Product product = repository.GetProductByID(selectedProductID);

                        cart.AddLine(product, 1);
                    }
                }
                else
                {
                    if (int.TryParse(Request.Form["RemoveProduct"], out int selectedProductID))
                    {
                        repository.DeleteProductByID(selectedProductID);
                    }
                }
            }
        }

        private string GetVirtualPath(ProductCategory category, int page)
        {
            if (category != ProductCategory.NONE)
            {
                return RouteTable.Routes.GetVirtualPath(null, null,
                    new RouteValueDictionary() { 
                        { "category", category.ToUrlRepresentation() }, { "page", page } 
                    }).VirtualPath;
            }
            else
            {
                if (page != 1)
                {
                    return RouteTable.Routes.GetVirtualPath(null, null,
                        new RouteValueDictionary() { { "page", page } }).VirtualPath;
                }
                else
                {
                    return RouteTable.Routes.GetVirtualPath(null, null).VirtualPath;
                }
            }
        }

        protected void CreatePagesList()
        {
            int currentPage = CurrentPage;
            int maxPage = MaxPage;
            ProductCategory currentCategory = CurrentCategory;

            if (maxPage == 0)
            {
                return;
            }

            Response.Write("<div id='products-pages'>");
            Response.Write("<span>Страницы: </span>");

            if (currentPage > PAGE_COUNT)
            {
                string path = GetVirtualPath(currentCategory, 1);
                Response.Write($"<a href='{path}'>В начало</a>");
            }

            if (currentPage > 1)
            {
                string path = GetVirtualPath(currentCategory, currentPage - 1);
                Response.Write($"<a href='{path}'>Назад</a>");
            }

            int startPage = currentPage - (currentPage - 1) % PAGE_COUNT;
            int endPage = Math.Min(startPage + PAGE_COUNT, maxPage);
            for (int i = startPage; i <= endPage; i++)
            {
                string path = GetVirtualPath(currentCategory, i);

                if (i == currentPage)
                {
                    Response.Write($"<a href='javascript:void(0)' class='page-selected'>{i}</a>");
                }
                else
                {
                    Response.Write($"<a href='{path}'>{i}</a>");
                }
            }

            if (currentPage < maxPage)
            {
                string path = GetVirtualPath(currentCategory, currentPage + 1);
                Response.Write($"<a href='{path}'>Вперёд</a>");
            }

            if (currentPage <= maxPage - PAGE_COUNT)
            {
                string path = GetVirtualPath(currentCategory, maxPage);
                Response.Write($"<a href='{path}'>В конец</a>");
            }

            Response.Write("</div>");
        }

        protected void CreateProductsList()
        {
            User user = SessionHelper.GetUser(Session);

            foreach (Product product in CurrentProducts)
            {
                Response.Write("<div id='product-item'>");
                Response.Write($"<h3>{product.Name}</h3>");
                Response.Write($"<p style='font-weight: bold'>Категория товара:<span style='font-weight: normal'> {product.Category.ToWebRepresentation()}</span></p>");
                Response.Write($"<img style='color: red' src='{product.ImagePath}' alt='Изображение не загружено' />");
                Response.Write($"<p style='font-weight: bold'>Описание товара:<span style='font-weight: normal'> {product.Description}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Цена товара:<span style='font-weight: normal'> {product.Price:c}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Количество товара на складе:<span style='font-weight: normal'> {product.Quantity}</span></p>");
                
                if (user != null)
                {
                    if (user.Level == PermissionsLevel.CLIENT)
                    {
                        bool isProductInChart = SessionHelper.GetCart(Session).IsInCart(product);

                        if (isProductInChart)
                        {
                            Response.Write("<span class='product-added'>Товар в корзине</span>");
                        }
                        else
                        {
                            Response.Write($"<button name='AddToCart' value='{product.ID}' type='submit'>Добавить в корзину</button>");
                        }
                    }
                    else
                    {
                        string updatePath = RouteTable.Routes.GetVirtualPath(null, "product",
                            new RouteValueDictionary() { { "id", product.ID } }).VirtualPath;

                        Response.Write("<div id='action-buttons'>");
                        Response.Write($"<a href='{updatePath}'>Обновить товар</a>");
                        Response.Write($"<button name='RemoveProduct' value='{product.ID}' type='submit'>Удалить товар</button>");
                        Response.Write("</div>");
                    }
                }

                Response.Write("</div>");
            }
        }
    }
}
