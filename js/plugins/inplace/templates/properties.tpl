<ul class="properties" id="propertiesDetails">
  [% var currentGroupID = null, LineCounter = 0; %]

  [%for(var i = 0, l = PropertyValues.length; i < l; i += 1){%]    
  
    [% LineCounter += 1; %]
    
    [% if(PropertyValues[i].Property.Group != null && currentGroupID != PropertyValues[i].Property.Group.PropertyGroupId) {%]
    [% currentGroupID = PropertyValues[i].Property.Group.PropertyGroupId; %]
    [% LineCounter = 1; %]
    <li class="propgroup">[%= PropertyValues[i].Property.Group.Name %]</li> 
    [% } %]

    [% if(PropertyValues[i].Property.Group == null && currentGroupID != null) {%]
    [% currentGroupID = null; %]
    [% LineCounter = 1; %]
    <li class="propgroup">[%= localize('propertOther')%]</li>
    [% } %]

    <li class="properties-row [%= (LineCounter%2 == 0 ? '' : 'properties-row-nth') %]">
    <div class="param-name">[%= PropertyValues[i].Property.Name%]</div>
    <div class="param-value">
      <div class="inplace-property-wrap">
        <input class="inplace-item"
               data-plugin="inplace"
               data-inplace-options="{mode: 'singleline', type: 'autocomplete', autocompleteUrl: './httphandlers/getpropertiesnames.ashx'}"
               data-inplace-params='{id:[%= PropertyValues[i].PropertyValueId %], propertyId:[%= PropertyValues[i].Property.PropertyId %], productId:[%= ProductID %], type: "[%= Type %]", prop: "[%= Prop %]"}'
               value='[%=PropertyValues[i].Value%]'>
          <a href="javascript:void(0);" class="inplace-property-delete" data-inplace-property-delete="true">Delete</a>
        </div>
    </div>
  </li>
  [%}%]
</ul>