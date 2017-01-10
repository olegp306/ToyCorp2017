[% for(var i = 0, arrLength = Videos.length; i < arrLength; i+=1) { %]
<div class="video-item">
    <div class="video-embed">[%= Videos[i].PlayerCode%]</div>
    <div class="video-description">[%= Videos[i].Description%]</div>
</div>
[% }%]