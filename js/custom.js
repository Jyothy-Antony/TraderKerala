	////SCRIPT FOR MODAL POPUP WINDOW

$(document).ready(function(){	
	$('#francie').click(function(){
								  alert("hi");
			var visits = $.cookie('visits') || 0;
	visits++;
	
	$.cookie('visits', visits, { expires: 1, path: '/' });
		
	console.debug($.cookie('visits'));
		
	});
	
	
	if ( $.cookie('visits') > 1 ) {
		$('#popup-container').hide();
		window.location = "http://www.gmail.com";
	} else {
			var pageHeight = $(document).height();
			
			$('#popup-container').show();
	}

	if ($.cookie('noShowWelcome')) { $('#popup-container').hide(); $('#active-popup').hide(); }
});	

$(document).mouseup(function(e){
	var container = $('#popup-container');
	
	if( !container.is(e.target)&& container.has(e.target).length === 0)
	{
		container.fadeOut();
		$('#active-popup').fadeOut();
	}

});
