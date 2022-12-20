<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="ClientsPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.ClientsPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="users-root">
        <h1>Список пользователей</h1>
        <% CreateUsersRoot(); %>
    </div>
</asp:Content>
