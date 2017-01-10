$(function () {

    if (!$("div.top-panel-constructor", "div.top-panel").length) return;

    $.ajax({
        method: "POST",
        url: "httphandlers/design/getalldesignsettings.ashx",
        datatype: "json",
        async: false,
        cache: false,
        success: function (data) {

            $("body").data("designDefault", data.DesignCurrent);

            constructor(data.Themes, data.Colors, data.Backgrounds, data.Structures, data.DesignCurrent, data.isTrial, data.isTemplate);
        },
        error: function (data) {
            //notify(localize("constructorGetDesignError") + " status text:" + data.statusText, notifyType.error, true);
        }
    });

});

function constructor(arrayTheme, arrayColor, arrayBackground, arrayStructures, designDefault, showOnLoad, showStructure) {

    //generate html			 
    var html = "<div class='constructor'>";
    
    /**structure**/
    if (!showStructure) {
        html += "<div class='constructor-structure'>";
        html += "<div class='title'>" + localize("constructorStructure") + "</div>";
        html += "<ul class=\"list-stretch\">";
        html += "<li><div class='structure-main'><input id='structureMain' type='radio' name='structure' value='" + arrayStructures[0] + "' " + (arrayStructures[0] == designDefault.Structure ? "checked='checked'" : "") + "><label class='structure-main-pic " + (arrayStructures[0] == designDefault.Structure ? "structure-selected" : "") + " ' for='structureMain'></label></li> ";
        html += "<li><div class='structure-twocolumns'><input id='structureTwocolumns' type='radio' name='structure' value='" + arrayStructures[1] + "' " + (arrayStructures[1] == designDefault.Structure ? "checked='checked'" : "") + "><label class='structure-twocolumns-pic " + (arrayStructures[1] == designDefault.Structure ? "structure-selected" : "") + " ' for='structureTwocolumns'></label></div></li> ";
        html += "<li><div class='structure-threecolumns'><input id='structureThreecolumns' type='radio' name='structure' value='" + arrayStructures[2] + "' " + (arrayStructures[2] == designDefault.Structure ? "checked='checked'" : "") + "><label class='structure-threecolumns-pic " + (arrayStructures[2] == designDefault.Structure ? "structure-selected" : "") + " ' for='structureThreecolumns'></label></div></li> ";
        html += "<li><div class='structure-descr'>" + localize("constructorSaveStructure") + "</div></li> ";
        html += "</ul>";
        html += "</div>";
    }

    /**themes**/
    html += "<div class='constructor-themes'>";
    html += "<div class='title'>" + localize("constructorThemes") + "</div>";
    html += "<div class=\"constructor-content\">";
    var tpl = "<div><input type='radio' name='{0}' id='{1}' value='{2}' data-template='{4}' /><label for='{1}'>{3}</label></div>";
    $.each(arrayTheme, function (idx, el) {
        html += String.Format(tpl, "theme", "theme" + el.Name, el.Name.toLowerCase(), el.Title, designDefault.Template);
    });
    html += "</div>";
    html += "<br class='clear'/></div>";

    /**colors**/
    html += "<div class='constructor-colors'><div class='title'>" + localize("constructorColorSchemes") + "</div>";
    html += "<div class=\"constructor-content\">";
    $.each(arrayColor, function (idx, el) {
        html += String.Format(tpl, "color", "color" + el.Name, el.Name.toLowerCase(), el.Title, designDefault.Template);
    });
    html += "</div>";
    html += "<br class='clear'/></div>";

    /**background**/
    html += "<div class='constructor-background'><div class='title'>" + localize("constructorBackground") + "</div>";
    html += "<div class=\"constructor-content\">";
    $.each(arrayBackground, function (idx, el) {
        html += String.Format(tpl, "background", "background" + el.Name, el.Name.toLowerCase(), el.Title, designDefault.Template);
    });
    html += "</div>";
    html += "<br class='clear'/></div>";
    /****/

    var themeInputs, colorInputs, backgroundInputs, structuresInputs;

    //init modal
    var advmodal = $.advModal({
        title: localize("constructorTitle"),
        htmlContent: html,
        clickOut: false,
        control: $("div.top-panel-constructor", "div.top-panel"),
        isEnableBackground: false,
        modalId: "modal-constructor",
        funcCross: designBack,
        initCallback: function () {
            themeInputs = $("input:radio", "div.constructor-themes"),
            colorInputs = $("input:radio", "div.constructor-colors"),
            backgroundInputs = $("input:radio", "div.constructor-background"),
            structuresInputs = $("input:radio", "div.constructor-structure");
        },
        afterOpen: function () {
            radioMarks(themeInputs, colorInputs, backgroundInputs);
        },
        buttons: [
            { textBtn: localize("constructorSave"), isBtnClose: true, classBtn: "btn-confirm", func: function () { designSave(themeInputs.filter(":checked"), colorInputs.filter(":checked"), backgroundInputs.filter(":checked"), structuresInputs.filter(":checked")); } },
            { textBtn: localize("constructorCancel"), isBtnClose: true, classBtn: "btn-action", func: designBack }
        ]
    });

    $("#modal-constructor").draggable({ handle: $("#modal-constructor > div.title"), scroll: false });

    if (showOnLoad) {
        advmodal.modalShow();
    }

    //structure
    $("div.constructor-structure", "div.constructor").click(function (e) {
        var el = inputGet($(e.target));
        if (!el) return;

        el.closest("div.constructor-structure").find("label").removeClass("structure-selected");

        el.siblings("label").addClass("structure-selected");
    });
    //theme
    $("div.constructor-themes", "div.constructor").click(function (e) {
        var el = inputGet($(e.target));
        if (!el) return;

        var templatePath = el.attr("data-template");

        if (templatePath != "") {
            templatePath = "templates/" + el.attr("data-template") + "/";
        }
        var path = String.Format("{0}design/themes/{1}/css/styles.css", templatePath, el.val());
        $("#themecss").attr("href", path.toLowerCase());

        resetRadio("background");
    });
    //color
    $("div.constructor-colors", "div.constructor").click(function (e) {
        var el = inputGet($(e.target));
        if (!el) return;

        var templatePath = el.attr("data-template");

        if (templatePath != "") {
            templatePath = "templates/" + el.attr("data-template") + "/";
        }
        var path = String.Format("{0}design/colors/{1}/css/styles.css", templatePath, el.val());
        $("#colorcss").attr("href", path.toLowerCase());

    });
    //background
    $("div.constructor-background", "div.constructor").click(function (e) {
        var el = inputGet($(e.target));
        if (!el) return;

        var templatePath = el.attr("data-template");

        if (templatePath != "") {
            templatePath = "templates/" + el.attr("data-template") + "/";
        }
        var path = String.Format("{0}design/backgrounds/{1}/css/styles.css", templatePath, el.val());
        $("#themecss").attr("href", path.toLowerCase());

        resetRadio("theme");
    });
}


