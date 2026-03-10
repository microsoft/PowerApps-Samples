import { PublicClientApplication } from "@azure/msal-browser";
import 'dotenv/config'

// Load the environment variables from the .env file
const config = {
   baseUrl: process.env.BASE_URL,
   clientId: process.env.CLIENT_ID,
   tenantId: process.env.TENANT_ID,
   redirectUri: process.env.REDIRECT_URI,
};

// Microsoft Authentication Library (MSAL) configuration
const msalConfig = {
  auth: {
    clientId: config.clientId,
    authority: "https://login.microsoftonline.com/" + config.tenantId,
    redirectUri: config.redirectUri,
    postLogoutRedirectUri: config.redirectUri,
  },
  cache: {
    cacheLocation: "sessionStorage", // This configures where your cache will be stored
    storeAuthStateInCookie: true,
  },
};

// Create an instance of MSAL
const msalInstance = new PublicClientApplication(msalConfig);

// body/main element where messages are displayed
const container = document.getElementById("container");

// Event handler for login button
async function logIn() {
  await msalInstance.initialize();

  if (!msalInstance.getActiveAccount()) {
    const request = {
      scopes: ["User.Read", config.baseUrl + "/user_impersonation"],
    };
    try {
      const response = await msalInstance.loginPopup(request);
      msalInstance.setActiveAccount(response.account);

      // Hide the loginButton so it won't get pressed twice
      document.getElementById("loginButton").style.display = "none";

      // Show the logoutButton
      const logoutButton = document.getElementById("logoutButton");
      logoutButton.innerHTML = "Logout " + response.account.name;
      logoutButton.style.display = "block";
      // Enable any buttons in the nav element
      document.getElementsByTagName("nav")[0].classList.remove("disabled");
    } catch (error) {
      let p = document.createElement("p");
      p.textContent = "Error logging in: " + error;
      p.className = "error";
      container.append(p);
    }
  } else {
    // Clear the active account and try again
    msalInstance.setActiveAccount(null);
    this.click();
  }
}

// Event handler for logout button
async function logOut() {
  const activeAccount = await msalInstance.getActiveAccount();
  const logoutRequest = {
    account: activeAccount,
    mainWindowRedirectUri: config.redirectUri,
  };

  try {
    await msalInstance.logoutPopup(logoutRequest);

    document.getElementById("loginButton").style.display = "block";

    this.innerHTML = "Logout ";
    this.style.display = "none";
    document.getElementsByTagName("nav")[0].classList.remove("disabled");
  } catch (error) {
    console.error("Error logging out: ", error);
  }
}

/**
 * Retrieves an access token using MSAL (Microsoft Authentication Library).
 * Set as the getToken function for the DataverseWebAPI client in the login function.
 *
 * @async
 * @function getToken
 * @returns {Promise<string>} The access token.
 * @throws {Error} If token acquisition fails and is not an interaction required error.
 */
async function getToken() {
  const request = {
    scopes: [config.baseUrl + "/.default"],
  };

  try {
    const response = await msalInstance.acquireTokenSilent(request);
    return response.accessToken;
  } catch (error) {
    if (error instanceof msal.InteractionRequiredAuthError) {
      const response = await msalInstance.acquireTokenPopup(request);
      return response.accessToken;
    } else {
      console.error(error);
      throw error;
    }
  }
}

// Add event listener to the login button
document.getElementById("loginButton").onclick = logIn;

// Add event listener to the logout button
document.getElementById("logoutButton").onclick = logOut;

/// Function to get the current user's information
/// using the WhoAmI function of the Dataverse Web API.
async function whoAmI() {
  const token = await getToken();
  const request = new Request(config.baseUrl + "/api/data/v9.2/WhoAmI", {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
      Accept: "application/json",
      "OData-Version": "4.0",
      "OData-MaxVersion": "4.0",
    },
  });
  // Send the request to the API
  const response = await fetch(request);
  // Handle the response
  if (!response.ok) {
    throw new Error("Network response was not ok: " + response.statusText);
  }
  // Successfully received response
  return await response.json();
}

// Add event listener to the whoAmI button
document.getElementById("whoAmIButton").onclick = async function () {
  // Clear any previous messages
  container.replaceChildren();
  try {
    const response = await whoAmI();
    let p1 = document.createElement("p");
    p1.textContent =
      "Congratulations! You connected to Dataverse using the Web API.";
    container.append(p1);
    let p2 = document.createElement("p");
    p2.textContent = "User ID: " + response.UserId;
    container.append(p2);
  } catch (error) {
    let p = document.createElement("p");
    p.textContent = "Error fetching user info: " + error;
    p.className = "error";
    container.append(p);
  }
};
