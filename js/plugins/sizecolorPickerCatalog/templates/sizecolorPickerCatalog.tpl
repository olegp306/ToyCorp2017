<div class="sizeColorPicker">
    <div class="colors">
            [% for (var j = 0, arrColors = Colors.length; j < arrColors; j += 1) {%]
            [% if (Colors[j].PhotoName === '' || Colors[j].PhotoName == null) {%]
              <div class="color-item [%= Colors[j].Selected ? 'selected' : '' %]" style="background: [%= Colors[j].ColorCode%]; width:[%=ImageWidth%]px; height:[%=ImageHeight%]px" title="[%= Colors[j].ColorName%]" data-color-id="[%= Colors[j].ColorId%]"></div>
            [%}else{%]
              <img src ="pictures/color/catalog/[%= Colors[j].PhotoName%]" class="color-item [%= Colors[j].Selected ? 'selected' : '' %]" title="[%= Colors[j].ColorName%]" data-color-id="[%= Colors[j].ColorId%]" />
              [%}%]
            [%}%]
    </div>
</div>