<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="OrdersPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.OrdersPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="content-root">
        <% CreateTitle(); %>
        <% CreatePagesList(); %>
        <% CreateOrdersList(); %>
        <% CreatePagesList(); %>
        <% CreateClearButton(); %>
    </div>
</asp:Content>
