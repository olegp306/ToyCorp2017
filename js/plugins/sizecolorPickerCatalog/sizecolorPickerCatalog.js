(function ($) {

    var storage = {};

    var sizeColorPickerCatalog = function (place) {

        var urlObj = new Advantshop.Utilities.Uri(window.location.href),
            colorsId = urlObj.getQueryParamValue('color'),
            colors = Advantshop.Utilities.Eval(place.attr('data-colors')),
            arrColorsSelected, isColorSelectedExist = false;

        if (colors == null) {
            return null;
        }

        //fix change last item color
        colors.reverse();

        this.$obj = place;
        this.Colors = colors;
        this.ImageWidth = place.attr('data-color-image-width');
        this.ImageHeight = place.attr('data-color-image-height');
        colorsId = colorsId != null ? colorsId.split(',') : null;

        arrColorsSelected = colorsId || [place.attr('data-color-id-active')];

        for(var i = 0, l = arrColorsSelected.length; i < l; i +=1){
            arrColorsSelected[i] = parseInt(arrColorsSelected[i]);
        }

        for (var c = 0, lc = colors.length; c < lc; c += 1) {

            if ($.inArray(colors[c].ColorId, arrColorsSelected) != -1 && colors[c].Main === 1) {
                colors[c].Selected = true;
                isColorSelectedExist = true;
                break;
            }
        }

        if (isColorSelectedExist === false) {

            for (var c = 0, lc = colors.length; c < lc; c += 1) {

                if ($.inArray(colors[c].ColorId, arrColorsSelected) != -1) {
                    colors[c].Selected = true;
                    isColorSelectedExist = true;
                    break;
                }
            }
        }

        //fix change last item color
        colors.reverse();

    };

    Advantshop.NamespaceRequire('Advantshop.Details.Part');

    Advantshop.Details.Part.sizeColorPickerCatalog = sizeColorPickerCatalog;

    sizeColorPickerCatalog.prototype.Init = function (place) {

        if (place.length === 0) {
            return null;
        }

        var sizeColorPickerCatalogObj = new sizeColorPickerCatalog(place);

        if (sizeColorPickerCatalogObj.hasOwnProperty('Colors') === false) {
            return null;
        }

        sizeColorPickerCatalogObj.Generate();

        sizeColorPickerCatalogObj.BindEvent();

        return sizeColorPickerCatalogObj;
    };

    sizeColorPickerCatalog.prototype.InitTotal = function () {
        var placeSizeColors = $('[data-part="sizeColorPickerCatalog"]'),
            productId, offers, options;

        if (placeSizeColors.length === 0) {
            return null;
        }


        for (var i = 0, l = placeSizeColors.length; i < l; i += 1) {
            sizeColorPickerCatalog.prototype.Init(placeSizeColors.eq(i));
        }
    };

    $(sizeColorPickerCatalog.prototype.InitTotal);

    sizeColorPickerCatalog.prototype.Generate = function () {
        new EJS({ url: 'js/plugins/sizeColorPickerCatalog/templates/sizeColorPickerCatalog.tpl' }).update(this.$obj[0], this);
    };

    sizeColorPickerCatalog.prototype.BindEvent = function () {
        var sizeColorPickerCatalogObj = this,
            colors = sizeColorPickerCatalogObj.colors,
            colorsItems = $('.color-item', sizeColorPickerCatalogObj.$obj);

        this.$obj.on('click', '.color-item', function () {

            if (this.className.indexOf('selected') !== -1) {
                return;
            }

            $(this).trigger('changeColor', [sizeColorPickerCatalogObj]);

            colorsItems.removeClass('selected');

            $(this).addClass('selected');

            sizeColorPickerCatalogObj.$obj.attr('data-color-id', $(this).attr('data-color-id'))
        });
    };

})(jQuery);
