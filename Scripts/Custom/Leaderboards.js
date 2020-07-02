function ShowChallengers() {
    if (!$("#challengerLink").hasClass("active")) {
        var region = $("#region-league").val();
        var queue = $("#queue").val();

        console.log(region);
        console.log(queue);
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_ChallengerLeague",
            type: "POST",
            data: { queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);

                $("#challengerLink").addClass("active");
                $("#grandmasterLink").removeClass("active");
                $("#masterLink").removeClass("active");

                $("div[name='waiting-div']").hide();
            }
        });
    }
}

function ShowGrandmasters() {
    if (!$("#grandmasterLink").hasClass("active")) {
        var region = $("#region-league").val();
        var queue = $("#queue").val();

        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_GrandmasterLeague",
            type: "POST",
            data: { queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);

                $("#grandmasterLink").addClass("active");
                $("#challengerLink").removeClass("active");
                $("#masterLink").removeClass("active");

                $("div[name='waiting-div']").hide();
            }
        });
    }
}

function ShowMasters() {
    if (!$("#masterLink").hasClass("active")) {
        var region = $("#region-league").val();
        var queue = $("#queue").val();

        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_MasterLeague",
            type: "POST",
            data: { queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);

                $("#masterLink").addClass("active");
                $("#grandmasterLink").removeClass("active");
                $("#challengerLink").removeClass("active");

                $("div[name='waiting-div']").hide();
            }
        });
    }
}


function UpdateLeague() {
    var region = $("#region-league").val();
    var queue = $("#queue").val();

    if ($("#masterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_MasterLeague",
            type: "POST",
            data: { queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_0").addClass("active");
            }
        });
    }
    else if ($("#grandmasterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_GrandmasterLeague",
            type: "POST",
            data: { queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_0").addClass("active");
            }
        });
    }
    else if ($("#challengerLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_ChallengerLeague",
            type: "POST",
            data: { queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_0").addClass("active");
            }
        });
    }
}

function GetPage(element) {
    var region = $("#region-league").val();
    var queue = $("#queue").val();


    if ($("#challengerLink").hasClass("active")) {
        $("div[name='waiting-div']").show();
        var page = $(element).text();

        $.ajax({
            url: "/Leaderboard/_ChallengerLeague",
            type: "POST",
            data: { page: page - 1, queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (page - 1)).addClass("active");
            }
        });
    }
    else if ($("#grandmasterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();
        var page = $(element).text();

        $.ajax({
            url: "/Leaderboard/_GrandmasterLeague",
            type: "POST",
            data: { page: page - 1, queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (page - 1)).addClass("active");
            }
        });
    }
    else if ($("#masterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();
        var page = $(element).text();

        $.ajax({
            url: "/Leaderboard/_MasterLeague",
            type: "POST",
            data: { page: page - 1, queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (page - 1)).addClass("active");
            }
        });
    }
}

function NextPage(currPage, maxPages) {
    var region = $("#region-league").val();
    var queue = $("#queue").val();

    if ($("#challengerLink").hasClass("active")) {
        $("div[name='waiting-div']").show();
        
        $.ajax({
            url: "/Leaderboard/_ChallengerLeague",
            type: "POST",
            data: { page: (currPage + 1), queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (currPage + 1 >= maxPages ? currPage : currPage + 1)).addClass("active");
            }
        });
    }
    else if ($("#grandmasterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_GrandmasterLeague",
            type: "POST",
            data: { page: (currPage + 1), queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (currPage + 1 >= maxPages ? currPage : currPage + 1)).addClass("active");
            }
        });
    }
    else if ($("#masterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();
        
        $.ajax({
            url: "/Leaderboard/_MasterLeague",
            type: "POST",
            data: { page: (currPage + 1), queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (currPage + 1 >= maxPages ? currPage : currPage + 1)).addClass("active");
            }
        });
    }
}

function PreviousPage(currPage) {
    var region = $("#region-league").val();
    var queue = $("#queue").val();

    if ($("#challengerLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_ChallengerLeague",
            type: "POST",
            data: { page: currPage - 1, queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (currPage === 0 ? currPage : currPage - 1)).addClass("active");
            }
        });
    }
    else if ($("#grandmasterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_GrandmasterLeague",
            type: "POST",
            data: { page: currPage - 1, queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (currPage === 0 ? currPage : currPage - 1)).addClass("active");
            }
        });
    }
    else if ($("#masterLink").hasClass("active")) {
        $("div[name='waiting-div']").show();

        $.ajax({
            url: "/Leaderboard/_MasterLeague",
            type: "POST",
            data: { page: currPage - 1, queue: queue, region: region },
            success: function (data) {
                $("#League").html(data);
                $("div[name='waiting-div']").hide();
                $("#page_" + (currPage === 0 ? currPage : currPage - 1)).addClass("active");
            }
        });
    }
}