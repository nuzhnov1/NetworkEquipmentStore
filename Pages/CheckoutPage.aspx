<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="CheckoutPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.CheckoutPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="content-root">
        <div runat="server" id="CheckoutForm" class="checkout-form">
            <h1>Оформление заказа</h1>
            <span>Введите вашу фамилию, имя и отчество:</span>
            <input runat="server" id="Name" type="text" placeholder="ФИО" required />
            <span>Введите ваш номер телефона:</span>
            <input runat="server" id="Phone" type="tel" placeholder="Номер телефона: 79993337689" required />
            <span>Введите вашу электронную почту:</span>
            <input runat="server" id="Email" type="email" placeholder="Электронная почта: example@mail.ru" required />
            <span>Введите адрес, куда мы доставим ваш заказ:</span>
            <input runat="server" id="Address" type="text" placeholder="Адрес доставки" required />
            <asp:Label runat="server" ID="ErrorLabel" />
            <button name="SubmitOrder" type="submit">Подтвердить заказ</button>
        </div>
        <div runat="server" id="PostbackForm" class="postback-form">
            <h1>Благодарим за покупку!</h1>
            <span>Ваш заказ будет обработан администратором, который свяжется с вами по указанным данным для уточнения доставки.</span>
            <span>Удачных покупок!</span>
        </div>
    </div>
</asp:Content>
