var langStoreReviews = [];

langStoreReviews['StoreReviewsAwfull'] = 'Awfull';
langStoreReviews['StoreReviewsBad'] = 'Bad';
langStoreReviews['StoreReviewsNormal'] = 'Normal';
langStoreReviews['StoreReviewsGood'] = 'Good';
langStoreReviews['StoreReviewsExcelent'] = 'Excelent';
langStoreReviews['StoreReviewsAlreadyVote'] = 'You have already voted';

langStoreReviews["StoreReviewsValidEmail"] = "Incorrect email";
langStoreReviews["StoreReviewsValidRequired"] = "This field is required";

langStoreReviews["StoreReviewsFormReplyTitle"] = "Reply";
langStoreReviews["StoreReviewsFormReplyName"] = "Name";
langStoreReviews["StoreReviewsFormReplyEmail"] = "Email";
langStoreReviews["StoreReviewsFormReplyReview"] = "Message";
langStoreReviews["StoreReviewsFormReplySend"] = "Send";
langStoreReviews["StoreReviewsFormReplyDelete"] = "Remove";
langStoreReviews["StoreReviewsFormReplyCancel"] = "Cancel";

langStoreReviews["StoreReviewsFormReplyErrorAdd"] = "Error adding reviews";

function localizeStoreReviews(param) {
    var p = param.toString();
    return langStoreReviews[p] || '<span style="color:red;">NOT RESOURCED</span>';
};