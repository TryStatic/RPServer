var firstname = "";
var lastname = "";
var isMale = false;
var ShapeFirst = 0;
var ShapeSecond = 0;
var ShapeMix = 0.5;
var SkinMix = 0.5;


var currentStep;



jQuery(function ($) {
    ShowStep(2);

    $("#nextStep").click(function() {
        ShowNextStep();
      });
});


jQuery(function ($) {
    $("#creation_part1").on('submit', function (e) {
        e.preventDefault();
        firstname = $("#firstname").val();
        lastname = $("#lastname").val();

        var radioValue = $("input[name='gender']:checked").val();
        if(radioValue == "male") isMale = true;
        else if(radioValue == "female") isMale = false;

        ShowNextStep();
    });
});


$(function() {
    $("#ShapeFirst").ionRangeSlider({
        min: 0,
        max: 45,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {

        }
    });

    $("#ShapeSecond").ionRangeSlider({
        min: 0,
        max: 45,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {
            
        }
    });

    $("#ShapeMix").ionRangeSlider({
        min: 0.0,
        max: 1.0,
        from: 0.5,
        step: 0.1,
        hide_min_max: true,
        onFinish: function (data) {

        }
    });

    $("#SkinMix").ionRangeSlider({
        min: 0.0,
        max: 1.0,
        from: 0.5,
        step: 0.1,
        hide_min_max: true,
        onFinish: function (data) {

        }
    });
});

function ShowNextStep() {
    currentStep++;
    ShowStep(currentStep);
}

function ShowStep(numba) {
    currentStep = numba;
    $("#step" + numba).show();

    var i;
    for (i = 0; i < 10; i++) {
      if(i != numba) {
        $("#step" + i).hide();
      }
    }
}