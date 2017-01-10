var langCartPopup = new Array();

langCartPopup['CartPopup_ProductAddedToCart'] = 'Product added to cart';
langCartPopup['CartPopup_YourCart'] = 'Your cart';
langCartPopup['CartPopup_TotalAmount'] = 'Total';
langCartPopup['CartPopup_ShopCart'] = 'View cart';
langCartPopup['CartPopup_ContinueShopping'] = 'Continue shopping';
langCartPopup['CartPopup_Checkout'] = 'Checkout';

function localizeCartPopup(param) {
    var p = param.toString();
    return langCartPopup[p] || '<span style="color:red;">NOT RESOURCED</span>';
};