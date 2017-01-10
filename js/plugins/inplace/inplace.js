'use strict';

(function ($) {


    if (typeof (CKEDITOR) == 'undefined') {
        return;
    }

    CKEDITOR.disableAutoInline = true;

    var advantshop = Advantshop, scriptManager = advantshop.ScriptsManager, utilities = advantshop.Utilities,
        advancedControls,
        advancedControlsAlt,
        fileInputUpdate, // for image load
        fileInputAdd, // for image load
        isMouseOverControl = false,
        border,
        timerPos,
        inplaceCurrentImageUpload,
        placeholderText = localize('InplaceEmpty'),
        isUpdate = true,
        fakeInput,
        contentAmountText,
        contentWeightUnit,
        modalPriceWithDiscountError,
        modalMetaInplceObj,
        isInplaceEnabled = false;

    var inplace = function (selector, params, options) {

        this.$obj = advantshop.GetJQueryObject(selector);


        if (this.$obj.hasClass('inplace-disabled') === true || inplace.prototype.checkEnabled() === false) {
            return null;
        }

        var parentOffset = this.$obj.closest('.inplace-indicator-offset-off');

        if (parentOffset.length > 0) {
            parentOffset.removeClass('inplace-indicator-offset-off');
        }

        this.params = params;
        this.params.content = $.trim(inplace.prototype.getValue(this.$obj));
        this.options = $.extend({}, inplace.prototype.defaultOptions, options);

        return this;
    };

    inplace.prototype.enumMode = {
        singleline: 'singleline',
        multiline: 'multiline'
    };

    inplace.prototype.enumType = {
        editor: 'editor',
        autocomplete: 'autocomplete',
        image: 'image',
        modal: 'modal'
    };

    advantshop.NamespaceRequire('Advantshop.ScriptsManager');
    scriptManager.Inplace = inplace;

    inplace.prototype.InitTotal = function (elements) {

        var objects = elements || $('[data-plugin="inplace"]'),
            params, options;

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {

            params = utilities.Eval(objects.eq(i).attr('data-inplace-params'));

            if (params == null) {
                continue;
            }

            options = utilities.Eval(objects.eq(i).attr('data-inplace-options')) || {};

            inplace.prototype.Init(objects.eq(i), params, options);
        }

        var flexsliderMain = $('.flexslider-main');

        if (flexsliderMain.attr('hidden') !== null) {
            flexsliderMain.removeAttr('hidden');

            if (flexsliderMain.data('flexslider') != null) {
                flexsliderMain.data('flexslider').resize();
            }
        }
    };

    inplace.prototype.DestroyTotal = function (elements) {

        var objects = elements || $('[data-plugin="inplace"]');

        for (var i = 0, arrLength = objects.length; i < arrLength; i += 1) {
            inplace.prototype.Destroy(objects.eq(i));
        }

        var ckeEditors = CKEDITOR.instances;

        for (var editor in ckeEditors) {
            if (ckeEditors.hasOwnProperty(editor) === true) {
                ckeEditors[editor].destroy();
            }
        }

        var flexsliderMain = $('.flexslider-main');

        if (flexsliderMain.length > 0) {
            var flexsliderData = flexsliderMain.data('flexslider');

            if (flexsliderData.slides.length === 1 && flexsliderData.slides.eq(0).find('#carouselNoPhoto').length > 0) {
                flexsliderMain.attr('hidden', 'hidden');
            }
        }


        $('#inplace-acc, #cke-acc-alt').remove();
    };

    inplace.prototype.Destroy = function (elem) {
        var inplaceObj = elem.data('inplace');

        if (inplaceObj != null) {

            elem.off('.inplace');

            switch (inplaceObj.options.type) {
                case inplace.prototype.enumType.autocomplete:
                    elem.off('.autocomplete');
                    break;
                case inplace.prototype.enumType.image:

                    if (fileInputAdd != null) {
                        fileInputAdd.remove();
                        fileInputAdd = null;
                    }
                    if (fileInputUpdate != null) {
                        fileInputUpdate.remove();
                        fileInputUpdate = null;
                    }

                    break;
                case inplace.prototype.enumType.editor:
                    elem.removeAttr('contenteditable');
                    $('body').off('mousedown.inplace');
                    elem.off('click.ckeInit');

                    if (elem.hasClass('inplace-placeholder') === true) {
                        elem.empty();
                    }

                    elem.removeClass('inplace-item inplace-type-editor inplace-placeholder');

                    break;
            }

            var parentOffset = elem.closest('.inplace-indicator-offset');

            if (parentOffset.length > 0) {
                parentOffset.addClass('inplace-indicator-offset-off');
            }
        }

    };

    $(window).load(function () {

        if (inplace.prototype.checkEnabled() === false) {
            return;
        }

        inplace.prototype.InitTotal();
    });

    inplace.prototype.Init = function (selector, params, options) {
        var obj = advantshop.GetJQueryObject(selector);

        if (obj.hasClass('inplace-disabled') === true || inplace.prototype.checkEnabled() === false) {
            return;
        }

        var inplaceObj = new inplace(selector, params, options);

        inplaceObj.GenerateHtml();

        inplaceObj.BindEvent();

        return inplaceObj;
    };

    inplace.prototype.GenerateHtml = function () {
        var inplaceObj = this;


        if (fakeInput == null) {
            fakeInput = $('<input />', {
                'type': 'text',
                'id': 'inplaceFakeObjForFocus'
            });
            $('body').append(fakeInput);
        }

        inplaceObj.$obj.addClass('inplace-item inplace-type-' + inplaceObj.options.type);

        switch (inplaceObj.options.type) {
            case inplace.prototype.enumType.editor:


                //var sizes = null,
                //    h = this.$obj.height(),
                //    w = this.$obj.width();

                //if (h > 0) {
                //    sizes = sizes || {};
                //    sizes.minHeight = h;
                //}

                //if (w > 0) {
                //    sizes = sizes || {};
                //    sizes.minWidth = w;
                //}

                //if (sizes != null) {
                //    inplaceObj.$obj.css(sizes);
                //}

                getContentFromDB(inplaceObj.params.type, inplaceObj.params.id).then(function (response) {
                    inplaceObj.$obj.data('hasScript', response.indexOf('<script') > -1);
                })

                inplaceObj.advancedCondrolsInit();
                break;

            case inplace.prototype.enumType.autocomplete:
                inplaceObj.advancedCondrolsInit();
                break;
            case inplace.prototype.enumType.image:
                fileInputUpdate = $('#inplaceImageUpdate');
                fileInputAdd = $('#inplaceImageAdd');

                if (fileInputUpdate.length === 0) {
                    fileInputUpdate = $('<input />', {
                        'id': 'inplaceImageUpdate',
                        'type': 'file',
                        'name': 'filesInplaceUpdate[]',
                        'multiple': 'multiple',
                        'class': 'inplace-upload',
                        'title': '',
                        'accept': 'image/*'
                    });
                }

                if (fileInputAdd.length === 0) {
                    fileInputAdd = $('<input />', {
                        'id': 'inplaceImageAdd',
                        'type': 'file',
                        'name': 'filesInplaceAdd[]',
                        'multiple': 'multiple',
                        'class': 'inplace-upload',
                        'title': '',
                        'accept': 'image/*'
                    });
                }

                inplaceObj.advancedCondrolsInit(fileInputAdd, fileInputUpdate);

                break;
        }

        //placeholder
        if ((inplaceObj.$obj.is('input') === false && inplaceObj.$obj.is('img') === false) && $.trim(inplaceObj.$obj.text()).length === 0 && inplaceObj.$obj.html().toLowerCase().indexOf('img') === -1) {
            inplaceObj.$obj.text(placeholderText);
            inplaceObj.$obj.addClass(inplaceObj.options.cssPlaceholder);
        }
    };

    inplace.prototype.BindEvent = function () {
        var inplaceObj = this;
        var $obj = inplaceObj.$obj;

        $obj.add($obj.closest('.inplace-indicator-offset')).on('click.inplace', function (e) {
            e.stopPropagation();
        })

        if (utilities.Events.isExistEvent($('body'), 'click.inplace') === false) {
            $('body').on('click.inplace', function (e) {
                var target = $(e.target),
                    InplacePropertyRemove = target.closest('[data-inplace-property-delete]'),
                    InplaceElem = $('.inplace-item.inplace-focus'),
                    isInplaceElem = target.closest(InplaceElem).length > 0,
                    isCKEDialog = target.closest('.cke_dialog').length > 0,
                    isCKEToolbar = target.closest('.cke').length > 0;

                if (InplaceElem.length > 0 && isInplaceElem === false && isCKEDialog === false && isCKEToolbar === false && InplacePropertyRemove.length === 0) {

                    if (CKEDITOR.currentInstance != null) {
                        CKEDITOR.currentInstance.focusManager.blur();
                    }

                    blur(InplaceElem.data('inplace'), e);
                }

                if (InplacePropertyRemove.length > 0) {
                    var inplacePropElem = InplacePropertyRemove.prevAll('.inplace-item');

                    if (inplacePropElem.length > 0) {
                        inplacePropElem.val('');

                        blur(inplacePropElem.data('inplace'), e);
                    }
                }

            });
        }



        switch (inplaceObj.options.type) {
            //#region autocomplete
            case inplace.prototype.enumType.autocomplete:

                var options = {
                    delay: 10,
                    minChars: 1,
                    matchSubset: 1,
                    autoFill: true,
                    matchContains: 1,
                    cacheLength: 0,
                    selectFirst: true,
                    //formatItem: liFormat,
                    maxItemsToShow: 10,
                    extraParams: inplaceObj.params,
                    cacheRequest: false
                };

                if ($obj.is('#inplacePropertyName')) {

                    options.formatRow = function (key, value, i, num) {
                        return $('<li />', { html: value }).attr('data-property-id', key)[0];
                    }

                    $obj.on('keyup.inplace', function () {
                        $obj.removeAttr('data-property-id');
                    });

                }

                if ($obj.is('#inplacePropertyValue')) {

                    options.formatRow = function (key, value, i, num) {
                        return $('<li />', { html: value }).attr('data-propertyvalue-id', key)[0];
                    }

                    $obj.on('keyup.inplace', function () {
                        $obj.removeAttr('data-propertyvalue-id');
                    });
                }

                $obj.autocomplete(inplaceObj.options.autocompleteUrl, options);

                $obj.on('blur.inplace', function (e) {
                    blur($obj.data('inplace'), e);
                });

                break;
                //#endregion

                //#region fileupload
            case inplace.prototype.enumType.image:

                var isImageLoad = false;

                fileInputAdd.fileupload({
                    url: inplaceObj.options.urlSave,
                    autoUpload: false,
                    singleFileUploads: false,
                    pasteZone: null
                });

                fileInputUpdate.fileupload({
                    url: inplaceObj.options.urlSave,
                    autoUpload: false,
                    singleFileUploads: false,
                    pasteZone: null
                });

                fileInputAdd.add(fileInputUpdate).on('click.inplace', function () {
                    isImageLoad = true;
                });

                fileInputAdd.on('fileuploadadd.inplace', function (e, data) {

                    e.preventDefault();
                    e.stopPropagation();
                    e.stopImmediatePropagation();

                    if (!(/\.(gif|jpg|jpeg|png)$/i).test(data.files[0].name)) {
                        return;
                    }

                    var obj = $('#cke-acc-alt:visible').prevAll('[data-plugin="inplace"]');

                    if (obj.length === 0) {
                        obj = $('#cke-acc-alt').prevAll('[data-plugin="inplace"].inplace-image-focus');
                    }

                    if (obj.length === 0) {
                        obj = $('.js-inplace-drop-active, .js-inplace-drop-carousel-active');
                    }

                    if (isImageLoad === true) {
                        obj = $('#cke-acc-alt').prev('[data-plugin="inplace"]');
                    }

                    if (obj.length === 0) {
                        return;
                    }

                    isImageLoad = false;

                    var objData = advantshop.Utilities.Eval(obj.attr('data-inplace-params'));

                    if (obj.length === 0 || obj.hasClass('inplace-loading') === true || (obj.hasClass('js-inplace-drop-carousel-active') === false && objData.id != -1 && obj.hasClass('js-inplace-drop-active'))) {
                        return;
                    }

                    var progressFileUpload = scriptManager.Progress.prototype.Init(obj),
                        scpDetails = $('[data-part="sizeColorPickerDetails"]'),
                        scpCatalog = obj.closest('.scp-item').find('[data-part="sizeColorPickerCatalog"]');

                    var colorID;

                    if (scpDetails.length > 0) {
                        colorID = $('.color-item.selected', scpDetails).attr('data-color-id');
                    } else if (scpCatalog.length > 0) {
                        colorID = $('.color-item.selected', scpCatalog).attr('data-color-id');
                    }

                    progressFileUpload.Show();

                    obj.addClass('inplace-loading');

                    data.formData = objData;

                    if (colorID != null) {
                        data.formData.colorID = colorID;
                    }

                    data.formData.command = 'Add';

                    var q = data.submit()
                        .success(function (data, textStatus, jqXHR) {

                            var flexslider = obj.closest('[data-plugin="flexslider"]');

                            if (flexslider.length > 0) {
                                var items = $([]);
                                var flexData = flexslider.data('flexslider');
                                obj.data('inplace').advancedCondrolsHide();

                                $('body').append(advancedControlsAlt);


                                for (var i = 0, l = data.length; i < l; i += 1) {
                                    items = items.add($(data[i]));
                                }

                                if (obj.is('#carouselNoPhoto') === true) {
                                    flexData.removeSlide(flexData.currentSlide);

                                    for (var i = 0, l = items.length; i < l; i += 1) {
                                        flexData.addSlide(items.eq(i));
                                    }

                                } else {
                                    for (var i = 0, l = items.length; i < l; i += 1) {
                                        flexData.addSlide(items.eq(i));
                                    }

                                    flexData.flexAnimate(flexData.slides.length - items.length, true, true);
                                }

                                for (var i = 0, l = items.length; i < l; i += 1) {
                                    inplace.prototype.InitTotal(items.eq(i).find('[data-plugin="inplace"]'));
                                }


                            } else {
                                if (obj.data('inplace') != null && obj.data('inplace').options.updateObj != null) {

                                    if (Array.isArray(data) === true) {
                                        data = data[0];
                                    }

                                    for (var i = 0, li = obj.data('inplace').options.updateObj.length; i < li; i += 1) {
                                        $('[data-inplace-update="' + obj.data('inplace').options.updateObj[i] + '"]').attr('src', data + '?' + Math.random());
                                    }
                                }
                            }
                        });

                    q.then(function (response) {

                        obj.removeClass('inplace-loading js-inplace-drop-active js-inplace-drop-carousel-active');
                        progressFileUpload.Hide();
                        obj.trigger('saved.inplace', [obj.data('inplace'), response]);
                        obj.data('inplace').advancedCondrolsHide();
                    });
                });

                fileInputUpdate.on('fileuploadadd.inplace', function (e, data) {

                    e.preventDefault();
                    e.stopPropagation();
                    e.stopImmediatePropagation();

                    if (!(/\.(gif|jpg|jpeg|png)$/i).test(data.files[0].name)) {
                        return;
                    }

                    var obj = $('#cke-acc-alt:visible').prevAll('[data-plugin="inplace"]');

                    if (obj.length === 0) {
                        obj = $('#cke-acc-alt').prevAll('[data-plugin="inplace"].inplace-image-focus');
                    }

                    if (obj.length === 0) {
                        obj = $('.js-inplace-drop-active, .js-inplace-drop-carousel-active');
                    }

                    if (isImageLoad === true) {
                        obj = $('#cke-acc-alt').prev('[data-plugin="inplace"]');
                    }

                    isImageLoad = false;

                    if (obj.length === 0) {
                        return;
                    }

                    var objData = advantshop.Utilities.Eval(obj.attr('data-inplace-params'));

                    if (obj.length === 0 || obj.hasClass('inplace-loading') === true) {
                        return;
                    }

                    obj.addClass('inplace-loading');

                    var progressFileUpload = scriptManager.Progress.prototype.Init(obj),
                    scpDetails = $('[data-part="sizeColorPickerDetails"]'),
                    scpCatalog = obj.closest('.scp-item').find('[data-part="sizeColorPickerCatalog"]');

                    progressFileUpload.Show();

                    var colorID;

                    if (scpDetails.length > 0) {
                        colorID = $('.color-item.selected', scpDetails).attr('data-color-id');
                    } else if (scpCatalog.length > 0) {
                        colorID = $('.color-item.selected', scpCatalog).attr('data-color-id');
                    }

                    data.formData = advantshop.Utilities.Eval(obj.attr('data-inplace-params'));

                    if (colorID != null) {
                        data.formData.colorID = colorID;
                    }

                    data.formData.command = 'Update';

                    var q = data.submit()
                        .success(function (data, textStatus, jqXHR) {

                            data = data[0];

                            if (obj.data('inplace') != null && obj.data('inplace').options.updateObj != null) {
                                for (var i = 0, li = obj.data('inplace').options.updateObj.length; i < li; i += 1) {
                                    $('[data-inplace-update="' + obj.data('inplace').options.updateObj[i] + '"]').attr('src', data + '?' + Math.random());
                                }
                            }
                        });

                    q.then(function (response) {
                        obj.removeClass('inplace-loading js-inplace-drop-active js-inplace-drop-carousel-active');
                        progressFileUpload.Hide();
                        obj.trigger('saved.inplace', [obj.data('inplace'), response]);
                        obj.data('inplace').advancedCondrolsHide();
                    });

                });

                $obj.add($obj.closest('#wrap')).on('drop.inplace', function () {
                    $obj.addClass('js-inplace-drop-active');
                });

                if (utilities.Events.isExistEvent($('#flexsliderDetails'), 'drop.inplace') === false) {
                    $('#flexsliderDetails').on('drop.inplace', function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        $('#preview-img').addClass('js-inplace-drop-carousel-active');
                        fileInputAdd.fileupload('add', { files: e.originalEvent.dataTransfer.files });
                    });
                }

                $(document).on('drop.inplace dragover.inplace dragenter.inplace dragleave.inplace', function (e) {
                    e.preventDefault();
                    // target.data('inplace').advancedCondrolsHide();
                });

                $obj.on('mouseover.inplace', function (e) {
                    var target = $(this);
                    target.addClass('inplace-image-focus');
                    target.data('inplace').advancedCondrolsShow();
                });

                $obj.on('mouseout.inplace', function (e) {
                    var target = $(this);
                    target.removeClass('inplace-image-focus');
                    target.data('inplace').advancedCondrolsHide();
                });

                var sliderControlEmul = $('.js-inplace-slider-emulate');

                if (sliderControlEmul.length > 0) {
                    sliderControlEmul.off('mouseover.inplace');
                    sliderControlEmul.on('mouseover.inplace', function () {
                        $(this).prevAll('[data-plugin="inplace"]').trigger('mouseover');
                    });
                }
                break;
                //#endregion

                //#region ckeditor
            case inplace.prototype.enumType.editor:

                $obj.on('click.ckeInit', function () {
                    var $this = $(this);

                    if ($this.hasClass('cke_editable_inline') === true) {
                        $this.off('click.ckeInit');
                        return;
                    }

                    var progress = scriptManager.Progress.prototype.Init($this);

                    progress.Show();

                    var ckeConfig = {
                        on: {
                            instanceReady: function (editor) {
                                var el = $(editor.sender.element.$);

                                if ($.trim(el.text()) === placeholderText && $this.html().toLowerCase().indexOf('img') === -1) {
                                    el.text('');
                                    el.removeClass(inplaceObj.options.cssPlaceholder);
                                }

                                if (el.data('hasScript') === true && inplaceObj.options.type === inplace.prototype.enumType.editor) {

                                    getContentFromDB(inplaceObj.params.type, inplaceObj.params.id).then(function (response) {
                                        if (el.data('cke') != null) {
                                            el.data('cke').setData(response);
                                        }
                                    });
                                }

                                progress.Hide();

                                el.trigger('focus');
                                //editor.editor.focusManager.focus(el);
                            }
                        }
                    };

                    switch (inplaceObj.options.mode) {
                        case inplace.prototype.enumMode.singleline:

                            ckeConfig.on.key = function (event) {

                                var keyCode = event.data.keyCode;

                                switch (keyCode) {
                                    case 13://enter

                                        isUpdate = true;

                                        fakeInput.trigger('focus');
                                        $('body').trigger('click');

                                        event.editor.focusManager.blur(false);

                                        event.stop();
                                        event.cancel();

                                        break;
                                    case 27://esc
                                        isUpdate = false;

                                        fakeInput.trigger('focus');
                                        $('body').trigger('click');

                                        event.editor.focusManager.blur(false);
                                        event.stop();
                                        event.cancel();
                                        break;
                                }
                            };

                            ckeConfig.removePlugins = 'toolbar, showborders, magicline';
                            ckeConfig.enterMode = CKEDITOR.ENTER_BR;
                            ckeConfig.forcePasteAsPlainText = true;
                            break;
                        case inplace.prototype.enumMode.multiline:
                            ckeConfig.on.key = function (event) {

                                var keyCode = event.data.keyCode;

                                switch (keyCode) {
                                    case 27://esc
                                        isUpdate = false;
                                        $('body').trigger('click');
                                        break;
                                }
                            };
                            ckeConfig.removePlugins = 'showborders, magicline';
                            break;
                    }


                    $obj.attr('contenteditable', 'true');
                    $obj.data('cke', CKEDITOR.inline($this[0], ckeConfig));
                });
                break;
                //#endregion

                //#region modal
            case inplace.prototype.enumType.modal:
                $.advModal({
                    modalClass: 'inplace-modal',
                    title: localize('inplaceModalTitle'),
                    control: $obj,
                    controlAfterOpen: modalMetaAfterOpen,
                    beforeClose: modalMetaBeforeClose,
                    buttons: [
                        {
                            textBtn: localize('Save'),
                            classBtn: 'btn-submit',
                            func: modalMetaApply
                        },
                        {
                            textBtn: localize('Close'),
                            classBtn: 'btn-action',
                            isBtnClose: true
                        }]
                });
                break;
                //#endregion
        }


        $obj.on('focus.inplace', function (e) {
            focus($obj.data('inplace'), e);
        });

        //$obj.on('blur.inplace', function (e) {
        //    blur($obj.data('inplace'), e);
        //});

        $obj.data('inplace', inplaceObj);

    };

    inplace.prototype.getValue = function ($obj) {
        var result = '';

        if (CKEDITOR.currentInstance != null) {
            result = CKEDITOR.currentInstance.getData();
        } else if ($obj[0].value != null) {
            result = $obj.val()
        } else {
            result = $.trim($obj.html())
        }

        return result;
    }

    inplace.prototype.setValue = function ($obj, value) {
        return $obj[0].value != null ? $obj.val(value) : $obj.html(value);
    }

    inplace.prototype.advancedCondrolPosAuto = function () {
        var inplaceObj = this;
        var cke = CKEDITOR.currentInstance;

        if (cke == null || advancedControls == null) {
            return;
        }


        (function pos() {

            if (timerPos != null) {
                clearTimeout(timerPos);
            }

            timerPos = setTimeout(function () {

                if (cke != null && cke.element != null) {
                    inplaceObj.advancedCondrolsPos($(cke.element.$));
                    pos();
                }

            }, 150);

        })();

    };

    inplace.prototype.advancedCondrolPosAutoDisable = function () {
        if (timerPos != null) {
            clearTimeout(timerPos);
        }
    };

    inplace.prototype.advancedCondrolsInit = function (contentBtnAdd, contentBtnUpdate) {

        var inplaceObj = this;

        if (this.options.type === inplace.prototype.enumType.editor || this.options.type === inplace.prototype.enumType.autocomplete) {


            if ($('#inplace-acc').length > 0) {
                return;
            }

            var container = $('<div />', {
                'id': 'inplace-acc',
                'class': 'inplace-acc-container'
            });

            var btnSave = $('<a />', {
                'href': 'javascript:void(0)',
                'class': 'inplace-acc inplace-acc-save',
                'text': 'Save'
            });

            var btnClose = $('<a />', {
                'href': 'javascript:void(0)',
                'class': 'inplace-acc inplace-acc-cancel',
                'text': 'Cancel'
            });

            container.append(btnSave).append(btnClose);

            $('body').append(container);

            advancedControls = container;

            advancedControls.on('mouseover', function () {
                isMouseOverControl = true;
            });

            advancedControls.on('mouseout', function () {
                isMouseOverControl = false;
            });

            btnSave.on('click.inplaceSave mouseenter.inplaceSave mouseleave.inplaceSave', function () {
                isUpdate = true;
            });

            btnClose.on('click.inplaceCancel mouseenter.inplaceCancel', function () {
                isUpdate = false;
            });

            btnClose.on('mouseleave.inplaceCancel', function () {
                isUpdate = true;
            });

            $(window).on('resize.windowInplaceResize', function () {
                var cke = CKEDITOR.currentInstance;

                if (cke != null && cke.element != null) {
                    inplaceObj.advancedCondrolsPos($(cke.element.$));
                }
            });
        } else {



            if ($('#cke-acc-alt').length > 0) {
                return;
            }

            var container = $('<div />', {
                'id': 'cke-acc-alt',
                'class': 'inplace-acc-alt-container'
            });


            var btnAdd = $('<a />', {
                'href': 'javascript:void(0)',
                'class': 'inplace-acc-alt inplace-acc-alt-add',
                'text': 'Add'
            }).append(contentBtnAdd);

            var btnUpdate = $('<a />', {
                'href': 'javascript:void(0)',
                'class': 'inplace-acc-alt inplace-acc-alt-update',
                'text': 'Replace',
            }).append(contentBtnUpdate);

            var btnDelete = $('<a />', {
                'href': 'javascript:void(0)',
                'class': 'inplace-acc-alt inplace-acc-alt-delete',
                'text': 'Delete',
            });

            container.append(btnAdd).append(btnUpdate).append(btnDelete);

            $('body').append(container);

            advancedControlsAlt = container;

            advancedControlsAlt.data('buttons', {
                'add': btnAdd,
                'update': btnUpdate,
                'delete': btnDelete
            });

            advancedControlsAlt.on('mouseover', function (e) {
                advancedControlsAlt.show();
            });

            advancedControlsAlt.on('mouseout', function (e) {
                advancedControlsAlt.hide();
            });

            advancedControlsAlt.on('click', function (e) {
                e.stopPropagation();
            });

            btnDelete.on('click', function (e) {
                var obj = advancedControlsAlt.prevAll('[data-plugin="inplace"]');
                var flexslider = obj.closest('[data-plugin="flexslider"]').data('flexslider');
                var params = obj.data('inplace').params;
                var scpCatalog = $(e.target).closest('.scp-item').find('[data-part="sizeColorPickerCatalog"]');

                var colorID;

                if (scpCatalog.length > 0) {
                    colorID = $('.color-item.selected', scpCatalog).attr('data-color-id');
                }

                if (colorID != null) {
                    params.colorID = colorID;
                }

                obj.data('inplace').advancedCondrolsHide();

                $('body').append(advancedControlsAlt);

                obj.data('inplace').Save($.extend({}, params, {
                    command: 'Delete'
                }), function (data) {
                    if (flexslider != null) {
                        flexslider.removeSlide(flexslider.currentSlide);

                        if (flexslider.slides.length === 0) {
                            var item = $(data[0]);
                            flexslider.addSlide(item);
                            inplace.prototype.InitTotal(item.find('[data-plugin="inplace"]'));
                        }
                    }
                });
            });
        }
    };

    inplace.prototype.advancedCondrolsShow = function () {

        var $obj = this.$obj;
        var controlsOptions = advantshop.Utilities.Eval($obj.attr('data-inplace-controls')) || { 'add': false, 'update': true, 'delete': true };

        if (this.options.type === inplace.prototype.enumType.editor || this.options.type === inplace.prototype.enumType.autocomplete) {
            if (advancedControls == null) {
                return;
            }

            advancedControls.show();
        } else {

            if (advancedControlsAlt == null) {
                return;
            }

            var buttons = advancedControlsAlt.data('buttons');

            if (controlsOptions != null) {


                for (var key in controlsOptions) {
                    if (controlsOptions[key] === false) {
                        buttons[key].hide();
                    } else {
                        buttons[key].show();
                    }
                }
            } else {
                for (var key in buttons) {
                    buttons[key].show();
                }
            }

            advancedControlsAlt.show();
        }


        this.advancedCondrolsPos();
    };

    inplace.prototype.advancedCondrolsHide = function () {
        if (this.options.type === inplace.prototype.enumType.editor || this.options.type === inplace.prototype.enumType.autocomplete) {
            advancedControls.hide();
        } else {
            advancedControlsAlt.hide();
        }
        this.advancedCondrolPosAutoDisable();


    };

    inplace.prototype.advancedCondrolsPos = function () {
        var inplaceObj = this,
            $obj = inplaceObj.$obj;

        if (this.options.type === inplace.prototype.enumType.editor || this.options.type === inplace.prototype.enumType.autocomplete) {

            if (advancedControls == null) {
                return;
            }

            var objOffset = $obj.offset();
            var objSize = {
                width: $obj.outerWidth(),
                height: $obj.outerHeight()
            }


            var pos = {
                top: objOffset.top + objSize.height,
                left: (objOffset.left + objSize.width) - advancedControls.outerWidth()
            };

            advancedControls.css(pos);
            fakeInput.css(pos);
        } else {
            if (advancedControlsAlt == null) {
                return;
            }

            $obj.after(advancedControlsAlt);

            var objOffset = $obj.position();

            var objSize = {
                width: $obj.outerWidth(),
                height: $obj.outerHeight()
            }

            var controlsSize = {
                width: advancedControlsAlt.outerWidth(),
                height: advancedControlsAlt.outerHeight()
            }

            advancedControlsAlt.css({
                top: objOffset.top + objSize.height - (objSize.height > controlsSize.height * 2 && $obj.hasClass('inplace-controls-pos-outside-bottom') === false ? controlsSize.height : 0),
                left: objOffset.left + objSize.width - controlsSize.width
            });


        }
    };

    inplace.prototype.Save = function (params, func) {
        var inplaceObj = this;
        var progress = scriptManager.Progress.prototype.Init(inplaceObj.$obj[0]);

        params = $.extend({}, inplaceObj.params, params);

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: inplaceObj.options.urlSave,
            data: params,
            cache: false,
            async: false,
            beforeSend: function () {
                progress.Show();
            },
            complete: function () {
                progress.Hide();
            },
            success: function (data) {

                if (data != null && data.isHasError === true) {
                    notify(data.errorMessage, notifyType.error);
                }

                if (inplaceObj.options.updateObj != null) {

                    var inplaceEditorsInside, inplaceEditorsInsideParams, inplaceEditorsInsideOptions;
                    var ckeditorInside;
                    var response = data != null ? data : inplaceObj.params.content;

                    inplaceObj.updateArr = [];

                    for (var i = 0, l = inplaceObj.options.updateObj.length; i < l; i += 1) {
                        inplaceObj.updateArr[inplaceObj.options.updateObj[i]] = $('[data-inplace-update="' + inplaceObj.options.updateObj[i] + '"]');
                    }


                    for (var key in inplaceObj.updateArr) {

                        if (inplaceObj.updateArr[key].length === 0) {
                            continue;
                        }

                        /*find ckeditors inside for destroy*/
                        inplaceEditorsInside = inplaceObj.updateArr[key].find('[data-plugin="inplace"]');

                        if (inplaceEditorsInside.length > 0) {
                            for (var c = 0, cl = inplaceEditorsInside.length; c < cl; c += 1) {
                                ckeditorInside = CKEDITOR.dom.element.get(inplaceEditorsInside[c]).getEditor();
                                if (ckeditorInside != null) {
                                    ckeditorInside.destroy();
                                }

                            }
                        }

                        /*replace html*/

                        if (inplaceObj.params.prop === 'Price' || inplaceObj.params.prop === 'PriceWithDiscount') {
                            advantshop.Offers.prototype.UpdateProduct.call(null, document.getElementById('hfProductId').value, response);
                        } else {

                            if (inplaceObj.updateArr[key].is('img') === true) {
                                inplaceObj.updateArr[key].attr('src', response + '?' + Math.random());
                            } else {
                                inplaceObj.updateArr[key].html(response);
                            }

                            /*reinit inplace inside*/
                            inplaceEditorsInside = inplaceObj.updateArr[key].find('[data-plugin="inplace"]');


                            if (inplaceEditorsInside.length > 0) {
                                for (var e = 0, el = inplaceEditorsInside.length; e < el; e += 1) {
                                    inplaceEditorsInsideParams = Advantshop.Utilities.Eval(inplaceEditorsInside.eq(e).attr('data-inplace-params')) || {};
                                    inplaceEditorsInsideOptions = Advantshop.Utilities.Eval(inplaceEditorsInside.eq(e).attr('data-inplace-options')) || {};
                                    inplace.prototype.Init(inplaceEditorsInside.eq(e), inplaceEditorsInsideParams, inplaceEditorsInsideOptions);
                                }
                            }
                        }
                    }
                }

                if (func != null) {
                    func(data);
                }

                inplaceObj.$obj.trigger('saved.inplace', [inplaceObj, response]);
            },
            error: function (data) {
                notify(localize('InplaceSaveError') + ': ' + data.statusText, notifyType.error);
            }
        });

    };

    inplace.prototype.checkEnabled = function () {

        var inplaceEnableControl = $('#inplaceEnable');

        if (inplaceEnableControl.length > 0) {
            isInplaceEnabled = inplaceEnableControl.is(':checked');
        } else {
            isInplaceEnabled = inplace.prototype.getEnabledStatus();
        }

        return isInplaceEnabled;
    };

    inplace.prototype.getEnabledStatus = function () {
        var result = false;

        $.ajax({
            type: 'GET',
            async: false,
            cache: false,
            url: './admin/httphandlers/inplaceeditor/inplaceStatus.ashx',
            success: function (data) {
                result = data.isEnabled;
            }
        });

        return result;
    };

    inplace.prototype.defaultOptions = {
        urlSave: './admin/httphandlers/inplaceeditor/inplaceeditor.ashx',
        updateObj: null,
        mode: inplace.prototype.enumMode.multiline,
        type: inplace.prototype.enumType.editor,
        cssInvalid: 'inplace-invalid',
        cssPlaceholder: 'inplace-placeholder',
        autocompleteUrl: null,
        modalTemplate: 'js/plugins/inplace/templates/meta.tpl'
    };

    function blur(inplaceClass, event) {

        var inplaceObj = inplaceClass;
        var $this = inplaceObj.$obj;
        var elemOffset = $this.closest('.inplace-indicator-offset-disabled');
        var saveCallback, saveParams;

        $this.removeClass(inplaceObj.options.cssInvalid);

        event.stopPropagation();

        if (inplaceObj.params.type === 'Product' && inplaceObj.params.prop === 'Weight') {
            contentWeightUnit.remove();
        }

        var contentHtml = inplace.prototype.getValue($this);

        if (contentHtml != inplaceObj.params.content) {

            if ($this.is('input') === false && $.trim($this.text()) === placeholderText && $this.html().toLowerCase().indexOf('img') === -1) {
                contentHtml = '';
            }

            //strip tags if mode singleline
            if (inplaceObj.options.mode === inplace.prototype.enumMode.singleline) {
                contentHtml = $.trim(contentHtml);
                contentHtml = contentHtml.replace(/<\/?([a-z][a-z0-9]*)\b[^>]*>/gi, '');
            }

            if (inplaceObj.params.required != null) {
                if (contentHtml.length === 0) {
                    $this.addClass(inplaceObj.options.cssInvalid);
                    return;
                } else {
                    $this.removeClass(inplaceObj.options.cssInvalid);
                }
            }

            if (inplaceObj.params.float != null) {
                if (contentHtml.length === 0 || isNaN(Number(contentHtml.replace(',', '.'))) === true) {
                    $this.addClass(inplaceObj.options.cssInvalid);
                    return;
                } else {
                    $this.removeClass(inplaceObj.options.cssInvalid);
                }
            }

            if (isUpdate === true) { //inplaceObj.params.content != contentHtml

                if ((inplaceObj.params.prop === 'Price' || inplaceObj.params.prop === 'PriceWithDiscount') && contentHtml.length > 0) {

                    contentHtml = contentHtml.replace(/&nbsp;*|\s*/g, '').replace(/,/g, '.');

                    if (/^\d+\s?\d*.?\d*?$/.test(contentHtml) != true) {
                        $this.addClass(inplaceObj.options.cssInvalid);
                        return;
                    } else {
                        $this.removeClass(inplaceObj.options.cssInvalid);
                    }

                    if (inplaceObj.params.prop === 'PriceWithDiscount') {
                        var oldPriceElem = $this.closest('#priceWrap').find('.price-old').find('[data-plugin="inplace"]').filter(function () {

                            var params = Advantshop.Utilities.Eval(this.getAttribute('data-inplace-params'));

                            return params.prop === 'Price';
                        });

                        var oldPrice = Number(oldPriceElem.text().replace(/&nbsp;*|\s*/g, '').replace(/,/g, '.'));
                        var priceWithDiscountNumber = Number(contentHtml);

                        if (isNaN(oldPrice) === false) {
                            if (oldPrice < priceWithDiscountNumber) {

                                if (modalPriceWithDiscountError == null) {
                                    modalPriceWithDiscountError = $.advModal({
                                        title: localize('inplacePriceWithDiscountErrorTitle'),
                                        htmlContent: localize('inplacePriceWithDiscountError'),
                                        buttons: [{
                                            textBtn: 'Ok',
                                            isBtnClose: true,
                                            classBtn: 'btn-confirm'
                                        }]
                                    });
                                }

                                modalPriceWithDiscountError.modalShow();
                                event.stopImmediatePropagation();

                                return;
                            }
                        } else {
                            oldPriceElem.addClass(inplaceObj.options.cssInvalid);
                        }

                    }
                }

                if (inplaceObj.params.type === 'Property') {

                    if ($this.is('#inplacePropertyName') === true || $this.is('#inplacePropertyValue') === true) {
                        var inplacePropertyName = $('#inplacePropertyName'),
                            inplacePropertyValue = $('#inplacePropertyValue'),
                            isError = null;

                        if ($this.is(inplacePropertyValue) === true) {

                            if (inplace.prototype.getValue(inplacePropertyName).length === 0) {
                                inplacePropertyName.addClass(inplaceObj.options.cssInvalid);
                                isError = true;
                            }

                            if (inplace.prototype.getValue(inplacePropertyValue).length === 0) {
                                inplacePropertyValue.addClass(inplaceObj.options.cssInvalid);
                                isError = true;
                            }
                        }

                        if (isError === true) { //|| $this.data('autocompleteResults').is(':visible')
                            return;
                        }


                        if ($this.is(inplacePropertyName) === true) {

                            var inplacePropertyValueObj = inplacePropertyValue.data('inplace');

                            //find value in result for get propertyId

                            //if (inplacePropertyName.attr('data-property-id') == null) {


                            var r = inplacePropertyName[0].autocompleter.findValue();
                            var val = inplacePropertyName.val();

                            for (var key in r) {
                                if (r.hasOwnProperty(key) && r[key] === val) {
                                    inplacePropertyName.attr('data-property-id', key);
                                    break;
                                }
                            }

                            //}

                            //update parameter propertyId for get propertyValues


                            var autocompleteParams = inplacePropertyValueObj.params;

                            if (inplacePropertyName.attr('data-property-id') != null) {
                                $.extend(inplacePropertyValueObj.params, { propertyid: inplacePropertyName.attr('data-property-id') });
                            } else {
                                delete inplacePropertyValueObj.params.propertyid;
                            }

                            inplacePropertyValue[0].autocompleter.setExtraParams(autocompleteParams);

                        } else if ($this.is(inplacePropertyValue) === true) {

                            var r = inplacePropertyValue[0].autocompleter.findValue();
                            var val = inplacePropertyValue.val();

                            for (var key in r) {
                                if (r.hasOwnProperty(key) && r[key] === val) {
                                    inplacePropertyValue.attr('data-propertyvalue-id', key);
                                    break;
                                }
                            }

                        }

                        if (inplaceObj.$obj.data('autocompleteResults').is(':visible') === true && isMouseOverControl === false) {
                            return;
                        }

                        var params = $.extend({}, inplaceObj.params, {
                            id: -1,
                            propertyid: inplacePropertyName.attr('data-property-id'),
                            propertyvalueid: inplacePropertyValue.attr('data-propertyvalue-id'),
                            propertyName: inplace.prototype.getValue(inplacePropertyName),
                            propertyValue: inplace.prototype.getValue(inplacePropertyValue),
                            content: 'add property'
                        });

                        if (params.propertyid != null && $this.is(inplacePropertyName) === true) {
                            $this.removeClass('inplace-focus');
                            inplaceObj.advancedCondrolsHide();
                            return;
                        }

                        var funcUpdateName = function (data) {
                            $this.removeClass('inplace-focus');
                            inplaceObj.advancedCondrolsHide();

                            $this.attr('data-property-id', data.PropertyId);
                        };

                        var funcUpdateValue = function () {
                            inplace.prototype.setValue(inplacePropertyName, '');
                            inplace.prototype.setValue(inplacePropertyValue, '');

                            inplacePropertyName.removeAttr('data-property-id');
                            inplacePropertyValue.removeAttr('data-propertyvalue-id');

                            $this.removeClass('inplace-focus');

                            $.ajax({
                                type: 'GET',
                                url: 'httphandlers/getpropertieslist.ashx',
                                data: { productId: inplaceObj.params.productId, categoryId: $('#propertiesDetails').attr('data-category-id') },
                                cache: false,
                                success: function (response) {
                                    var html = new EJS({ url: 'js/plugins/inplace/templates/properties.tpl' }).render(response);

                                    $('#propertiesDetails').html(html);

                                    var inplaces = $('[data-plugin="inplace"]', '#propertiesDetails');

                                    inplace.prototype.InitTotal(inplaces);

                                }
                            });

                            inplaceObj.advancedCondrolsHide();
                        };

                        saveParams = params;
                        saveCallback = $this.is('#inplacePropertyValue') === true ? funcUpdateValue : funcUpdateName;

                    } else {
                        saveCallback = function () {
                            $.ajax({
                                type: 'GET',
                                url: 'httphandlers/getpropertieslist.ashx',
                                data: { productId: inplaceObj.params.productId, categoryId: $('#propertiesDetails').attr('data-category-id') },
                                cache: false,
                                success: function (response) {
                                    var html = new EJS({ url: 'js/plugins/inplace/templates/properties.tpl' }).render(response);

                                    $('#propertiesDetails').html(html);

                                    var inplaces = $('[data-plugin="inplace"]', '#propertiesDetails');

                                    inplace.prototype.InitTotal(inplaces);

                                }
                            });
                        };
                    }
                }

                if (inplaceObj.params.type === 'Product' && inplaceObj.params.prop === 'Weight' && contentWeightUnit != null) {
                    inplaceObj.$obj.append(contentWeightUnit);
                }

                if (inplaceObj.params.prop == 'ArtNo') {
                    saveCallback = function (data) {
                        var productsStorage = Advantshop.Offers.prototype.GetStorage(),
                            product,
                            offers,
                            offerSelected;

                        for (var product in productsStorage) {

                            offers = productsStorage[product].storageOffers.Offers;

                            for (var i = offers.length - 1; i >= 0; i--) {

                                if (offers[i].OfferId == inplaceObj.params.id) {
                                    offerSelected = offers[i];
                                    break;
                                }
                            }
                        }

                        if (offerSelected != null) {
                            offerSelected.ArtNo = data.artNo;
                        }
                    };
                }

                inplaceObj.params.content = contentHtml;

                inplaceObj.Save(saveParams, saveCallback);

            } else {

                var contentOld = inplaceObj.params.type === 'Offer' && inplaceObj.params.prop === 'Amount' ? contentAmountText : inplaceObj.params.content;

                inplace.prototype.setValue($this, contentOld);
            }
        }


        contentAmountText = null;

        inplaceObj.advancedCondrolsHide();

        //placeholder;
        if ($this.is('input') === false && $.trim($this.text()).length === 0 && $this.html().toLowerCase().indexOf('img') === -1) {
            inplace.prototype.setValue($this, placeholderText);
            $this.addClass(inplaceObj.options.cssPlaceholder);
        }

        if (elemOffset.length > 0) {
            elemOffset.removeClass('inplace-indicator-offset-disabled').addClass('inplace-indicator-offset');
            elemOffset.removeClass('inplace-offset-focus');
        }

        //if (CKEDITOR.currentInstance != null) {
        //    $this.html(contentHtml);
        //    CKEDITOR.currentInstance.focusManager.blur();
        //}

        $this.removeClass('inplace-focus');
    }

    function focus(inplaceClass, event) {
        //event.stopPropagation();
        var inplaceObj = inplaceClass;
        var $this = inplaceObj.$obj;
        var content = $.trim($this.html());
        var elemOffset = $this.closest('.inplace-indicator-offset');

        //if ($this.data('hasScript') === true && inplaceObj.options.type === inplace.prototype.enumType.editor) {

        //    getContentFromDB(inplaceObj.params.type, inplaceObj.params.id).then(function (response) {
        //        if ($this.data('cke') != null) {
        //            $this.data('cke').setData(response);
        //        }
        //    });
        //}

        isUpdate = true;

        //placeholder

        if ($this.is('input') === false && $.trim($this.text()) === placeholderText && $this.html().toLowerCase().indexOf('img') === -1) {
            inplace.prototype.setValue($this, '');
            $this.removeClass(inplaceObj.options.cssPlaceholder);
        }

        if ($this.hasClass('price') === true && isNaN($.trim($this.html())) === true) {
            $this.html('');
        }

        inplaceObj.advancedCondrolsShow();

        inplaceObj.advancedCondrolPosAuto();

        if (elemOffset.length > 0) {
            elemOffset.addClass('inplace-offset-focus');
            elemOffset.removeClass('inplace-indicator-offset').addClass('inplace-indicator-offset-disabled');
        }

        $this.addClass('inplace-focus');

        if (inplaceObj.params.type === 'Offer' && inplaceObj.params.prop === 'Amount' && $this.hasClass(inplaceObj.options.cssInvalid) === false) {
            var $this = $(this);

            if (contentAmountText == null) {
                contentAmountText = $this.html();
            }

            var hfProductId = document.getElementById('hfProductId');

            if (hfProductId != null && hfProductId.getAttribute('data-page') === 'details') {
                var offers = new Advantshop.Offers(hfProductId.value);

                if (offers != null && offers.storageOffers != null && offers.storageOffers.offerSelected != null) {
                    var inplaceParameters = Advantshop.Utilities.Eval($this.attr('data-inplace-params'));

                    inplaceObj.params.id = inplaceParameters.id = offers.storageOffers.offerSelected.OfferId;

                    $this.attr('data-inplace-params', JSON.stringify(inplaceParameters).replace(/"/g, '\''));

                    $this.html(offers.storageOffers.offerSelected.Amount);
                } else {
                    var inplaceAmountParam = Advantshop.Utilities.Eval($this.attr('data-inplace-params'));
                    $this.html(inplaceAmountParam.content);
                }
            }

        } else if (inplaceObj.params.type === 'Product' && inplaceObj.params.prop === 'Weight' && $this.hasClass(inplaceObj.options.cssInvalid) === false) {
            if (contentWeightUnit == null) {
                contentWeightUnit = $this.find('.js-weight-unit');
            }

            contentWeightUnit.remove();

        } else if (inplaceObj.params.type === 'Offer' && inplaceObj.params.prop === 'ArtNo') {

            var hfProductId = document.getElementById('hfProductId');
            var offers = new Advantshop.Offers(hfProductId.value);

            if (offers != null && offers.storageOffers != null && offers.storageOffers.offerSelected != null) {
                var inplaceParameters = Advantshop.Utilities.Eval($this.attr('data-inplace-params'));

                inplaceObj.params.id = inplaceParameters.id = offers.storageOffers.offerSelected.OfferId;

                $this.attr('data-inplace-params', JSON.stringify(inplaceParameters).replace(/"/g, '\''));
            }
        }
    }

    function modalMetaAfterOpen(modal, event) {
        var params = Advantshop.Utilities.Eval($(event.target).attr('data-inplace-params'));

        var progressModal = Advantshop.ScriptsManager.Progress.prototype.Init(modal);

        progressModal.Show();

        modalMetaInplceObj = $(event.target).data('inplace');

        if (params != null) {

            $.ajax({
                url: './httphandlers/getmeta.ashx',
                data: params,
                cache: false
            }).done(function (data) {

                if (modal.find('.content').length === 0) {
                    var modalTpl = new EJS({ url: modalMetaInplceObj.options.modalTemplate }).render({ data: data });
                    modal.modalContent(modalTpl);
                    modal.modalPosition();
                } else {
                    modalMetaFill(data);
                }

                initValidation($('#form'), 'inplaceModalMetaInput');

            }).always(function () {
                progressModal.Hide();
            });

        }
    }

    function modalMetaBeforeClose() {
        modalMetaInplceObj = null;
    };

    function modalMetaApply(event) {

        if ($('#form').valid('inplaceModalMetaInput') === false) {
            return;
        }

        modalMetaInplceObj.Save(modalMetaData(), function (data) {
            window.location.reload(true);
        });
    };

    function modalMetaFill(data) {
        var inputs = modalMetaGetInputs();

        inputs.Title.value = data.Title;
        inputs.H1.value = data.H1;
        inputs.MetaKeywords.value = data.MetaKeywords;
        inputs.MetaDescription.value = data.MetaDescription;
        inputs.Name.value = data.Name;
    };

    function modalMetaData(data) {

        var inputs = modalMetaGetInputs();

        return {
            Title: inputs.Title.value,
            H1: inputs.H1.value,
            MetaKeywords: inputs.MetaKeywords.value,
            MetaDescription: inputs.MetaDescription.value,
            Name: inputs.Name.value
        }
    };

    function modalMetaGetInputs() {
        if (modalMetaGetInputs.Title == null) {
            modalMetaGetInputs.Title = document.getElementById('inplaceModalMetaTitle');
        }
        if (modalMetaGetInputs.H1 == null) {
            modalMetaGetInputs.H1 = document.getElementById('inplaceModalMetaH1');
        }
        if (modalMetaGetInputs.MetaKeywords == null) {
            modalMetaGetInputs.MetaKeywords = document.getElementById('inplaceModalMetaKeywords');
        }
        if (modalMetaGetInputs.MetaDescription == null) {
            modalMetaGetInputs.MetaDescription = document.getElementById('inplaceModalMetaDescription');
        }
        if (modalMetaGetInputs.Name == null) {
            modalMetaGetInputs.Name = document.getElementById('inplaceModalName');
        }

        return modalMetaGetInputs;
    };

    function getContentFromDB(type, id) {
        return $.ajax({
            type: 'GET',
            dataType: 'html',
            async: false,
            cache: false,
            url: './admin/httphandlers/inplaceeditor/inplacegetcontent.ashx',
            data: { type: type, id: id }
        });
    }

})(jQuery);