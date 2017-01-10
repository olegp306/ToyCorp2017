var langStoreReviews = [];

langStoreReviews["StoreReviewsAwfull"] = "Ужасно";
langStoreReviews["StoreReviewsBad"] = "Плохо";
langStoreReviews["StoreReviewsNormal"] = "Нормально";
langStoreReviews["StoreReviewsGood"] = "Хорошо";
langStoreReviews["StoreReviewsExcelent"] = "Отлично";
langStoreReviews["StoreReviewsAlreadyVote"] = "Вы уже голосовали";

langStoreReviews["StoreReviewsValidEmail"] = "Некорректно введен email";
langStoreReviews["StoreReviewsValidRequired"] = "Это поле обязательное для заполнения";

langStoreReviews["StoreReviewsFormReplyTitle"] = "Ответить";
langStoreReviews["StoreReviewsFormReplyName"] = "Имя";
langStoreReviews["StoreReviewsFormReplyEmail"] = "Email";
langStoreReviews["StoreReviewsFormReplyReview"] = "Сообщение"; 
langStoreReviews["StoreReviewsFormReplySend"] = "Отправить";
langStoreReviews["StoreReviewsFormReplyDelete"] = "Удалить";
langStoreReviews["StoreReviewsFormReplyCancel"] = "Отмена";

langStoreReviews["StoreReviewsFormReplyErrorAdd"] = "Ошибка при добавлении отзыва";

function localizeStoreReviews(param) {
    var p = param.toString();
    return langStoreReviews[p] || '<span style="color:red;">NOT RESOURCED</span>';
};