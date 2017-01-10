var langCartPopup = new Array();

langCartPopup['CartPopup_ProductAddedToCart'] = 'Товар добавлен в корзину';
langCartPopup['CartPopup_YourCart'] = 'Ваша корзина';
langCartPopup['CartPopup_TotalAmount'] = 'Итого';
langCartPopup['CartPopup_ShopCart'] = 'Корзина';
langCartPopup['CartPopup_ContinueShopping'] = 'Продолжить покупки';
langCartPopup['CartPopup_Checkout'] = 'Оформить заказ';


function localizeCartPopup(param) {
    var p = param.toString();
    return langCartPopup[p] || '<span style="color:red;">NOT RESOURCED</span>';
};