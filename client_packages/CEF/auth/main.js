function showError(message) {
	$("form").trigger('reset');
	 $('#displayerror-text').text(message);
     $('#displayerror-box').show();
	 
	setTimeout(function(){
     $('#displayerror-box').hide();
	}, 15000);
}

$(function() {
	$("#container-register").addClass("canbehidden");
	$("#container-login").addClass("canbehidden");
});

function ShowLoading() {
	$('.canbehidden').hide();
	$('.loading').show();
}


function HideLoading() {
	$("form").trigger('reset');
	$('.canbehidden').show();
	$('.loading').hide();
}