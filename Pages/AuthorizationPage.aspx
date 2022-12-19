<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="AuthorizationPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.AuthorizationPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="auth-root">
        <div id="auth-form">
            <span>Логин</span>
            <input runat="server" id="Login" type="text" placeholder="Логин" required="required" />
            <span>Пароль</span>
            <input runat="server" id="Password" type="password" placeholder="Пароль" required="required" />
            <asp:Label runat="server" ID="ErrorLabel" />
            <asp:Button runat="server" ID="LoginButton" Text="Войти" UseSubmitBehavior="true" OnClick="OnLogin" />
            <a runat="server" id="RegisterLink">Зарегистрироваться</a>
        </div>
    </div>
</asp:Content>
