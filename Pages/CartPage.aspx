<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="CartPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.CartPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="cart-root">
        <h1>Корзина товаров</h1>
        <% CreateCartRoot(); %>
        <asp:Label runat="server" ID="ErrorLabel" />
    </div>
</asp:Content>
