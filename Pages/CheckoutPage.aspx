<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/NetworkEquipmentStore.Master" CodeBehind="CheckoutPage.aspx.cs" Inherits="NetworkEquipmentStore.Pages.CheckoutPage" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div id="checkout-root">
        <div runat="server" id="CheckoutForm">
            <h1>Оформление заказа</h1>
            <input runat="server" id="Name" type="text" placeholder="ФИО" required />
            <input runat="server" id="Phone" type="tel" placeholder="Номер телефона: 79993337689" required />
            <input runat="server" id="Email" type="email" placeholder="Электронная почта: example@mail.ru" required />
            <input runat="server" id="Address" type="text" placeholder="Адрес доставки" required />
            <asp:Label runat="server" ID="ErrorLabel" />
            <button name="SubmitOrder" type="submit">Подтвердить заказ</button>
        </div>
        <div runat="server" id="PostbackForm">
            <h1>Благодарим за покупку!</h1>
            <span>Ваш заказ будет обработан администратором, который свяжется с вами по указанным данным для уточнения доставки.</span>
            <span>Удачных покупок!</span>
        </div>
    </div>
</asp:Content>
