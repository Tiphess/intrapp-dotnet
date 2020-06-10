var matchHistory = (function () {
    var startIndex = 0;
    var endIndex = 5;

    function getMatches(accountId) {
        startIndex += 5;
        endIndex += 5;
        $('#waiting-div').show();
        $.ajax({
            url: "/Summoner/_MatchHistory",
            type: "POST",
            data: { accountId: accountId, startIndex: startIndex, endIndex: endIndex },
            success: function (data) {
                $("#matchHistory").append(data);
                $('#waiting-div').hide();
            }
        });
    }

    return {
        fetchMoreMatches: function (accountId) {
            getMatches(accountId);
        }
    }
})();

function SetSummonerInfoHandlers(accountId) {
    $("a[name='playerLink']").click(function () {
        $('#waiting-div').show();
    });
    $("#loadBtn").click(function () {
        matchHistory.fetchMoreMatches(accountId);
    });
}