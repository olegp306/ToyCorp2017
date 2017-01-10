[% for(var i = 0, arrLength = Reviews.length; i < arrLength; i+=1) { %]
<div data-reviews-itemid="[%= Reviews[i].Id%]" class="review-item">
    <div class="author">
        [%= Reviews[i].Name%]<span class="date">[%= Reviews[i].Date%]</span></div>
    <div class="message">
        [%= Reviews[i].Text%]</div>
    <div class="btn-review">
        <a id="btn-send" href="javascript:void(0);" data-reviews-action="reply">[%= localize('reviewsAnswer')%]</a>
        [% if (Reviews[i].isAdmin === true ) { %]
        <a class="btn-remove" href="javascript:void(0)" data-reviews-action="delete">[%= localize('reviewsDelete')%]</a>
        [% } %]
     </div>
            [%= Advantshop.ScriptsManager.Reviews.prototype.GetChild(Reviews[i].Children) %]
</div>
[% } %]
