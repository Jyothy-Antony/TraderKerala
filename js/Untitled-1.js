function changeprty() {
    document.getElementById('txt1').style.opacity = "1";
    document.getElementById('img1').style.float = "left";
    document.getElementById('img2').style.margin = "0 0 0 20px";
    document.getElementById('img3').style.margin = "0 0 0 20px";
    document.getElementById('img4').style.margin = "0 0 0 20px";
    document.getElementById('img1').style.margin = "0 0 0 20px";
    document.getElementById('txt2').style.opacity = "1";
    document.getElementById('img2').style.float = "left";
    document.getElementById('txt3').style.opacity = "1";
    document.getElementById('img3').style.float = "left";
    document.getElementById('txt4').style.opacity = "1";
    document.getElementById('img4').style.float = "left";
    document.getElementById('img1').style.padding = "10px 10px 0 5px";
    document.getElementById('img2').style.padding = "10px 10px 0 5px";
    document.getElementById('img3').style.padding = "10px 10px 0 5px";
    document.getElementById('img4').style.padding = "10px 10px 0 5px";
    document.getElementById('icon').style.background = "url('logo.jpg') no-repeat center center";
    document.getElementById('icon').style.width = "180px";
    document.getElementById('txt1').style.display = "block";
    document.getElementById('txt2').style.display = "block";
    document.getElementById('txt3').style.display = "block";
    document.getElementById('txt4').style.display = "block";
}

function hide1() {
    document.getElementById('rd2').style.display = "none";
    document.getElementById('rd3').style.display = "none";
    document.getElementById('rd4').style.display = "none";
    document.getElementById('rd1').style.marginLeft = "360px";
    document.getElementById('rda').style.backgroundColor = "#232f3b";
    document.getElementById('rda').style.color = "#fff";
    document.getElementById('rda').style.backgroundImage = "url('images/cycle-click.png')";
    document.getElementById('rda').style.position = "29px";
    document.getElementById('rda').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdb').style.backgroundColor = "#fff";
    document.getElementById('rdb').style.color = "#232f3b";
    document.getElementById('rdb').style.backgroundImage = "url('images/accessories.png')";
    document.getElementById('rdb').style.position = "29px";
    document.getElementById('rdb').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdc').style.backgroundColor = "#fff";
    document.getElementById('rdc').style.color = "#232f3b";
    document.getElementById('rdc').style.backgroundImage = "url('images/revenduers.png')";
    document.getElementById('rdc').style.position = "29px";
    document.getElementById('rdc').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdd').style.backgroundColor = "#fff";
    document.getElementById('rdd').style.color = "#232f3b";
    document.getElementById('rdd').style.backgroundImage = "url('images/collections.png')";
    document.getElementById('rdd').style.position = "29px";
    document.getElementById('rdd').style.backgroundRepeat = "no-repeat";
    document.getElementById('prdcts').style.display = "none";
    document.getElementById('xbtn').style.display = "none";
}

