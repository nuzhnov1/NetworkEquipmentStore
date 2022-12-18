﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="RegistrationPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.RegistrationPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="auth-root">
        <div id="auth-form">
            <span>Имя пользователя</span>
            <input runat="server" id="Username" type="text" placeholder="Введите имя пользователя" required="required" />
            <span>Логин</span>
            <input runat="server" id="Login" type="text" placeholder="Введите логин" required="required" />
            <span>Пароль</span>
            <input runat="server" id="Password" type="password" placeholder="Введите пароль" required="required" />
            <span>Подтвердите пароль</span>
            <input runat="server" id="PasswordConfirm" type="password" placeholder="Подтвердите пароль" required="required" />
            <asp:Label runat="server" ID="ErrorLabel" />
            <asp:Button runat="server" ID="RegistrationButton" Text="Зарегистрироваться" UseSubmitBehavior="true" OnClick="RegistrationButtonClick" />
        </div>
    </div>
</asp:Content>
