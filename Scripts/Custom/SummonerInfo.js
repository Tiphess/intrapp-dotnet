var matchHistory = (function () {
    var startIndex = 0;
    var endIndex = 5;

    function getMatches(accountId, region) {
        startIndex += 5;
        endIndex += 5;
        $("div[name='waiting-div']").show();
        $.ajax({
            url: "/Summoner/_MatchHistory",
            type: "POST",
            data: { accountId: accountId, startIndex: startIndex, endIndex: endIndex, region: region },
            success: function (data) {
                $("#matchHistory").append(data);
                $("div[name='waiting-div']").hide();
            }
        });
    }

    return {
        fetchMoreMatches: function (accountId, region) {
            getMatches(accountId, region);
        }
    }
})();

function SetSummonerInfoHandlers(accountId, region) {
    $("a[name='playerLink']").click(function () {
        $('#waiting-div').show();
    });
    $("#loadBtn").click(function () {
        matchHistory.fetchMoreMatches(accountId, region);
    });
}