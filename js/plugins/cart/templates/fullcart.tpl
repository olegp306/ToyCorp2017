[% if(CartProducts.length === 0) { %]
<div class="fullcart-empty">
    [%= localize('shoppingCartNoProducts')%]</div>
[% } %] 

[% if(CartProducts.length > 0) { %]
<h2 class="cartSectionTitle">Ваши товары</h2>
<table class="fullcart">
    <thead style="display:none">
        <tr>
            <th class="fullcart-name" colspan="2">
                [%= localize('shoppingCartName')%]
            </th>
            <th class="fullcart-price">
                [%= localize('shoppingCartPricePerUnit')%]
            </th>
            <th class="fullcart-count">
                [%= localize('shoppingCartCount')%]
            </th>
            <th class="fullcart-cost">
                [%= localize('shoppingCartCost')%]
            </th>
            <th class="fullcart-delete">
                <a href="javascript:void(0);" data-cart-clear="true" class="cross" title="[%= localize('shoppingCartClear')%]">
                </a>
            </th>
        </tr>
    </thead>
    <tbody>
        [% for(var i=0, arrLength = CartProducts.length; i< arrLength; i++) { %]
        <tr data-itemid="[%= CartProducts[i].ShoppingCartItemId%]">
            <td class="fullcart-photo-data">
                <a href="[%= CartProducts[i].Link%]">[%= CartProducts[i].Photo%]</a>
            </td>
            <td class="fullcart-name-data">
                <div>
                    <a href="[%= CartProducts[i].Link%]" class="link-pv-name">[%= CartProducts[i].Name%]</a></div>
					[%= CartProducts[i].Description%]
                [% if(CartProducts[i].ColorName != null) {%]
                <div> [%= ColorHeader + ': '  + CartProducts[i].ColorName%]</div>
                [% } %] 
                
                [% if(CartProducts[i].SizeName != null) {%]
                <div> [%= SizeHeader + ': ' + CartProducts[i].SizeName%]</div>
                [% } %] 
                
                [%= CartProducts[i].SelectedOptions%]
            </td>
            <td class="fullcart-price-data">
                <span class="price-wrap">[%= CartProducts[i].Price %]</span>
            </td>
            <td class="fullcart-count-data">
                [% if (CartProducts[i].CanOrderByRequest === true) { %] [%= CartProducts[i].Amount%]
                [% }else{ %] <span class="input-wrap cart-counter">
                    <input data-cart-itemcount="true" class="cart-counter-inp" data-plugin="spinbox" type="text" value="[%= CartProducts[i].Amount%]" data-spinbox-options="{min:[%= CartProducts[i].MinAmount%],max:[%=CartProducts[i].MaxAmount%],step:[%=CartProducts[i].Multiplicity%]}"><span class="sht">шт</span></span>
                [% } %]
                <div class="not-available cart-padding">
                    [%= CartProducts[i].Avalible%]</div>
            </td>
            <td class="fullcart-cost-data">
                <span class="price-wrap">[%= CartProducts[i].Cost%]</span>
            </td>
            <td class="fullcart-delete-data">
                <a href="javascript:void(0);" data-cart-remove="[%= CartProducts[i].ShoppingCartItemId%]" class="cross"
                    title="[%= localize('shoppingCartDeleteProduct') %]"></a>
            </td>
        </tr>
        [%}%] 
    </tbody>
</table>
   

<div class="bottomCartSectionVisible">
	<a onclick="showSkidka();" class="promoCodeExpander">Введите здесь код на скидку</a>
	<div class="totalAmount">             
			<span class="fullcart-summary-text">[%= Summary[Summary.length -1].Key%]: </span>
			<span class="price-wrap">[%= Summary[Summary.length -1].Value%]</span>
	</div>
</div>

<div class="clearfix cart-bonus-block" style="display:none;">
			[% if(BonusPlus != "") {%]
				<div class="fullcart-bonus-block">
					<div class="fullcart-bonus-content">
						[%= localize('shoppingBonusText')%]
						<div class="fullcart-bonus-number">+[%= BonusPlus%]</div>
					</div>
				</div>
			[%}%]
            <div class="newTotalAmount">
                [% for(var i=0, arrLength = Summary.length - 1; i< arrLength; i++) { %]
                    <span class="price-wrap">
                        [%= Summary[i].Value%]</span>
                [% } %]
            </div>
</div>

<div class="bottomCartSectionHidden">
<div class="clearfix cart-footer-result">
				<div class="cart-coupon-block">
				[% if(CouponInputVisible === true) { %]
                <div class="fullcart-cupon-inputs">
                    <div class="input-wrap input-coupon">
                        <input type="text" class="" id="txtCertificateCoupon"></div>
                    <span class="btn-c"><a class="promoCodeButton purpleButton" href="javascript:void(0);"
                        data-cart-apply-cert-cupon="#txtCertificateCoupon">[%= localize('shoppingCartAplly')%]</a></span>
                </div>
                [% } %]
				
               [% if(Valid.length > 0) { %]
                <div id="errorMessage" class="cart-err">
                    [%= Valid%]
                </div>
               [% } %]


			   </div>
</div>
</div>

[% } %]