function designSave(theme, color, background, structure) {

    var themeId = theme.attr("id").replace("theme", "");
    var colorId = color.attr("id").replace("color", "");
    var backgroundId = background.attr("id").replace("background", "");
    var structureId = structure.val();


    $.ajax({
        dataType: "text",
        cache: false,
        type: "POST",
        url: "httphandlers/design/savedesignsettings.ashx",
        data: {
            theme: themeId,
            colorscheme: colorId,
            background: backgroundId,
            structure: structureId
        },
        success: function (data) {
            if (data == "error") {
                notify(localize("constructorError"), notifyType.error, true);
            } else {

                var designPrevius = $("body").data("designDefault");

                var design = { Theme: theme.val(), ColorScheme: color.val(), Background: background.val(), Structure: structure.val(), Template: designPrevius.Template };

                $("body").data("designDefault", design);

                if (designPrevius.Structure != structure.val()) {
                    window.location.reload(true);
                }
            }
        },
        error: function (data) {
            notify(localize("constructorError") + " status text:" + data.statusText, notifyType.error, true);
        }
    });
}
function designBack() {
    var designDefault = $("body").data("designDefault");

    var templatePath = designDefault.Template != "" ? "templates/" + designDefault.Template + "/" : "";

    if (designDefault.Theme != "_none") {
        $("#themecss").attr("href", templatePath + "design/themes/" + designDefault.Theme + "/css/styles.css");
    } else {
        $("#themecss").attr("href", templatePath + "design/backgrounds/" + designDefault.Background + "/css/styles.css");
    }

    $("#colorcss").attr("href", templatePath + "design/colors/" + designDefault.ColorScheme + "/css/styles.css");

}
function resetRadio(type) {
    if (type == "theme") {
        $("input:radio:first", "div.constructor-themes").attr("checked", "checked");
        //$("#themecss").attr("href", "design/themes/_none/css/styles.css");
    }
    else if (type == "background") {
        $("input:radio:first", "div.constructor-background").attr("checked", "checked");
        //$("#theme-container").removeClass();
    }
}
function inputGet(el) {

    if (!el.is("input:radio")) {
        return false;
    }

    return el;
}

function radioMarks(themeInputs, colorInputs, backgroundInputs) {

    var designDefault = $("body").data("designDefault");

    var themeActive = themeInputs.filter("[value='" + designDefault.Theme.toLowerCase() + "']");

    if (!themeActive.length) {
        themeActive = themeInputs.eq(0);
        themeActive.click();
    } else {
        themeActive.attr("checked", "checked");
    }

    themeActive.closest("div.constructor-content").scrollTop(themeActive.position().top);

    var colorActive = colorInputs.filter("[value='" + designDefault.ColorScheme.toLowerCase() + "']");

    if (!colorActive.length) {
        colorActive = colorInputs.eq(0);
        colorActive.click();
    } else {
        colorActive.attr("checked", "checked");
    }

    
    colorActive.closest("div.constructor-content").scrollTop(colorActive.position().top);

    var backgroundActive = backgroundInputs.filter("[value='" + designDefault.Background.toLowerCase() + "']");

    if (!backgroundActive.length) {
        backgroundActive = backgroundInputs.eq(0);
        backgroundActive.click();
    } else {
        backgroundActive.attr("checked", "checked");
    }

    
    backgroundActive.closest("div.constructor-content").scrollTop(backgroundActive.position().top);
}