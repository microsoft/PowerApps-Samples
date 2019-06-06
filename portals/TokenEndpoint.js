/* 
    Portals - Implicit Grant Sample
    
    Token Endpoint

    Description:
        * getAuthenticationToken function is used to fetch an ID token using the Token endpoint in Portals.
        * getAuthenticationToken is added to window.auth using an IIFE.
        * After the IIFE gets executed, the token can be fetched anytime using window.auth.getAuthenticationToken(callback);
        * getAuthenticationToken accepts a callback function to handle the token returned from Portals.
        * Token is returned only for authenticated users.
        * ImplicitGrantForceLogin cookie key is used to decide whether or not to redirect user to login page based on the Token endpoint's response.
*/

// Part 1: Add getAuthenticationToken to window.auth

(function (auth, $) {
    "use strict";

    var callback = null;

    //Force Login for Anonymous users. Below line can be commented if force login is not needed.
    $.cookie("ImplicitGrantForceLogin", 1);

	let clientId = "6d08757f-4f46-e911"; //Add the Client ID registered on CRM.

    auth.getAuthenticationToken = function (callbackFn) {
        callback = callbackFn;
        
        $.ajax({
            type: 'GET',
            url: `/_services/auth/token?client_id=${clientId}`,
            cache: false,
            success: handleGetAuthenticationTokenSuccess,
            error: jqXHR => callback(null, jqXHR)
        });
    }

    function handleGetAuthenticationTokenSuccess(data, status, jqXHR) {
        var jsonResult = JSON.parse(jqXHR.getResponseHeader('X-Responded-JSON'));
        if (jsonResult && jsonResult.status == 401) {
            var forceLogin = Number($.cookie("ImplicitGrantForceLogin")) === 1;
            if (forceLogin) {
                // If the user is not logged in, redirect to login page
                redirectToLogin();
            } else {
                callback(null,jsonResult); //Run callback method with error message
            }
        } else {
            // Pass the token to the callback function
            callback(data);
        }
    }

    function redirectToLogin() {
        var redirectUrl = window.location;
        var loginUrl = window.location.origin + '/SignIn?returnUrl=' + encodeURIComponent(redirectUrl);
        window.location = loginUrl;
    }

}(window.auth || (window.auth = {}), window.jQuery));


// Part 2: Define Callback function and call window.auth.getAuthenticationToken to fetch the token.

var callbackFn = function(data, err) { 
	if(data) {
		//Token received
		console.log(data);
	}
	else {
		console.log("Error");
		if(err){
            // 401 is returned for anonymous users.
			if(err.status == 401){
				console.log("Login required");
            }
            // To handle any other error
			else{
				console.log(err.responseJSON.ErrorId + " : " + err.responseJSON.ErrorMessage);
            }
        }
    }
}

// Fetch Token Call
window.auth.getAuthenticationToken(callbackFn);