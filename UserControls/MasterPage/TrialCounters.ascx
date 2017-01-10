﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TrialCounters.ascx.cs" 
    Inherits="UserControls.MasterPage.TrialCounters" EnableViewState="false" %>
<div class="counter-wrap">
    <!-- Yandex.Metrika counter -->
    <script type="text/javascript">
        (function (d, w, c) {
            (w[c] = w[c] || []).push(function () {
                try {
                    w.yaCounter16316386 = new Ya.Metrika({
                        id: 16316386,
                        webvisor: true,
                        clickmap: true,
                        trackLinks: true,
                        accurateTrackBounce: true
                    });
                } catch (e) { }
            });

            var n = d.getElementsByTagName("script")[0],
                s = d.createElement("script"),
                f = function () { n.parentNode.insertBefore(s, n); };
            s.type = "text/javascript";
            s.async = true;
            s.src = (d.location.protocol == "https:" ? "https:" : "http:") + "//mc.yandex.ru/metrika/watch.js";

            if (w.opera == "[object Opera]") {
                d.addEventListener("DOMContentLoaded", f, false);
            } else { f(); }
        })(document, window, "yandex_metrika_callbacks");

        $(function () {
            if ($("a.trialAdmin").length) {
                $.advModal({
                    title: localize("demoMode"),
                    control: $("a.trialAdmin"),
                    isEnableBackground: true,
                    htmlContent: localize("demoCreateTrial"),
                    beforeOpen: function () {
                        yaCounter16316386.reachGoal('demo_admin_link');
                    },
                    buttons: [
                        { textBtn: localize("demoCreateNow"), isBtnClose: true, classBtn: "btn-confirm", func: function () { yaCounter16316386.reachGoal('demo_trial_button'); window.location = localize("trialUrl"); } },
                        { textBtn: localize("demoCancel"), isBtnClose: true, classBtn: "btn-action" }
                    ]
                });
            }
        });
    </script>
    <noscript>
        <div>
            <img src="//mc.yandex.ru/watch/16316386" style="position: absolute; left: -9999px;" alt="" />
        </div>
    </noscript>
    <!-- /Yandex.Metrika counter -->
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');
        ga('create', 'UA-41699428-1', 'advantshop.net');
        ga('send', 'pageview');
    </script>
</div>