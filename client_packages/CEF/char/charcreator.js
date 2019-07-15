// Name / Gender
var firstname = "";
var lastname = "";
var isMale = false;

// HeadBlend
var ShapeFirst = 0;
var ShapeSecond = 0;
var ShapeMix = 0.5;
var SkinMix = 0.5;

// FaceFeatures
var FaceFeatures = new Float32Array(20);


var Blemishes = 0;
var BlemishesOpacity = 255;
var BlemishesColor = 0;
var BlemishesSecColor = 0;

var currentStep;



jQuery(function ($) {
    ShowStep(4);

    $(".nextStep").click(function() {
        console.log("called");
        ShowNextStep();
      });
});

// Gender/Name
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

// Headblend
$(function() {
    $("#ShapeFirst").ionRangeSlider({
        min: 0,
        max: 45,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeFirst = data["from"];
            UpdateHeadBlend();
        }
    });

    $("#ShapeSecond").ionRangeSlider({
        min: 0,
        max: 45,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeSecond = data["from"];
            UpdateHeadBlend();
        }
    });

    $("#ShapeMix").ionRangeSlider({
        min: 0.0,
        max: 1.0,
        from: 0.5,
        step: 0.01,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeMix = data["from"];
            UpdateHeadBlend();
        }
    });

    $("#SkinMix").ionRangeSlider({
        min: 0.0,
        max: 1.0,
        from: 0.5,
        step: 0.01,
        hide_min_max: true,
        onFinish: function (data) {
            SkinMix = data["from"];
            UpdateHeadBlend();
        }
    });
});

// FaceFeatures
$(function() {
    var i;
    for(i=0;i<=19;i++) {
        $("#facefeature" + i).ionRangeSlider({
            min: -1.0,
            max: 1.0,
            from: 0.0,
            step: 0.01,
            hide_min_max: true,
            onFinish: function (data) {
                //FaceFeatures[i] = data["from"];
                var id = $(data.input[0]).attr('id').match(/\d+/)[0];
                FaceFeatures[id] = data["from"];
                console.log(FaceFeatures[id]);
                UpdateFaceFeature(id);
            }
        });
    }

});



// HeadOverlay - Blemishes
$(function() {
    $("#Blemishes").ionRangeSlider({
        min: 0,
        max: 24,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeFirst = data["from"];
            UpdateOverlay();
        }
    });
    $("#BlemishesOpacity").ionRangeSlider({
        min: 0.0,
        max: 1.0,
        from: 1.0,
        step: 0.01,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeMix = data["from"];
            UpdateOverlay();
        }
    });
    $("#BlemishesColor").ionRangeSlider({
        min: 0,
        max: 24,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeFirst = data["from"];
            UpdateOverlay();
        }
    });
    $("#BlemishesSecColor").ionRangeSlider({
        min: 0,
        max: 24,
        from: 0,
        hide_min_max: true,
        onFinish: function (data) {
            ShapeFirst = data["from"];
            UpdateOverlay();
        }
    });
});

function UpdateHeadBlend() {
    //mp.trigger("onUpdateHeadBlend", ShapeFirst, ShapeSecond, SkinMix, ShapeMix, SkinMix);
}

function UpdateFaceFeature(index) {
    //mp.trigger("onUpdateFaceFeature", index, FaceFeatures[index]);
}



function ShowNextStep() {
    currentStep++;
    ShowStep(currentStep);
}

function ShowStep(numba) {
    currentStep = numba;
    $("#step" + numba).show();
    console.log("showed"+currentStep);
    var i;
    for (i = 0; i < 100; i++) {
      if(i != numba) {
        $("#step" + i).hide();
      }
    }
}