(function ($, $body) {
    $(function () {
        $('.shop-reviews-rating').each(function () {
            var item = $(this),
                scope = item.attr('data-sr-rating');

            item.StoreReviewsRaty({
                path: 'modules/StoreReviews/module-js/module-rate/module-images/',
                hints: [localizeStoreReviews("StoreReviewsAwfull"), localizeStoreReviews("StoreReviewsBad"), localizeStoreReviews("StoreReviewsNormal"), localizeStoreReviews("StoreReviewsGood"), localizeStoreReviews("StoreReviewsExcelent")],
                readOnly: true,
                score: scope
            });
        });

        $('.shop-reviews-form-rate').StoreReviewsRaty({
            path: 'modules/StoreReviews/module-js/module-rate/module-images/',
            hints: [localizeStoreReviews("StoreReviewsAwfull"), localizeStoreReviews("StoreReviewsBad"), localizeStoreReviews("StoreReviewsNormal"), localizeStoreReviews("StoreReviewsGood"), localizeStoreReviews("StoreReviewsExcelent")],
            readOnly: false,
            target: '#hint',
            targetKeep: true,
            click: function (scope) {
                $('#hfScope').val(scope);
            }
        });

        $('[data-sr-reply]').on('click', function () {
            var btn = $(this),
                $place = btn.closest('[data-sr-form-btn]'),
                $form = $('[data-sr-form-reply]'),
                $container = btn.closest('[data-sr-item]'),
                id = $container.attr('data-sr-item'),
                parentId = $container.attr('data-sr-parentId');

            if ($form.length === 0) {
                if (window.EJS == null) {
                    $.ajax({
                        url: 'modules/StoreReviews/module-js/ejs.js',
                        async: false,
                        dataType: 'script',
                        success: function (data) {
                            (new Function('return ' + data))();
                        },
                        error: function (data) {
                            throw Error(data.responseText);
                        }
                    });
                }

                var html = new EJS({ url: 'modules/StoreReviews/module-js/module-template/form-reply.tpl' }).render({ id: id, parentId: parentId });

                $form = $(html);

                $place.append($form);

                validateElemStoreReviews.prototype.InitTotal();

            } else {

                $form.attr({
                    'data-sr-id': id,
                    'data-sr-parentId': parentId
                });

                $place.after($form);

                $form.find('input:text, textarea').val('');

                if ($form.is(':hidden') === true) {
                    $form.show();
                }
            }

        });

        $body.on('click', '[data-sr-form-ajax]', function (e) {

            if (validateElemStoreReviews.prototype.ValidGroup('StoreReviews-reply') === false) {
                return;
            }

            var container = $(e.target).closest('[data-sr-form-reply="true"]'),
                parentId = container.attr('data-sr-id'),
                dataForm = $('[data-sr-form-data]', container),
                name = dataForm.filter('[data-sr-form-data="name"]').val(),
                email = dataForm.filter('[data-sr-form-data="email"]').val(),
                review = dataForm.filter('[data-sr-form-data="review"]').val();

            $.ajax({
                type: 'POST',
                cache: false,
                traditional: true,
                url: 'modules/StoreReviews/module-httphandlers/StoreReviewsadd.ashx',
                data: { parentId: parentId, name: name, email: email, review: review, scope: 0 },
                success: function (data) {
                    if (data === 'success') {
                        window.location.replace(window.location.href);
                    } else {
                        throw Error(localizeStoreReviews('StoreReviewsFormReplyErrorAdd'));
                    }
                },
                error: function (data) {
                    throw Error(localizeStoreReviews('StoreReviewsFormReplyErrorAdd') + ' : ' + data.responseText);
                }
            });
        });

        $body.on('click', '[data-sr-cancel]', function (e) {
            var $form = $(e.target).closest('[data-sr-form-reply]');

            if ($form.length === 0) {
                return;
            }

            $form.find('input:text, textarea').val('');

            $form.hide();
        });


        $(document).on('keyup', function (e) {

            if (e.ctrlKey != true) {
                return;
            }

            var pagingPrev = $('[data-sr-paging] [data-sr-paging-prev]');
            var pagingNext = $('[data-sr-paging] [data-sr-paging-next]');

            switch (e.keyCode) {
                case 37:
                    if (pagingPrev.length > 0) {
                        window.location.href = pagingPrev.attr('href');
                    }
                    break;
                case 39:
                    if (pagingNext.length > 0) {
                        window.location.href = pagingNext.attr('href');
                    }
                    break;
                default:
                    break;
            }

        });
    });
})(jQuery, $(document.body));