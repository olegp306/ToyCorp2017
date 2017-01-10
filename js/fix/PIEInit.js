var listPIE = ".pie,\
               .btn, .input-wrap,\
               .textarea, .tabs .tabs-headers a,\
               .tabs .tabs-headers div.tab-header,\
               .tabs .tabs-headers .tab-header span.tab-inside,\
               .stroke, .stroke-inside, .error-message, .p-table td.img-middle, .flex-control-nav a, .flexslider .slides img, .staticBlock, .textarea-wrap";

function PIELoad(elements, container) {
    if (window.PIE) {
        container = container || 'body';
        elements = elements != null ? elements : listPIE;
        elements = $(elements, container).filter(':not(.no-pie)');
        PIEDeatch(elements);
        PIEAttach(elements);
    }
}
function PIEDeatch(elements, container) {
    if (window.PIE) {
        container = container || 'body';
        elements = elements != null ? elements : listPIE;
        elements = $(elements, container).filter(':not(.no-pie)');

        elements.each(function () {
            PIE.detach(this);
        });
    }
}
function PIEAttach(elements, container) {
    if (window.PIE) {
        container = container || 'body';
        elements = elements != null ? elements : listPIE;
        elements = $(elements, container).filter(':not(.no-pie)');

        elements.each(function () {
            PIE.attach(this);
        });

    }
}