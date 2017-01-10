<form novalidate="novalidate" data-reviews-form="true">
  <ul class="form reviews-form">
    <li class="header">[%= localize('reviewsAnswer')%]</li>
    <li class="reviews-form-name">
      <div class="param-name">
        <label for="txtName-reply">
          [%= localize('reviewsName')%]
        </label>
      </div>
      <div class="param-value">
        <div class="input-wrap">
          <input type="text" data-reviews-param="name" class="valid-required" id="txtName-reply" name="txtName-reply" value="[%= userName %]" />
        </div>
      </div>
    </li>
    <li class="reviews-form-email">
      <div class="param-name">
        <label for="txtEmail-reply">
          [%= localize('reviewsEmail')%]
        </label>
      </div>
      <div class="param-value">
        <div class="input-wrap">
          <input type="text" data-reviews-param="email" class="valid-email" id="txtEmail-reply" name="txtEmail-reply" value="[%= userEmail %]" />
        </div>
      </div>
    </li>
    <li class="clear"></li>
    <li class="reviews-form-message li-long">
      <div class="param-name">
        <label for="txtMessage-reply">
          [%= localize('reviewsComent')%]:
        </label>
      </div>
      <div class="param-value">
        <div class="textarea-wrap">
          <textarea data-reviews-param="text" class="valid-required" id="txtMessage-reply" name="txtMessage-reply" cols="20"
                rows="2"></textarea>
        </div>
      </div>
    </li>
    <li class="reviews-send">
      <span class="btn-c">
        <a data-reviews-action="add" class="btn greenButton"
                  href="javascript:void(0);">[%= localize("reviewsSend")%]</a>
      </span>
      <span class="btn-c">
        <a data-reviews-action="cancel"
           class="btn greenButton" href="javascript:void(0);">[%= localize('reviewsCancel')%]</a>
      </span>

    </li>
  </ul>
  <br class="clear" />
</form>
