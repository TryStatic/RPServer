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
var FaceFeatures = [];
var FaceFeaturesNames = [
    "Nose Width",
    "Nose Height",
    "Nose Length",
    "Nose Bridge",
    "Nose Tip",
    "NoseBridge Shift",
    "Brow Height",
    "Brow Width",
    "Cheekbone Height",
    "Cheekbone Width",
    "Cheeks Width",
    "Eyes",
    "Lips",
    "Jaw Width",
    "Jaw Height",
    "Chin Length",
    "Chin Position",
    "Chin Width",
    "Chin Shape",
    "Neck Width"
];

// Overlays
var Overlays = [];
var OverlayNames = [
    "Blemishes",
    "FacialHair",
    "Eyebrows",
    "Ageing",
    "Makeup",
    "Blush",
    "Complexion",
    "SunDamage",
    "Lipstick",
    "Freckles",
    "ChestHair",
    "BodyBlemishes",
    "Additional BodyBlemishes"
];

var currentStep;



jQuery(function ($) {
    ShowStep(3);

    var i;
    for(i=0;i<20;i++) {
        FaceFeatures.push(0.0);
        var faceFeatureHTML = `${FaceFeaturesNames[i]}<input id="facefeature${i}" type="range">`;
        $(".allfacefeatures").append(faceFeatureHTML);

    }

    for(i=0;i<13;i++) {
        Overlays.push([255, 0, 0, 0]);
        
        var overlayHTML = `
        <div class="overlay">
        <p>${OverlayNames[i]}</p>
        Variation:
        <input id="overlay${i}" type="range">
        Opacity:
        <input id="overlayOpacity${i}" type="range">
        Color:
        <input id="overlayColor${i}" type="range">
        Secondary Color:
        <input id="overlaySecColor${i}" type="range">
        <br />
        </div>
        `;

        $(".alloverlays").append(overlayHTML);

    }

    $(".nextStep").click(function() {

        $('.customscroll').animate({
            scrollTop: $(".innercontainer").offset().top
        }, 1000);

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



// HeadOverlay
$(function() {
    var i;
    for(i=0;i<=12;i++) {
        $("#overlay" + i).ionRangeSlider({
            min: 0,
            max: 75,
            from: 0,
            hide_min_max: true,
            onFinish: function (data) {
                var id = $(data.input[0]).attr('id').match(/\d+/)[0];
                value = data["from"];
                if(value == 0) value = 255;
                Overlays[id][0] = value;
                console.log(Overlays[id]);
            }
        });
        $("#overlayOpacity" + i).ionRangeSlider({
            min: 0.0,
            max: 1.0,
            from: 0.0,
            step: 0.01,
            hide_min_max: true,
            onFinish: function (data) {
                var id = $(data.input[0]).attr('id').match(/\d+/)[0];
                Overlays[id][1] = data["from"];
                console.log(Overlays[id]);
            }
        });
        $("#overlayColor" + i).ionRangeSlider({
            min: 0,
            max: 100,
            from: 0,
            hide_min_max: true,
            onFinish: function (data) {
                var id = $(data.input[0]).attr('id').match(/\d+/)[0];
                Overlays[id][2] = data["from"];
                console.log(Overlays[id]);
            }
        });
        $("#overlaySecColor" + i).ionRangeSlider({
            min: 0,
            max: 100,
            from: 0,
            hide_min_max: true,
            onFinish: function (data) {
                var id = $(data.input[0]).attr('id').match(/\d+/)[0];
                Overlays[id][3] = data["from"];
                console.log(Overlays[id]);
            }
        });

    }
});

function UpdateHeadBlend(index) {
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