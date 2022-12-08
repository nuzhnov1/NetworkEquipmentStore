<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="Products.aspx.cs" Inherits="NetworkEquipmentStore.Pages.Products" %>
<%@ Import Namespace="NetworkEquipmentStore.Models" %>
<%@ Import Namespace="NetworkEquipmentStore.Pages.Helpers" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="products-root">
        <GS:CategoryList runat="server" />
        <div id="products-container">
            <h1>Список товаров</h1>
            <% CreatePagesList(); %>
            <% CreateProductsList(); %>
            <% CreatePagesList(); %>
        </div>
    </div>
</asp:Content>
