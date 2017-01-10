<span class="minicart-text">
  <div class="h-goodsCount">
    <span>[%= Count%]</span>
  </div>
  [% for(var i=0, arrLength = Summary.length; i< arrLength; i++) { %]
          <div class="h-goodsPrice">[%= Summary[i].Value%]</div>
  [% } %]
</span>
<div class="minicart-list">
    <div data-plugin="scrollbar" data-cart-scrollbar="{height: 305}">
        <ul class="minicart-body">
            [% for(var i=0, arrLength = CartProducts.length; i< arrLength; i++) { %]
            <li class="minicart-item">
                <figure class="minicart-photo">
                    <a href="[%= CartProducts[i].Link%]">[%= CartProducts[i].Photo%] </a>
                </figure>
                <div class="minicart-info">
                    <div class="minicart-name">
                        <a href="[%= CartProducts[i].Link%]" class="minicart-name">[%= CartProducts[i].Name%]</a>
                    </div>
                    <div class="minicart-count">
                        <span class="minicart-param-name">[%= localize('shoppingCartAmount')%]</span>: [%=
                        CartProducts[i].Amount%]
                    </div>
                    <div class="minicart-price">
                        <span class="minicart-param-name">[%= localize("shoppingCartPrice")%]</span>: [%=
                        CartProducts[i].Price%]
                    </div>
                </div>
            </li>
            [% } %]
        </ul>
    </div>
    <div class="minicart-result">
        <div class="clearfix">
            <div class="minicart-result-block">
                [% for(var i=0, arrLength = Summary.length; i< arrLength; i++) { %]
        <div class="minicart-result-row">
                    <span class="minicart-result-name">[%= Summary[i].Key%]</span>: [%= Summary[i].Value%]
                </div>
                [% } %]

                <!--<div class="minicart-btns">
                    [%if(ShowConfirmButtons){%]
                    <span class="btn-c">
                        <a href="shoppingcart.aspx" class="btn btn-confirm btn-middle btn-cart-margin">
                            [%= localize('shoppingCartBtn')%]
                        </a>
                    </span>
                    <a href="orderconfirmation.aspx" class="minicart-link-confirm">
                        [%= localize('shoppingCartOrderConfirmation')%]
                    </a>
                    [%}else{%]
                    <a href="shoppingcart.aspx" class="minicart-link-confirm">
                        [%= localize('shoppingCartOrderConfirmation')%]
                    </a>
                    [%}%]
                </div>-->

            </div>

        </div>
    </div>
</div>
