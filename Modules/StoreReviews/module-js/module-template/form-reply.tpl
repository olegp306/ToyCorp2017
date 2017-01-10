<div data-sr-form-reply="true" class="shop-reviews-form-reply" data-sr-id="[%= id%]" data-sr-parentId="[%= parentId %]">
    <div class="shop-reviews-form-title">
        [%= localizeStoreReviews('StoreReviewsFormReplyTitle')%]
    </div>
    <ul class="shop-reviews-form">
        <li class="shop-reviews-form-row">
            <label for="txtReviewerName-reply">
                [%= localizeStoreReviews('StoreReviewsFormReplyName')%]</label>
            <div>
                <div class="input-wrap">
                    <input id="txtReviewerName-reply" type="text" data-plugin="validelem" data-validelem-group="StoreReviews-reply"
                           data-validelem-methods="['required']" data-sr-form-data="name" />
                </div>
            </div>
        </li>
        <li class="shop-reviews-form-row">
            <label for="txtEmail-reply">
                [%= localizeStoreReviews('StoreReviewsFormReplyEmail')%]</label>
            <div>
                <div class="input-wrap">
                    <input id="txtEmail-reply" type="text" data-plugin="validelem" data-validelem-group="StoreReviews-reply"
                           data-validelem-methods="['required', 'email']" data-sr-form-data="email" />
                </div>
            </div>
        </li>
        <li class="shop-reviews-form-row">
            <label for="txtReview-reply">
                [%= localizeStoreReviews('StoreReviewsFormReplyReview')%]</label>
            <div>
                <div class="textarea-wrap">
                    <textarea id="txtReview-reply" data-validelem-group="StoreReviews-reply" data-plugin="validelem" data-validelem-methods="['required']" data-sr-form-data="review"></textarea>
                </div>
            </div>
        </li>
        <li class="shop-reviews-form-row"><a href="javascript:void(0);" class="btn btn-submit btn-middle" data-sr-form-ajax="true" data-validelem-btn="StoreReviews-reply">
                                              [%= localizeStoreReviews('StoreReviewsFormReplySend')%]</a>
        
            
            <a href="javascript:void(0);" class="btn btn-submit btn-middle" data-sr-cancel="true">
                [%= localizeStoreReviews('StoreReviewsFormReplyCancel')%]</a>
        </li>
    </ul>
</div>
