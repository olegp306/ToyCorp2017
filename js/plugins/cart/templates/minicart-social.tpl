[%= localize('shoppingCartTitle')%]: <span class="minicart-count">[%= Count%]</span>
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
        [% for(var i=0, arrLength = Summary.length; i< arrLength; i++) { %]
        <div class="minicart-result-row">
            [%= Summary[i].Key%]: [%= Summary[i].Value%]
        </div>
        [% } %]
        <div class="minicart-btns">
            <span class="btn-c"><a href="social/shoppingcartsocial.aspx" class="btn btn-add btn-small btn-cart-margin">
                [%= localize('shoppingCartBtn')%]</a></span>
        </div>
    </div>
</div>
