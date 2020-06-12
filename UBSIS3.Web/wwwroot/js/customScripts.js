var tpj = jQuery;
tpj.noConflict();

tpj(document).ready(function () {

    //window.onload = function () {
    //    setTimeout(function () {
    //        scrollTo(0, 0);
    //    }, 100); //100ms for example
    //}

    //yazılım sayfasında istenilen yazılıma ynölendirir. (hash id sine göre)
    //var hashcode = window.location.hash;
    //jQuery('html,body').animate({ scrollTop: jQuery('div' + hashcode).offset().top }, 'slow');
    tpj('form#ContactForm').submit(function (e) {
        debugger;
        e.preventDefault();
        var kod = document.getElementById('guvenlikKodu').value;
        var form = tpj('#ContactForm');
        tpj.validator.unobtrusive.parse(form);
        form.validate();

        if (form.valid()) {
            tpj.ajax({
                type: 'POST',
                url: "/Home/DecodeText",
                data: { imageFileName: tpj('#myCodeKey').attr('data-Nam') },
                success: function (response) {
                    if (response.success) {

                        if (response.result == kod) {
                            //tpj("#contactSubmit").trigger("click");
                            document.getElementById("ContactForm").submit();

                            //$("#ContactForm")[0].submit();      
                        }
                        else {
                            toastr.error(tpj('#warnCodeErrorC').val());
                            kod = "";
                            return false;
                        }
                    }
                }
            });
        }
    });

    tpj(window).on('resize', function () {
        if (tpj(window).width() < 980) {
            tpj('.portfolio-desc h5').css('text-align', 'left');
        }
        else {

            tpj('.portfolio-desc h5').css('text-align', 'center');
        }
    })

    var apiRevoSlider = tpj('#rev_slider_k_fullwidth').show().revolution(
        {
            sliderType: "standard",
            sliderLayout: "fullwidth",
            delay: 9000,
            navigation: {
                arrows: { enable: true }
            },
            responsiveLevels: [1240, 1024, 778, 480],
            visibilityLevels: [1240, 1024, 778, 480],
            gridwidth: [1240, 1024, 778, 480],
            gridheight: [600, 768, 960, 720],
        });

    apiRevoSlider.bind("revolution.slide.onloaded", function (e) {
        setTimeout(function () { SEMICOLON.slider.sliderParallaxDimensions(); }, 400);
    });

    apiRevoSlider.bind("revolution.slide.onchange", function (e, data) {
        SEMICOLON.slider.revolutionSliderMenu();
    });

});

function ActivateMacroTab(vall) {
    //debugger;
    //tüm liklerden active clasını kaldırıyorum.
    var aktifLink = document.getElementsByClassName('aaa nav-item nav-link active');
    tpj(aktifLink).removeClass('active');
    //alert(aktifLink.className);

    //tıklanan linkin id sine active clasını ekliyorum
    var linkId = "nav-" + vall + "-tab";
    var obj = document.getElementById(linkId);
    tpj(obj).addClass('active');


    //aktif tabdan active ve show classlarını kaldırıyorum.
    var obj2 = document.getElementsByClassName('aaa tab-pane fade show active');
    tpj(obj2).removeClass('active');
    tpj(obj2).removeClass('show');
    
    //yeni tabı aktive ediyorum.
    var tabId = "nav-" + vall;
    var obj3 = document.getElementById(tabId);
    tpj(obj3).addClass('show');
    tpj(obj3).addClass('active');

    //alert(vall);
    //var path = 'nav-' + vall;
    //var activeDiv = document.getElementById(path);

    //tpj('#nav-tabContent').children().removeClass('show');
    //tpj('#nav-tabContent').children().removeClass('active');

    //activeDiv.className += "show";
    //activeDiv.className += "active";
}

function ActivateMicroTab(param) {
    //debugger;
    //tüm liklerden active clasını kaldırıyorum.
    var aktifLink = document.getElementsByClassName('ccc nav-item nav-link active');
    tpj(aktifLink).removeClass('active');
    //alert(aktifLink.className);

    //tıklanan linkin id sine active clasını ekliyorum
    var linkId = "nav-" + param + "-tab";
    var obj = document.getElementById(linkId);
    tpj(obj).addClass('active');


    //aktif tabdan active ve show classlarını kaldırıyorum.
    var obj2 = document.getElementsByClassName('ccc tab-pane fade show active');
    tpj(obj2).removeClass('active');
    tpj(obj2).removeClass('show');

    //yeni tabı aktive ediyorum.
    var tabId = "nav-" + param;
    var obj3 = document.getElementById(tabId);
    tpj(obj3).addClass('show');
    tpj(obj3).addClass('active');

    //alert(vall);
    //var path = 'nav-' + vall;
    //var activeDiv = document.getElementById(path);

    //tpj('#nav-tabContent').children().removeClass('show');
    //tpj('#nav-tabContent').children().removeClass('active');

    //activeDiv.className += "show";
    //activeDiv.className += "active";
}


function validateNewsletter() {
    //debugger;

    var inputVal = document.getElementById("widget-subscribe-form-email").value;
    var pattern = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;

    var validation = pattern.test(inputVal);
    if (inputVal=='') {
        toastr.warning(document.getElementById("enterMail").value);
        return false;
    }

    if (!validation) {
        toastr.error(document.getElementById("enterValidMail").value);
        return false;
    }

    if (validation) {
        tpj.ajax({
            url: "/Home/AddToNewsletter",
            type: 'POST',
            data: { 'email': inputVal },

            success: function (response) {

                if (response.success) {
                    toastr.success(response.responseText);
                    document.getElementById("emalSubIcon").className = "icon-email2"; 
                    document.getElementById("widget-subscribe-form-email").value = ""; 
                }
                else {
                    toastr.warning(response.responseText);
                    document.getElementById("emalSubIcon").className = "icon-email2";
                    document.getElementById("widget-subscribe-form-email").value = ""; 
                }
            }
            
        }).fail(function () {
            toastr.error(document.getElementById("errorMail").value);
            document.getElementById("emalSubIcon").className = "icon-email2";
            document.getElementById("widget-subscribe-form-email").value = ""; 
        })
    }
}

//salary bilgisi girlirse currency seçimini zorunlu kılar.
jQuery('form#careerForm').submit(function (e) {
    //debugger;
    if (jQuery.isNumeric(jQuery("#Salary").val()) && tpj('#SalaryCurrency').val() == "") {
        jQuery('#SalaryCurrency').focus();
        jQuery('#SalaryCurrency').css("border", "1px solid red");
        jQuery("#currencySpan").css("display", "");
        //toastr.warning("Please select salary currency");
        //e.preventDefault();
        return false;
    }
    else {
        jQuery("#submitCareer").trigger("click");
    }
})

//salary hatasında currency selectlistinin değişen border rengini normale çevirir.
jQuery("#SalaryCurrency").click(function () {

    jQuery("#SalaryCurrency").css("border", "");
    jQuery("#currencySpan").css("display", "none");
})



toastr.options = {
    "closeButton": true,
    "debug": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "onclick": null,
    "showDuration": "400",
    "hideDuration": "1000",
    "timeOut": "10000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}