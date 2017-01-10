/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

/*set path for resources*/
//; (function () {
//    var adminWord = 'admin/',
//        base = document.getElementsByTagName('base')[0].href;

//    if (base.indexOf(adminWord) != -1) {
//        base = base.replace(adminWord, '');
//    }

//    CKEDITOR_BASEPATH = base + 'ckeditor/';

//})();
/**/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.title = false;

    config.allowedContent = true;

    config.autoParagraph = false;

    config.removePlugins = 'dragdrop, basket';

    config.baseHref = document.getElementsByTagName('base')[0].href;
    
    config.filebrowserBrowseUrl = 'ckeditor/plugins/filemanager/default.aspx';

    config.extraPlugins = 'sourcedialog';

    config.floatSpaceDockedOffsetY = 5;

    config.toolbar = [
        { name: 'source', items: ['Sourcedialog'] },
        { name: 'elements', items: ['NumberedList', 'BulletedList', 'Link', 'Unlink', '-', 'Image', 'Flash', 'Table', 'HorizontalRule'] },
        { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
        '/',
        { name: 'text', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'RemoveFormat'] },
        { name: 'text', items: ['TextColor', 'BGColor'] },
        { name: 'align', items: ['Outdent', 'Indent', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
        { name: 'document', items: ['PasteFromWord', 'Undo', 'Redo', 'Scayt'] },
    ];
};