<div class="vote">
    <div class="voting-question">
        [%= Question %]</div>
    <div class="voting-content">
            [% for(var i=0, arrLength = Result.Rows.length; i< arrLength; i++) { %]
            <div class="row">
                [%= Result.Rows[i].Text%]
                <div class="vote-progressbar">
                    <div style="width: [%= Result.Rows[i].Value %]%" class="vp-line [%= Result.Rows[i].Selected === true ? 'vp-line-sel' : '' %]">
                    </div>
                </div>
                <span class="variant-count">[%= Result.Rows[i].Value %]%</span>
            </div>
            [% } %]
        <div class="total">
            <span class="text">[%= localize('votingTotalVoces') %]</span> [%= Result.Count %]</div>
    </div>
</div>
