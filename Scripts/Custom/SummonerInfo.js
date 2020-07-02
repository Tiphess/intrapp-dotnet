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
        $("div[name='waiting-div']").show();
    });
    $("#loadBtn").click(function () {
        matchHistory.fetchMoreMatches(accountId, region);
    });
    $("#SummonerNameFormNavbar").submit(function () {
        $("div[name='waiting-div']").show();
    });
}

function OpenMatchBreakdown(accId, region, gameId) {
    if ($("div[name='MoreDetailsBtn_" + gameId + "']").css('display') !== 'none') {
        if ($("#overview_" + gameId).length) {
            $("#BreakdownDiv_" + gameId).slideDown(400);
            $("div[name='MoreDetailsBtn_" + gameId + "']").hide();
            $("div[name='CloseBtn_" + gameId + "']").show();
            $("#breakdown_hr_" + gameId).show();
        }
        else {
            $("#BreakdownWaitingDiv_" + gameId).show();
            $.ajax({
                url: "/Summoner/_MatchBreakdown",
                type: "POST",
                data: { accountId: accId, region: region, gameId: gameId },
                success: function (data) {
                    $("#BreakdownDiv_" + gameId).append(data);
                    $("#BreakdownDiv_" + gameId).slideDown(400);
                    $("#BreakdownWaitingDiv_" + gameId).hide();

                    $("div[name='MoreDetailsBtn_" + gameId + "']").hide();
                    $("div[name='CloseBtn_" + gameId + "']").show();
                    $("#breakdown_hr_" + gameId).show();
                }
            });
        }
    }
}

function CloseMatchBreakDown(gameId) {
    if ($("div[name='CloseBtn_" + gameId + "']").css('display') !== 'none') {
        $("#BreakdownDiv_" + gameId).slideUp(400);
        $("div[name='MoreDetailsBtn_" + gameId + "']").show();
        $("div[name='CloseBtn_" + gameId + "']").hide();
        $("#breakdown_hr_" + gameId).hide();
    }
}

function ShowAnalysis(gameId) {
    if ($("#analysis_" + gameId).css('display') === 'none') {
        if ($("#overviewLink_" + gameId).hasClass("active")) {
            $("#overview_" + gameId).slideToggle(400);
        }
        else if ($("#buildLink_" + gameId).hasClass("active")) {
            $("#build_" + gameId).slideToggle(400);
        }
        $("#analysis_" + gameId).slideToggle(400);

        $("#analysisLink_" + gameId).addClass("active");
        $("#overviewLink_" + gameId).removeClass("active");
        $("#buildLink_" + gameId).removeClass("active");
    }
}

function ShowOverview(gameId) {
    if ($("#overview_" + gameId).css('display') === 'none') {
        if ($("#analysisLink_" + gameId).hasClass("active")) {
            $("#analysis_" + gameId).slideToggle(400);
        }
        else if ($("#buildLink_" + gameId).hasClass("active")) {
            $("#build_" + gameId).slideToggle(400);
        }
        $("#overview_" + gameId).slideToggle(400);

        $("#overviewLink_" + gameId).addClass("active");
        $("#analysisLink_" + gameId).removeClass("active");
        $("#buildLink_" + gameId).removeClass("active");
    }
}

function ShowBuild(gameId) {
    if ($("#build_" + gameId).css('display') === 'none') {
        FillTableWithLevels(gameId);
        if ($("#overviewLink_" + gameId).hasClass("active")) {
            $("#overview_" + gameId).slideToggle(400);
        }
        else if ($("#analysisLink_" + gameId).hasClass("active")) {
            $("#analysis_" + gameId).slideToggle(400);
        }
        $("#build_" + gameId).slideToggle(400);

        $("#buildLink_" + gameId).addClass("active");
        $("#analysisLink_" + gameId).removeClass("active");
        $("#overviewLink_" + gameId).removeClass("active");
    }
}

function FillTableWithLevels(gameId) {
    var cellsOffset = 1 // Since the first 2 cells are not skillpoints
    $("#QRow_" + gameId + " > td").each(function (index) {
        if ($(this).hasClass("skills-table-skillpoint")) {
            $(this).text(index - cellsOffset);
        }
    });
    $("#WRow_" + gameId + " > td").each(function (index) {
        if ($(this).hasClass("skills-table-skillpoint")) {
            $(this).text(index - cellsOffset);
        }
    });
    $("#ERow_" + gameId + " > td").each(function (index) {
        if ($(this).hasClass("skills-table-skillpoint")) {
            $(this).text(index - cellsOffset);
        }
    });
    $("#RRow_" + gameId + " > td").each(function (index) {
        if ($(this).hasClass("skills-table-skillpoint")) {
            $(this).text(index - cellsOffset);
        }
    });
}
