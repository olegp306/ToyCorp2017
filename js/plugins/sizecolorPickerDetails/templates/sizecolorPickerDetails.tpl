<div class="sizeColorPicker">

    [% if(isColorsNull === false) { %]
    <div class="sizeColorPicker-field">[%= colorHeader %]:</div>
    <div class="colors">
            [%  for (var j = 0, arrColors = Colors.length; j < arrColors; j += 1) {%]
            [% if (Colors[j].PhotoName == null || Colors[j].PhotoName === '') {%]
              <div class="color-item [%= ColorIdSelected === Colors[j].ColorId ? 'selected' : '' %]" style="background-color: [%= Colors[j].ColorCode%];width:[%=ImageWidth%]px; height:[%=ImageHeight%]px" title="[%= Colors[j].ColorName%]" data-color-id="[%= Colors[j].ColorId%]"></div>
            [%}else{%]
              <img src ="pictures/color/details/[%= Colors[j].PhotoName%]" class="color-item [%= ColorIdSelected === Colors[j].ColorId ? 'selected' : '' %]" title="[%= Colors[j].ColorName%]" data-color-id="[%= Colors[j].ColorId%]" />
            [%}%]
            [% } %]
            </div>
    [% } %]

    [% if(isSizesNull === false) { %]
    <div class="sizeColorPicker-field">[%= sizeHeader %]:</div>
    <div class="sizes">
        [% for (var i = 0, arrSizes = Sizes.length; i < arrSizes; i += 1) {%]
        <div [%= Sizes[i].isDisabled === true ? 'data-disabled="disabled"' : '' %] class="size-item [%= SizeIdSelected === Sizes[i].SizeId ? 'selected' : '' %]" data-size-id="[%= Sizes[i].SizeId%]">
            <div class="size-item-inside">
                [%= Sizes[i].SizeName%]
            </div>
        </div>
        [% } %]
    </div>
    [% } %]
</div>