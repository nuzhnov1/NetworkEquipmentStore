using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.Routing;
using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;

namespace NetworkEquipmentStore.Pages
{
    public partial class ProductsPage : Page
    {
        private const int PAGE_COUNT = 5;
        private const int PRODUCTS_COUNT = 5;


        private readonly Repository repository = new Repository();

        private ProductCategory? _selectedCategory = null;
        private ProductCategory SelectedCategory
        {
            get
            {
                if (_selectedCategory == null)
                {
                    string reqValue = (string)RouteData.Values["category"] ?? Request.QueryString["category"];
                    _selectedCategory = reqValue.ToProductCategoryFromUrl();
                }

                return (ProductCategory)_selectedCategory;
            }
        }

        private int? _selectedPage = null;
        private int SelectedPage
        {
            get
            {
                if (_selectedCategory == null)
                {
                    string reqValue = (string)RouteData.Values["page"] ?? Request.QueryString["page"];
                    int page = reqValue != null && int.TryParse(reqValue, out page) ? page : 1;

                    if (page > 0)
                    {
                        _selectedPage = page > LastPage ? LastPage : page;
                    }
                    else
                    {
                        Response.RedirectPermanent(GetVirtualPath(SelectedCategory, 1));
                        _selectedPage = 1;
                    }
                }

                return (int)_selectedPage;
            }
        }

        private int? _lastPage = null;
        private int LastPage  // Последняя страница для выбранных товаров данной категории
        {
            get
            {
                if (_lastPage == null)
                {
                    _lastPage = (int)Math.Ceiling((decimal)repository.GetAllProductsCountByCategory(SelectedCategory) / PRODUCTS_COUNT);
                }

                return (int)_lastPage;
            }
        }

        private IEnumerable<Product> _selectedProducts = null;
        private IEnumerable<Product> SelectedProducts
        {
            get
            {
                if (_selectedProducts == null)
                {
                    // Выбираем товары данной категории на данной странице
                    _selectedProducts = repository.GetAllProducts()
                        .Where(item => SelectedCategory == ProductCategory.NONE || item.Category == SelectedCategory)
                        .OrderBy(item => item.ID)
                        .Skip((SelectedPage - 1) * PRODUCTS_COUNT)
                        .Take(PRODUCTS_COUNT);
                }

                return _selectedProducts;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пользователь авторизован и от него пришёл ответ
            if (IsPostBack && user != null)
            {
                if (user.Level == PermissionsLevel.CLIENT)
                {
                    if (int.TryParse(Request.Form["AddToCart"], out int selectedProductID))
                    {
                        user.Cart.AddLine(new CartLine
                        {
                            Product = repository.GetProductByID(selectedProductID),
                            Quantity = 1
                        });
                        repository.UpdateUser(user);
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
            int selectedPage = SelectedPage;
            int maxPage = LastPage;
            ProductCategory selectedCategory = SelectedCategory;

            if (maxPage == 0)
            {
                return;
            }

            Response.Write("<div id='pages'>");
            Response.Write("<span>Страницы: </span>");

            if (selectedPage > PAGE_COUNT)
            {
                string path = GetVirtualPath(selectedCategory, 1);
                Response.Write($"<a href='{path}'>В начало</a>");
            }

            if (selectedPage > 1)
            {
                string path = GetVirtualPath(selectedCategory, selectedPage - 1);
                Response.Write($"<a href='{path}'>Назад</a>");
            }

            int startPage = selectedPage - (selectedPage - 1) % PAGE_COUNT;
            int endPage = Math.Min(startPage + PAGE_COUNT, maxPage);
            for (int i = startPage; i <= endPage; i++)
            {
                string path = GetVirtualPath(selectedCategory, i);

                if (i == selectedPage)
                {
                    Response.Write($"<a href='javascript:void(0)' class='page-selected'>{i}</a>");
                }
                else
                {
                    Response.Write($"<a href='{path}'>{i}</a>");
                }
            }

            if (selectedPage < maxPage)
            {
                string path = GetVirtualPath(selectedCategory, selectedPage + 1);
                Response.Write($"<a href='{path}'>Вперёд</a>");
            }

            if (selectedPage <= maxPage - PAGE_COUNT)
            {
                string path = GetVirtualPath(selectedCategory, maxPage);
                Response.Write($"<a href='{path}'>В конец</a>");
            }

            Response.Write("</div>");
        }

        protected void CreateProductsList()
        {
            User user = SessionHelper.GetUser(Session);

            foreach (Product product in SelectedProducts)
            {
                Response.Write("<div id='list-item'>");
                Response.Write($"<h3>{product.Name}</h3>");
                Response.Write($"<p style='font-weight: bold'>Категория товара:<span style='font-weight: normal'> {product.Category.ToWebRepresentation()}</span></p>");
                Response.Write($"<img style='color: red' src='/Content/images/{product.ImageName}' alt='Изображение не загружено' />");
                Response.Write($"<p style='font-weight: bold'>Описание товара:<span style='font-weight: normal'> {product.Description}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Цена товара:<span style='font-weight: normal'> {product.Price:c}</span></p>");
                Response.Write($"<p style='font-weight: bold'>Количество товара на складе:<span style='font-weight: normal'> {product.Quantity}</span></p>");
                
                if (user != null)
                {
                    if (user.Level == PermissionsLevel.CLIENT)
                    {
                        bool isProductInChart = user.Cart.IsInCart(product);

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
                        Response.Write($"<a id='update-product' href='{updatePath}'>Обновить товар</a>");
                        Response.Write($"<button id='delete-product' name='RemoveProduct' value='{product.ID}' type='submit'>Удалить товар</button>");
                        Response.Write("</div>");
                    }
                }

                Response.Write("</div>");
            }

            if (SelectedProducts.Count() == 0)
            {
                Response.Write("<span id='empty-label'>Список товаров пуст</span>");
            }
        }
    }
}
