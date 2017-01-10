<div class="added-to-cart">
	<div class="header">[%= localizeCartPopup('CartPopup_ProductAddedToCart')%]</div>
	<div class="added-product-b">
		<div class="added-product-photo">
			[%= Photo%]
		</div>
		<div class="added-product-item">
            <div class="added-product-content">
			    <a href="[%= Link%]" class="link-pv-name">[%= Name%]</a>
                <div class="added-product-options">
                    <div class="item-option">
                        <span class="opt-h">[%= Sku%]</span></div>
			        [% if(ColorName != null) { %]
                        <div class="item-option"> 
                            <div class="opt-h">[%= ColorHeader%]:</div> [%= ColorName%]</div>
                    [% } %]
                    [% if(SizeName != null) { %]
                        <div class="item-option">
                            <div class="opt-h">[%= SizeHeader%]:</div> [%= SizeName%]</div>
                    [% } %]
                    <div class="added-product-price">
                        [%= Price%]
                    </div>
                </div>
            </div>
            <div class="added-cart">
                <div class="added-cart-title">[%= localizeCartPopup('CartPopup_YourCart')%]</div>
                <div>[%= TotalCount%]</div>
                <div class="added-cart-sum">
                    <span>[%= localizeCartPopup('CartPopup_TotalAmount')%]:</span> <div class="price">[%= TotalPrice%]</div>
                </div>
                <a href="shoppingcart.aspx" class="btn btn-add btn-big">[%= localizeCartPopup('CartPopup_ShopCart')%]</a>
            </div>
            
			<div class="added-btns">
                <a href="javascript:void(0);" class="btn btn-add btn-big cart-close">[%= localizeCartPopup('CartPopup_ContinueShopping')%]</a>
				<a class="btn btn-confirm btn-big" href="orderconfirmation.aspx">[%= localizeCartPopup('CartPopup_Checkout')%]</a>
			</div>
		</div>
		
	</div>

	[% if(RelatedProducts.length > 0) { %]
    <div class="related-products-title">
        [%= RelatedTitle%]:
    </div>
	<ul class="jcarousel carousel-related-products">
		[% for(var i=0, arrLength = RelatedProducts.length; i<arrLength; i++) { %]
		<li>
            <table class="p-table">
                <tr>
                    <td class="img-middle">
                        <a class="pv-photo" href="[%= RelatedProducts[i].Link%]">
                            [%= RelatedProducts[i].Photo%]
                        </a>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            <a href="[%= RelatedProducts[i].Link%]" class="link-pv-name">[%= RelatedProducts[i].Name%]</a>
                        </div>
                        <div class="sku">[%= RelatedProducts[i].Sku%]</div>
                        <div class="price-container">
                            <div class='price'>[%= RelatedProducts[i].Price%]</div>
                        </div>
                        [%= RelatedProducts[i].Buttons%]
                    </td>
                </tr>
            </table>
        </li>
		[% } %]
	</ul>
	[% } %]
</div>