function hide2() {
    document.getElementById('rd1').style.display = "none";
    document.getElementById('rd3').style.display = "none";
    document.getElementById('rd4').style.display = "none";
    document.getElementById('rd2').style.marginLeft = "360px";
    document.getElementById('rdb').style.backgroundColor = "#232f3b";
    document.getElementById('rdb').style.color = "#fff";
    document.getElementById('rdb').style.backgroundImage = "url('images/accessories-click.png')";
    document.getElementById('rdb').style.position = "29px";
    document.getElementById('rdb').style.backgroundRepeat = "no-repeat";
    document.getElementById('rda').style.backgroundColor = "#fff";
    document.getElementById('rda').style.color = "#232f3b";
    document.getElementById('rda').style.backgroundImage = "url('images/cycle.png')";
    document.getElementById('rda').style.position = "29px";
    document.getElementById('rda').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdc').style.backgroundColor = "#fff";
    document.getElementById('rdc').style.color = "#232f3b";
    document.getElementById('rdc').style.backgroundImage = "url('images/revenduers.png')";
    document.getElementById('rdc').style.position = "29px";
    document.getElementById('rdc').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdd').style.backgroundColor = "#fff";
    document.getElementById('rdd').style.color = "#232f3b";
    document.getElementById('rdd').style.backgroundImage = "url('images/collections.png')";
    document.getElementById('rdd').style.position = "29px";
    document.getElementById('rdd').style.backgroundRepeat = "no-repeat";
    document.getElementById('df').style.display = "none";
    document.getElementById('xbtn').style.display = "none";
    document.getElementById('prdcts').style.display = "none";
    document.getElementById('xbtn').style.display = "none";
}
$(document).ready(function() {
    $('#rda').click(function() {
        $("#rd1").animate({
            marginLeft: "toggle"
        }, 1200);
    });
    $('#rdb').click(function() {
        $("#rd2").animate({
            marginLeft: "toggle"
        }, 1200);
    });
    $('#rdc').click(function() {
        $("#rd3").animate({
            marginLeft: "toggle"
        }, 1200);
    });
    $('#rdd').click(function() {
        $("#rd4").animate({
            marginLeft: "toggle"
        }, 1200);
    });
    $('#rde').click(function() {
        $("#xbtn").animate({
            marginLeft: "toggle"
        }, 1500);
    });
    $('#rdf').click(function() {
        $("#prdcts").animate({
            marginLeft: "toggle"
        }, 1500);
    });
});

function hide3() {
    document.getElementById('rd1').style.display = "none";
    document.getElementById('rd2').style.display = "none";
    document.getElementById('rd4').style.display = "none";
    document.getElementById('rd3').style.marginLeft = "360px";
    document.getElementById('rdc').style.backgroundColor = "#232f3b";
    document.getElementById('rdc').style.color = "#fff";
    document.getElementById('rdc').style.backgroundImage = "url('images/revenduers-click.png')";
    document.getElementById('rdc').style.position = "29px";
    document.getElementById('rdc').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdb').style.backgroundColor = "#fff";
    document.getElementById('rdb').style.color = "#232f3b";
    document.getElementById('rdb').style.backgroundImage = "url('images/accessories.png')";
    document.getElementById('rdb').style.position = "29px";
    document.getElementById('rdb').style.backgroundRepeat = "no-repeat";
    document.getElementById('rda').style.backgroundColor = "#fff";
    document.getElementById('rda').style.color = "#232f3b";
    document.getElementById('rda').style.backgroundImage = "url('images/cycle.png')";
    document.getElementById('rda').style.position = "29px";
    document.getElementById('rda').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdd').style.backgroundColor = "#fff";
    document.getElementById('rdd').style.color = "#232f3b";
    document.getElementById('rdd').style.backgroundImage = "url('images/collections.png')";
    document.getElementById('rdd').style.position = "29px";
    document.getElementById('rdd').style.backgroundRepeat = "no-repeat";
    document.getElementById('prdcts').style.display = "none";
    document.getElementById('xbtn').style.display = "none";
}

