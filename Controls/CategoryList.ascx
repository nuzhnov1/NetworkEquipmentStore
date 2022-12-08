<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryList.ascx.cs" Inherits="NetworkEquipmentStore.Controls.CategoryList" %>
<%@ Import Namespace="NetworkEquipmentStore.Models" %>

<div id="products-categories">
    <h3>Категории товаров</h3>
    <ul>
        <% foreach (ProductCategory category in Categories)
           { %>
            <li id="category-item">
                <% CreateCategoryLink(category); %>
            </li>    
        <% } %>
    </ul>
</div>
