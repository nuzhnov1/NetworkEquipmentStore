using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.Routing;
using System.Web.UI;
using NetworkEquipmentStore.Models;
using NetworkEquipmentStore.Models.Repository;
using NetworkEquipmentStore.Pages.Helpers;

namespace NetworkEquipmentStore.Pages
{
    public partial class ProductPage : Page
    {
        const int MAX_IMAGE_SIZE = 1024 * 1024 * 1;  // Максимальный размер изображения - 1 MB



        private readonly Repository repository = new Repository();
        
        private int? CurrentProductID
        {
            get
            {
                string reqValue = (string)RouteData.Values["id"] ?? Request.QueryString["id"];

                if (reqValue != null)
                {
                    if (int.TryParse(reqValue, out int temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        private Product _currentProduct = null;
        protected Product CurrentProduct
        {
            get
            {
                if (_currentProduct == null)
                {
                    int? productID = CurrentProductID;

                    if (productID == null)
                    {
                        _currentProduct = null;
                    }
                    else
                    {
                        _currentProduct = repository.GetProductByID((int)productID);
                    }
                }

                return _currentProduct;
            }
        }

        

        protected void Page_Load(object sender, EventArgs e)
        {
            User user = SessionHelper.GetUser(Session);

            // Если пользователь - не администратор, то перенаправляем его на главную страницу
            if (user != null && user.Level == PermissionsLevel.ADMIN)
            {
                // Если идёт добавления нового товара, то добавление изображения обязательно
                if (CurrentProductID == null)
                {
                    ProductImageFile.Attributes.Add("required", "");
                }

                if (IsPostBack)
                {
                    if (Request.Form["SubmitProduct"] != null && IsValidProductForm())
                    {
                        // Если добавляется новый товар
                        if (CurrentProduct == null)
                        {
                            OnInsertProduct();
                            Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
                        }
                        // Если обновляется существующий
                        else
                        {
                            OnUpdateProduct();
                        }
                    }
                }
            }
            else
            {
                Response.RedirectPermanent(RouteTable.Routes.GetVirtualPath(null, null).VirtualPath);
            }
        }

        protected void CreateCategoriesList(ProductCategory choose = ProductCategory.NONE)
        {
            IEnumerable<ProductCategory> categories = Enum.GetValues(typeof(ProductCategory)).OfType<ProductCategory>();

            Response.Write("<select name='ProductCategory' required>");
            foreach (ProductCategory category in categories)
            {
                if (category == ProductCategory.NONE)
                {
                    continue;
                }

                string selectedOrNot = (choose == category) ? "selected" : "";
                string text = category.ToWebRepresentation();

                Response.Write($"<option {selectedOrNot} value='{category}'>{text}</option>");
            }
            Response.Write("</select>");
        }

        private void OnInsertProduct()
        {
            string name = Request.Form["ProductName"];
            ProductCategory category = (ProductCategory)Enum.Parse(typeof(ProductCategory), Request.Form["ProductCategory"]);
            string imageName = SaveFile(ProductImageFile.PostedFile);
            string description = Request.Form["ProductDescription"];
            decimal price = decimal.Parse(Request.Form["ProductPrice"]);
            int quantity = int.Parse(Request.Form["ProductQuantity"]);

            Product newProduct = new Product
            {
                ID = 0,
                Name = name,
                Category = category,
                Description = description,
                ImageName = imageName,
                Price = price,
                Quantity = quantity
            };

            repository.InsertProduct(newProduct);
        }

        private void OnUpdateProduct()
        {
            int id = int.Parse(Request.Form["ProductID"]);
            string name = Request.Form["ProductName"];
            ProductCategory category = (ProductCategory)Enum.Parse(typeof(ProductCategory), Request.Form["ProductCategory"]);

            HttpPostedFile postedImage = ProductImageFile.PostedFile;
            string imageName;
            if (postedImage.ContentLength == 0)
            {
                imageName = repository.GetProductByID(id).ImageName;
            }
            else
            {
                imageName = SaveFile(postedImage);
            }

            string description = Request.Form["ProductDescription"];
            decimal price = decimal.Parse(Request.Form["ProductPrice"]);
            int quantity = int.Parse(Request.Form["ProductQuantity"]);


            Product updatedProduct = new Product
            {
                ID = id,
                Name = name,
                Category = category,
                Description = description,
                ImageName = imageName,
                Price = price,
                Quantity = quantity
            };

            repository.UpdateProduct(updatedProduct);
            ShowStatus("Продукт успешно обновлён!");
        }

        private string SaveFile(HttpPostedFile imageFile)
        {
            string imagesDir = AppDomain.CurrentDomain.BaseDirectory + @"Content\images";
            string[] filenames = Directory.GetFiles(imagesDir);
            string newName = imageFile.FileName;

            // Генерируем уникальное имя для файла изображения
            while (filenames.Contains($"{imagesDir}\\{newName}"))
            {
                newName = $"{Guid.NewGuid()}.png";
            }

            // Создаём файл изображения
            string newFilePath = $"{imagesDir}\\{newName}";
            FileStream newImageFile = File.Create(newFilePath);

            // Сохраняем изображение в новый файл
            imageFile.InputStream.CopyTo(newImageFile);
            newImageFile.Close();

            return newName;
        }


        private bool IsValidProductForm() =>
            IsNameValid() && IsImageValid() &&
            IsValidDescription() && IsValidPrice() &&
            IsValidQuantity();

        private bool IsNameValid()
        {
            string name = Request.Form["ProductName"];

            if (name.Length == 0)
            {
                ShowError("имя товара не может быть пустым");
                return false;
            }
            else if (name.Length > 60)
            {
                ShowError("имя товара не должно превышать 60 символов");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsImageValid()
        {
            HttpPostedFile postedImage = ProductImageFile.PostedFile;

            if (postedImage.ContentLength > MAX_IMAGE_SIZE)
            {
                ShowError("превышен максимальный размер изображения в 1 МБ");
                return false;
            }
            else if (postedImage.FileName.Length > 60)
            {
                ShowError($"превышено максимальное число символов в названии изображения в 60 символов");
                return false;
            }
            else if (!IsValidFileName(postedImage.FileName))
            {
                ShowError("в названии файла изображения присутствуют недопустимые символы");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsValidFileName(string filename) => filename.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;

        private bool IsValidDescription()
        {
            string description = Request.Form["ProductDescription"];

            if (description.Length > 20_000)
            {
                ShowError("описание товара превышает максимально допустимое число символов в 20 000");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsValidPrice()
        {
            if (!decimal.TryParse(Request.Form["ProductPrice"], out decimal value))
            {
                ShowError("неверное значение цены товара");
                return false;
            }
            else if (value < 0)
            {
                ShowError("цена товара не может быть отрицательной");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsValidQuantity()
        {
            if (!int.TryParse(Request.Form["ProductQuantity"], out int value))
            {
                ShowError("неверное значение количества товаров");
                return false;
            }
            else if (value < 0)
            {
                ShowError("количество товаров не может быть отрицательным");
                return false;
            }
            else
            {
                return true;
            }
        }


        private void ShowStatus(string status)
        {
            StatusLabel.Text = status;
            StatusLabel.ForeColor = Color.Blue;
        }

        private void ShowError(string message)
        {
            StatusLabel.Text = $"Ошибка: {message}!";
            StatusLabel.ForeColor = Color.Red;
        }
    }
}
