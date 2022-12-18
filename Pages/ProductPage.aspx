<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="ProductPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.ProductPage" %>
<%@ Import Namespace="NetworkEquipmentStore.Models" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="product-root">
        <% Product product = CurrentProduct; %>
        <h1><%= (product == null) ? "Добавление нового продукта" : $"Изменение продукта '{product.Name}'" %></h1>
        <div id="product-form">
            <input name="ProductID" type="hidden" value="<%= (product == null) ? "" : product.ID.ToString() %>" />
            <span>Название товара:</span>
            <input name="ProductName" type="text" placeholder='Наименование товара' required value="<%= (product == null) ? "" : product.Name %>" />
            <span>Категория товара:</span>
            <% CreateCategoriesList((product == null) ? ProductCategory.NONE : product.Category); %>
            <span>Изображение товара:</span>
            <img id="product-image" style="color: red" src="<%= (product == null) ? "" : $"/Content/images/{product.ImageName}.png" %>" alt="Изображение не загружено" />
            <input runat="server" id="ProductImageFile" type="file" accept="image/*" onchange="onReadImage();" />
            <span>Описание товара:</span>
            <textarea name="ProductDescription" placeholder="Описание товара" ><%= (product == null) ? "" : product.Description %></textarea>
            <span>Цена товара:</span>
            <input name="ProductPrice" type="text" placeholder="Цена товара" required value="<%= (product == null) ? "" : product.Price.ToString() %>" />
            <span>Количество товара на складе:</span>
            <input name="ProductQuantity" type="number" placeholder="Количество товара" required value="<%= (product == null) ? "" : product.Quantity.ToString() %>"/>
            <asp:Label runat="server" id="StatusLabel" />
            <button name="submit" type="submit"><%= (product == null) ? "Добавить продукт" : "Подтвердить изменения" %></button>
        </div>
    </div>
</asp:Content>
