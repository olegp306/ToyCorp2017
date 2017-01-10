<ul class="form form-hr">
  <li>
    <div class="param-name">
      <label for="inplaceModalName">
        <div class="inplace-modal-select">[%= data.NameFieldTitle %]</div>
      </label>
    </div>
    <div class="param-value">
      <span class="input-wrap">
        <input autocomplete="off" id="inplaceModalName" class="valid-required group-inplaceModalMetaInput" type="text" value="[%= data.Name.replace(/"/g, '&quot;') %]" />
      </span>
    </div>
  </li>
  <li>
    <div class="param-name">
      <label for="inplaceModalMetaH1">
        <div class="inplace-modal-select">[%= localize('inplaceModalMetaH1') %]</div>
      </label>
    </div>
    <div class="param-value">
      <span class="input-wrap">
        <input autocomplete="off" id="inplaceModalMetaH1" type="text" value="[%= data.H1.replace(/"/g, '&quot;') %]" />
      </span>
    </div>
    <div class="inplace-modal-note">
      [%= data.globalVaribles%]
    </div>
  </li>
  <li>
    <div class="param-name">
      <label for="inplaceModalMetaTitle">
        <div class="inplace-modal-select">[%= localize('inplaceModalMetaTitle') %]</div>
      </label>
    </div>
    <div class="param-value">
      <span class="input-wrap">
        <input autocomplete="off" id="inplaceModalMetaTitle"  type="text" value="[%= data.Title.replace(/"/g, '&quot;') %]" />
      </span>
    </div>
    <div class="inplace-modal-note">
      [%= data.globalVaribles%]
    </div>
  </li>
  <li>
    <div class="param-name">
      <label for="inplaceModalMetaKeywords">
        <div class="inplace-modal-select">[%= localize('inplaceModalMetaKeywords') %]</div>
      </label>
    </div>
    <div class="param-value">
      <span class="textarea-wrap inplace-modal-textarea-wrap">
        <textarea autocomplete="off" id="inplaceModalMetaKeywords" type="text">[%= data.MetaKeywords.replace(/"/g, '&quot;') %]</textarea>
      </span>
    </div>
    <div class="inplace-modal-note">
      [%= data.globalVaribles%]
    </div>
  </li>
  <li>
    <div class="param-name">
      <label for="inplaceModalMetaDescription">
        <div class="inplace-modal-select">[%= localize('inplaceModalMetaDescription') %]</div>
      </label>
    </div>
    <div class="param-value">
      <span class="textarea-wrap inplace-modal-textarea-wrap">
        <textarea autocomplete="off" id="inplaceModalMetaDescription" type="text">[%= data.MetaDescription.replace(/"/g, '&quot;') %]</textarea>
      </span>
    </div>
    <div class="inplace-modal-note">
      [%= data.globalVaribles%]
    </div>
  </li>
</ul>