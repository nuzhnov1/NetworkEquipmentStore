<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="CartPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.CartPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="content-root">
        <h1>Корзина товаров</h1>
        <div id="cart-root">
            <% CreateCartRoot(); %>
        </div>
    </div>
</asp:Content>
