function showError(message) {
	 $('#displayerror-text').text(message);
     $('#displayerror-box').show();
	 
	setTimeout(function(){
     $('#displayerror-box').hide();
	}, 15000);
}

