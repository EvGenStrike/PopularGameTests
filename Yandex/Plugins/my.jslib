mergeInto(LibraryManager.library, {

	Hello: function() {
		window.alert("Hello, world!");
		console.log("testing)");
	},

	ShowAdv: function(){
		ysdk.adv.showFullscreenAdv({
    	callbacks: {
        	onClose: function(wasShown) {
        		console.log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
          	// some action after close
        },
        	onError: function(error) {
          	// some action on error
        }
    	}
		})
	},

})