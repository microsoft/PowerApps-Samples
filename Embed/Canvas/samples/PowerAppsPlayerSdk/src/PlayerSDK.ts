import * as PowerAppsPlayerSdk from "@microsoft/powerappsplayersdk";

// Initialze Player  SDK
async function initSdk(): Promise<void> {
    let options: PowerAppsPlayerSdk.SdkInitializer = {
        hostName: "Player SDK sample"
    }
    return PowerAppsPlayerSdk.initAsync(options).then(function () {
        return Promise.resolve();
    }).catch((error: Error) => {
        return Promise.reject(error.message);
    });
}

// Method to play by App Id
function playAppById(): void {
    const splashScreenColorVal: PowerAppsPlayerSdk.RGBA = {
        b: 100,
        g: 255,
        r: 100,
    };

    // Set player options 
    let options: PowerAppsPlayerSdk.PlayerOptions = {
        parentContainerId: "PlayerIframe",
        externalCorrelationId: "sampleApp.0",
        height: "1000px",
        width: "1000px",
        locale: "en",
        hideAppDetailsOnSplashScreen: true,
        splashScreenColor: splashScreenColorVal,
        // getAccessToken: function (
        //   audience: string, tokenCallback: (token: string) => void, errorCallback: (errorMessage: string) => void) {       
        //   tokenCallback("token");
        // },
    }

    // Initialize player instance by App Id
    const player = PowerAppsPlayerSdk.Player.initPlayerByAppId(options, '<appid>');

    // Subscribe to player events - if you want to perform operations on any of the events
    subscribeToPlayerEvents(player);

    // Registering host functions
    // This should conform to the WADL that was set using PowerApps Authoring SDK
    player.registerHostFunction("ChangeHostBackground", ChangeHostBackground);

    // Render App
    player.renderApp();
}

// Method to play by App LogicalName
function playAppByLogicalName(): void {
    // Set player options 
    let options: PowerAppsPlayerSdk.PlayerOptions = {
        parentContainerId: "PlayerIframe",
        externalCorrelationId: "sampleApp.0",
        height: "900px",
        width: "900px",
    }

    // Initialize player instance by App LogicalName
    const player = PowerAppsPlayerSdk.Player.initPlayerByLogicalName(options, '<logicalname>', '<environmentid>', '<tenantid>');

    // Subscribe to player events - if you want to perform operations on any of the events
    subscribeToPlayerEvents(player);

    // Registering host functions
    // This should conform to the WADL that was set using PowerApps Authoring SDK
    player.registerHostFunction("ChangeHostBackground", ChangeHostBackground);

    // Render App
    player.renderApp();
}

function ChangeHostBackground(color: string) {
    document.body.style.backgroundColor = decodeURIComponent(color);
}

function subscribeToPlayerEvents(playerObj: PowerAppsPlayerSdk.Player) {
    // Event handler that is called when the App is loaded
    playerObj.onAppLoaded.subscribe((appInfo) => {
        console.log("App Loaded: " + appInfo.appId);
        setData(playerObj);
    });

    // Event handler that is called when the App is unloaded
    playerObj.onAppUnload.subscribe((appInfo) => {
        console.log("App Unloaded: " + appInfo.appId);
    });

    // Event handler that is called when the App shows an error
    playerObj.onAppError.subscribe((appInfo) => {
        console.log("Error: " + appInfo.appId)
    });
}

function setData(playerObj: PowerAppsPlayerSdk.Player) {
    // This allows setting the data on the App 
    // Data should conform to the schema that was set while creating or editing the App using PowerApps Authoring SDK
    // This will override the data that was set previously
    // This data is not saved with the App.     
    const data: PowerAppsPlayerSdk.PowerAppsData = [
        {
            Name: "Player User 1",
            Age: 10
        },
        {
            Name: "Player User 2",
            Age: 30
        }]

    playerObj.setData(data);
}

export function initSdkAndPlayAppById() {
    // Check if SDK is initialized then play the App else initialize the SDK and play App
    if (PowerAppsPlayerSdk.isSdkInitialized()) {
        playAppById();
    }
    else {
        initSdk().then(function () {
            playAppById();
        });
    }
}

export function initSdkAndPlayAppByLogicalName() {
    // Check if SDK is initialized then play the App else initialize the SDK and play App
    if (PowerAppsPlayerSdk.isSdkInitialized()) {
        playAppByLogicalName();
    }
    else {
        initSdk().then(function () {
            playAppByLogicalName();
        });
    }
}
