var langCartPopup = new Array();

langCartPopup['CartPopup_ProductAddedToCart'] = 'Товар доданий в кошик';
langCartPopup['CartPopup_YourCart'] = 'Ваш кошик';
langCartPopup['CartPopup_TotalAmount'] = 'Разом';
langCartPopup['CartPopup_ShopCart'] = 'Кошик';
langCartPopup['CartPopup_ContinueShopping'] = 'Продовжити покупки';
langCartPopup['CartPopup_Checkout'] = 'Оформити замовлення';

function localizeCartPopup(param) {
    var p = param.toString();
    return langCartPopup[p] || '<span style="color:red;">NOT RESOURCED</span>';
};