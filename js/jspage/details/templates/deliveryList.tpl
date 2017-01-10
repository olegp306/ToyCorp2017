<div class="details-delivery-place">[%= localize('detailsDeliveryFor')%] <a href="javascript:void(0)" class="js-location-call js-location-replacement" data-location-mask="#city#"><!--[%= City%]--></a></div>
<ul class="details-delivery-list">
    [% for (var i = 0, arrDeliveryLength = Shippings.length; i < arrDeliveryLength; i += 1) {%]
    <li class="details-delivery-row">
      <span class="details-delivery-name">― [%= Shippings[i].Name %] [%=Shippings[i].DeliveryTime != null && Shippings[i].DeliveryTime.length > 0 ? '(' + Shippings[i].DeliveryTime + ')': '' %]
          [%=Shippings[i].Ext != "" ? "<div>" + Shippings[i].Ext + "</div>" : "" %]:
      </span> 
      <div class="details-delivery-cost">[%= Shippings[i].Rate %]</div>
    </li>
    [% } %]
</ul>