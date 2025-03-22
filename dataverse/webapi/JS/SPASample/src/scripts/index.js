import { PublicClientApplication } from "@azure/msal-browser";
import "dotenv/config";
import { DataverseWebAPI } from "./DataverseWebAPI.js";
import { Util } from "./Util.js";
import { TemplateSample } from "../samples/TemplateSample.js";
import { BasicOperationsSample } from "../samples/BasicOperationsSample.js";
import { QueryDataSample } from "../samples/QueryDataSample.js";
import { ConditionalOperationsSample } from "../samples/ConditionalOperationsSample.js";
import { FunctionsAndActions } from "../samples/FunctionsAndActions.js";
import { BatchSample } from "../samples/BatchSample.js";

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

// Set DataverseWebAPI.Client value when user logs in
let client = null;

// body/main element where messages are displayed
const container = document.getElementById("container");

const util = new Util(container);

// <logIn>
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
      client = new DataverseWebAPI.Client(config.baseUrl, getToken);

      document.getElementById("loginButton").style.display = "none";

      const logoutButton = document.getElementById("logoutButton");
      logoutButton.innerHTML = "Logout " + response.account.name;
      logoutButton.style.display = "block";
      document.getElementsByTagName("nav")[0].classList.remove("disabled");
    } catch (error) {
      let p = document.createElement("p");
      p.textContent = "Error logging in: " + error;
      p.className = "error";
      container.append(p);
    }
  } else {
    msalInstance.setActiveAccount(null);
    this.click();
  }
}
// </logIn>

// <logOut>
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

  // Clear the client instance
  client = null;
}
// </logOut>

// <getToken>
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
// </getToken>

// <runSample>
// Runs all samples in a consistent way
async function runSample(sample) {
  // Disable the buttons to prevent multiple clicks
  document.getElementsByTagName("nav")[0].classList.add("disabled");

  // Disable the logout button while the sample is running
  logoutButton.classList.add("disabled");

  // Run the sample
  await sample.SetUp();
  await sample.Run();
  await sample.CleanUp();

  // Re-enable the buttons
  document.getElementsByTagName("nav")[0].classList.remove("disabled");
  logoutButton.classList.remove("disabled");
}
// </runSample>

//#region verify that configuration has been updated.

if (config.baseUrl === "https://<your org>.api.crm.dynamics.com") {
  console.error(`Placeholder baseUrl ${config.baseUrl} found.`);
  util.showError(
    "Update the BASE_URL in the .env file to your Dataverse instance URL."
  );
}

if (config.clientId == "00001111-aaaa-2222-bbbb-3333cccc4444") {
  console.error(`Placeholder clientId ${config.clientId} found.`);
  util.showError(
    "Update the CLIENT_ID in the .env file to your Azure AD application client ID."
  );
}

if (config.tenantId == "aaaabbbb-0000-cccc-1111-dddd2222eeee") {
  console.error(`Placeholder tenantId ${config.tenantId} found.`);
  util.showError(
    "Update the TENANT_ID in the .env file to your Azure AD application client ID."
  );
}

const redirectUri = new URL(config.redirectUri);
const baseUrl = `${redirectUri.protocol}//${redirectUri.host}/`;

if (location.href.startsWith(baseUrl) === false) {
  console.error(
    `Placeholder redirectUri ${config.redirectUri} doesn't match ${location.href}.`
  );
  util.showError(
    "Update the REDIRECT_URI in the .env file to your Azure AD application redirect URI."
  );
}

//#endregion verify that configuration has been updated.

//#region Add Event Listeners

// Add event listener to the login button
document.getElementById("loginButton").onclick = logIn;

// Add event listener to the logout button
document.getElementById("logoutButton").onclick = logOut;

// Add event listener to the template button
document.getElementById("templateButton").onclick = async function () {
  runSample(new TemplateSample(client, container));
};

// Add event listener to the basic operations button
document.getElementById("basicOperationsButton").onclick = async function () {
  runSample(new BasicOperationsSample(client, container));
};

// Add event listener to the query data button
document.getElementById("queryDataButton").onclick = async function () {
  runSample(new QueryDataSample(client, container));
};

// Add event listener to the conditional operations button
document.getElementById("conditionalOperationsButton").onclick =
  async function () {
    runSample(new ConditionalOperationsSample(client, container));
  };

document.getElementById("functionsAndActionsButton").onclick =
  async function () {
    runSample(new FunctionsAndActions(client, container));
  };

// Add event listener to the batch button
document.getElementById("batchButton").onclick = async function () {
  runSample(new BatchSample(client, container));
};