function hide4() {
    document.getElementById('rd1').style.display = "none";
    document.getElementById('rd2').style.display = "none";
    document.getElementById('rd3').style.display = "none";
    document.getElementById('rd4').style.marginLeft = "360px";
    document.getElementById('rdd').style.backgroundColor = "#232f3b";
    document.getElementById('rdd').style.color = "#fff";
    document.getElementById('rdd').style.backgroundImage = "url('images/collections-click.png')";
    document.getElementById('rdd').style.position = "29px";
    document.getElementById('rdb').style.backgroundRepeat = "no-repeat";
    document.getElementById('rda').style.backgroundColor = "#fff";
    document.getElementById('rda').style.color = "#232f3b";
    document.getElementById('rda').style.backgroundImage = "url('images/cycle.png')";
    document.getElementById('rda').style.position = "29px";
    document.getElementById('rda').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdb').style.backgroundColor = "#fff";
    document.getElementById('rdb').style.color = "#232f3b";
    document.getElementById('rdb').style.backgroundImage = "url('images/accessories.png')";
    document.getElementById('rdb').style.position = "29px";
    document.getElementById('rdb').style.backgroundRepeat = "no-repeat";
    document.getElementById('rdc').style.backgroundColor = "#fff";
    document.getElementById('rdc').style.color = "#232f3b";
    document.getElementById('rdc').style.backgroundImage = "url('images/revenduers.png')";
    document.getElementById('rdc').style.position = "29px";
    document.getElementById('rdc').style.backgroundRepeat = "no-repeat";
    document.getElementById('prdcts').style.display = "none";
    document.getElementById('xbtn').style.display = "none";
}

function glry() {
    document.getElementById('prdcts').style.display = "none";
    document.getElementById('xbtn').style.position = "fixed";
    document.getElementById('xbtn').style.overflowY = "scroll";
    document.getElementById('xbtn').style.left = "-420px";
}

function xbtn() {
    document.getElementById('xbtn').style.display = "none";
    document.getElementById('df').style.display = "none";
}

function bdy() {
    document.getElementById('chartdiv').style.width = "100px";
    document.getElementById('txt1').style.opacity = "0";
    document.getElementById('img1').style.float = "left";
    document.getElementById('img2').style.margin = "0 0 0 20px";
    document.getElementById('img3').style.margin = "0 0 0 20px";
    document.getElementById('img4').style.margin = "0 0 0 20px";
    document.getElementById('img1').style.margin = "0 0 0 20px";
    document.getElementById('txt2').style.opacity = "0";
    document.getElementById('img2').style.float = "left";
    document.getElementById('txt3').style.opacity = "0";
    document.getElementById('img3').style.float = "left";
    document.getElementById('txt4').style.opacity = "0";
    document.getElementById('img4').style.float = "left";
    document.getElementById('img1').style.padding = "10px 10px 0 5px";
    document.getElementById('img2').style.padding = "10px 10px 0 5px";
    document.getElementById('img3').style.padding = "10px 10px 0 5px";
    document.getElementById('img4').style.padding = "10px 10px 0 5px";
    document.getElementById('icon').style.background = "url('lgo.jpg') no-repeat center center";
    document.getElementById('icon').style.width = "100px";
}
$("html").click(function(event) {
    var otarget = $(event.target);
    if (!otarget.parents('#xbtn').length && otarget.attr('id') != "xbtn" && !otarget.parents('#mmnu').length) {
        $('#xbtn').hide();
    }
    var otarget = $(event.target);
    if (!otarget.parents('#df').length && otarget.attr('id') != "df" && !otarget.parents('#mmnu').length) {
        $('#df').hide();
    }
    var otarget = $(event.target);
    if (!otarget.parents('#rd1').length && otarget.attr('id') != "rd1" && !otarget.parents('#mmnu').length) {
        $('#rd1').hide();
    }
    var otarget = $(event.target);
    if (!otarget.parents('#rd2').length && otarget.attr('id') != "rd2" && !otarget.parents('#mmnu').length) {
        $('#rd2').hide();
    }
    var otarget = $(event.target);
    if (!otarget.parents('#rd3').length && otarget.attr('id') != "rd3" && !otarget.parents('#mmnu').length) {
        $('#rd3').hide();
    }
    var otarget = $(event.target);
    if (!otarget.parents('#rd4').length && otarget.attr('id') != "rd4" && !otarget.parents('#mmnu').length) {
        $('#rd4').hide();
    }
});

function prdcts() {
    document.getElementById('prdcts').style.position = "fixed";
    document.getElementById('prdcts').style.overflowY = "scroll";
    document.getElementById('prdcts').style.left = "-420px";
    document.getElementById('xbtn').style.display = "none";
}