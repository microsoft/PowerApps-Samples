/* 
    Portals - Implicit Grant Sample
    
    Authorize Endpoint

    Description:
		* Authorize Endpoint returns the ID token as a fragment in the Redirected URL.
		* This sample also covers state validation supported in Implicit Grant.
		* There are two parts in Authorize Endpoint Sample.
		* Part 1: Redirect to Authorize Endpoint
			* Save state as a cookie.
			* Call Authorize Endpoint with required parameters.
		* Part 2: Handle response in redirected url
			* Fetch the parameters in url fragment.
			* State validation
			* Return the token.
*/

//Part 1 - Call Authorize Endpoint to Get Token

<button onclick="callAuthorizeEndpoint()">Authenticate using Authorize Endpoint</button>


<script>

//Remove this line to avoid State validation
$.cookie("useStateValidation",1); 

function callAuthorizeEndpoint(){
	//Used for State validation
	var useStateValidation = $.cookie("useStateValidation");
	var appStateKey = 'my@pp$tate';
	var sampleAppState = {id:500, name:"logic"};

	//Replace with Client Id Registered on CRM
	var clientId = "8ebd73587a354f948ec-93260410010"; //Sample ID

	//Replace with Redirect URL registered on CRM
	var redirectUri = encodeURIComponent("https://mbs7support.microsoftcrmportals.com/support");

	//Authorize Endpoint
	var redirectLocation = `/_services/auth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}`;

	//Save state in a cookie if State validation is enabled
	if(useStateValidation){
		$.cookie(appStateKey, JSON.stringify(sampleAppState));
		redirectLocation = redirectLocation + `&state=${appStateKey}`;
		console.log("Added State Parameter");
	}

	//Redirect
	window.location = redirectLocation;
}
</script>


//Part 2 - Handle Response in Redirected URL. Retrieve Token from URL fragment

// Note: For Authorize Endpoint, the below javascript is executed on the redirected page with token in url fragment

//Convert URL Fragment to Result Object
function getResultInUrlFragment(hash){
    if(hash){
        var result = {};
        hash.substring("1").split('&').forEach(function(keyValuePair){
            var arr = keyValuePair.split('=');
			//  Add to result, only the keys with values
            arr[1] && (result[arr[0]] = arr[1]);
        });
		return result;
    }
	else{
		return null;
    }
}

//Validate State parameter
//Returns true for valid state and false otherwise
function validateState(stateInUrlFragment){
	if(!stateInUrlFragment){
		console.error("State Validation Failed. State parameter not found in URL fragment");
		return false;
    }
	// State parameter in URL Fragment doesn't have a corresponding cookie.
	if(!$.cookie(stateInUrlFragment)){
		console.error("State Validation Failed. Invalid state parameter");
		return false;
    }
	return true;
}

var useStateValidation = $.cookie("useStateValidation");
var appState = null;

//Fetch the parameters in Url fragment
var authorizeEndpointResult = getResultInUrlFragment(window.location.hash);

//Validate State
if(useStateValidation){
	if(!validateState(authorizeEndpointResult.state)){
		authorizeEndpointResult = null;
    }
	else{
		appState = $.cookie(authorizeEndpointResult.state);	
		console.log("State: "+appState);
    }
}

//Display token
if(authorizeEndpointResult){
	console.log("Token:" + authorizeEndpointResult.token);
}