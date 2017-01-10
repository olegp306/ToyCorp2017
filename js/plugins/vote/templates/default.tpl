<div class="vote">
  <div class="voting-question">
    [%= Question %]
  </div>
  <div class="voting-content">
    [% for(var i=0, arrLength = Answers.length; i< arrLength; i++) { %]
            <div class="row">
      <input type="radio" id="vote-[%= Answers[i].AnswerId%]" value="[%= Answers[i].AnswerId%]"
          name="vote">
        <label for="vote-[%= Answers[i].AnswerId%]">[%= Answers[i].Text%]</label>
      </div>
    [% } %]
    <div class="vote-submit">
      <span class="btn-c">
        <a data-vote-control="vote-submit" class="btn btn-confirm btn-middle"
                href="javascript:void(0);">[%= localize('vote') %]</a>
      </span>
    </div>

    [% if(IsHaveNullVoice)   {%]
    <div class="vote-submit">
      <span class="btn-c">
        <a data-vote-control="novote-submit" class="btn btn-confirm btn-middle"
                href="javascript:void(0);">[%= localize('showvote') %]</a>
      </span>
    </div>
    [% } %]
  </div>
</div>
