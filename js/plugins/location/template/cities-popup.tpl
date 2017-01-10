<div class="location-modal">
    <div class="location-countries">
        [% for(var i = 0, arrLength = countries.length; i < arrLength; i++) { %]
        <div class="js-location-country location-countries-item[%= countries[i].CountryId == current.CountryId ? ' selected' : ''%]" data-countryId="[%=countries[i].CountryId%]">
            <img class="location-countries-item-img" src="images/countries/[%= countries[i].Iso2%].png" />
            <span class="location-countries-item-name js-location-country-name">[%= countries[i].Name%]</span>
        </div>
        [%}%]
    </div>
    
    <div class="location-search-wrap">
        <span>[%= localize('City')%]:</span> 
        <input type="text" autocomplete="off" class="js-location-search location-search" onkeyup="defaultButtonClick('btnLocationChange', event)" /> 
        <a data-location-search=".js-location-search" class="btn btn-submit btn-big js-location-search-confirm" href="javascript:void(0)" id="btnLocationChange">[%= localize('Confirm')%]</a>
    </div>

    <div class="location-cities">
        [% for(var i = 0, arrLength = cities.length; i < arrLength; i++) { %]
          [% if(i % Math.ceil(arrLength/4) == 0) {%]
            <div style="display:table-cell; vertical-align:top;">
          [%}%]
            <div class="location-cities-item js-location-cities-item"><span>[%= cities[i].Name%]</span></div>
              [% if(i % Math.ceil(arrLength/4)  == Math.ceil(arrLength/4)-1) {%]
            </div>
          [%}%]
       [%}%]
   </div>
</div>