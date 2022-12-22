<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="ClientsPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.ClientsPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="content-root">
        <h1>Список пользователей</h1>
        <div id="users-root">
            <% CreateUsersRoot(); %>
        </div>
    </div>
</asp:Content>
