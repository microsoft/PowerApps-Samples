import React, { Component } from 'react';
import * as PowerAppsSDK from '@microsoft/powerappsauthoringsdk';
import { Stack, StackItem, mergeStyleSets, DefaultPalette, PrimaryButton } from 'office-ui-fabric-react';

let gSdkInited = false;

export class PowerAppsDetails extends Component {
    // constructor 
    constructor(props) {
        super(props);
        this.AppName = "PowerBucketListBuildDemo";
        this.sessionStarted = this.sessionStarted.bind(this);
        this.appPublished = this.appPublished.bind(this);
        this.appSaved = this.appSaved.bind(this);
        this.componentDidMount = this.componentDidMount.bind(this);
        this.sessionStarted = this.sessionStarted.bind(this);
        this.fillMakerOptions = this.fillMakerOptions.bind(this);
        this.onLaunchMaker = this.onLaunchMaker.bind(this);
        this.getEmbeddedUrl = this.getEmbeddedUrl.bind(this);
        this.renderAuthoring = this.renderAuthoring.bind(this);
        this.renderPlayer = this.renderPlayer.bind(this);
        this.componentWillUnmount = this.componentWillUnmount.bind(this);

        this.state = {
            appPublished: false,
            appSaved: false,
            makerSession: null,
            isEditing: false
        };
    }

    async componentDidMount() {
        let appId;
        // Set the initial state of the component                 
        if (this.props.appID !== "")
            appId = window.localStorage.getItem("PowerPark_AppID");

        appId = appId ? appId : this.props.appId;

        this.setState({
            appId: appId ? appId : this.props.appId,
            item: this.props.item,
            appPublished: appId ? true : false
        });        
    }

    componentWillUnmount() {
        if (this.state.makerSession !== null) {
            this.state.makerSession.disposeAsync();
        }
    }

    styles = mergeStyleSets({
        stackRoot: {
        },
        content: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: 10
        },
        header: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: 10
        },
        item: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: 10
        }
    }
    );    
    
    appSaved(appInfo) {
        this.setState({ appSaved: true });
    }

    fillMakerOptions() {
        let makerOptions = {
            formFactor: PowerAppsSDK.FormFactor.Phone
        };
        if (this.state.appId !== "") {
            makerOptions["appId"] = this.state.appId;
        }
        return makerOptions;
    }

    // Creates a new PowerApps Studio session
    async onLaunchMaker() {
        // Step 1: Initialize the SDK       
        if (!gSdkInited) {
            await PowerAppsSDK.initAsync({ hostName: this.AppName });
            gSdkInited = true;
        }

      
        // Step 2: Set the session options       
        let sessionOptions = this.fillMakerOptions();
        this.setState({ isEditing: true });

        // Step 3: Launch the studio
        PowerAppsSDK.MakerSession.startAsync(sessionOptions).then(this.sessionStarted);
    }

    // Step 4: Handle the session starting - update state and subscribe to the published event
    sessionStarted(session) {
        this.setState({ makerSession: session, appSaved: false, appPublished: false });
        session.appPublished.subscribe(this.appPublished);
        session.appSaved.subscribe(this.appSaved);
    }
    
    // Step 5: Event handler that is called when the user publishes the app in PowerApp Studio
    appPublished(appInfo) {
        this.state.makerSession.disposeAsync();
        this.setState({ makerSession: null, appId: appInfo.appId, appPublished: true, isEditing: false });
        if (this.props.onAppUpdated !== null) {
            this.props.onAppUpdated(appInfo.appId);
        }
    }

    // Simple rendering describing what is happening with the authoring session
    renderAuthoring() {
        return (
            <StackItem className={this.styles.content} grow disableShrink>
                {this.state.makerSession !== null ? "Finish editing your app in the maker studio and publish it to see it here." : "Create a PowerApp for this view by clicking the button below."}
            </StackItem>
        );
    }

    // Step 1: Helper method that constructs the URL to the PowerApp, also adds an extra query parameter to force the iframe to refresh
    getEmbeddedUrl() {        
        let rand = Math.floor((Math.random() * 1000) + 1);

        // Set the Bing API here
        let bingAPIKey = "";
        return "https://web.powerapps.com/webplayer/iframeapp?source=builddemo&appId=" + this.state.appId + "&Address=" + encodeURI(this.props.item.address) + "&BingAPIKey=" + bingAPIKey + "&extra=" + rand;
    }

    // Step 2: Render method for creating the iframe that will host the PowerApps player
    renderPlayer() {
        return (
            <iframe width="100%" height="100%" frameBorder="0" src={this.getEmbeddedUrl()} allow="geolocation https://web.powerapps.com https://preview.web.powerapps.com ; microphone; camera" />
        );
    }

    // Main render method that either renders a prompt to add a PowerApp, or actually renders the PowerApp    
    render() {
        return (
            <Stack vertical disableShrink className={this.styles.stackRoot} padding={5} tokens={{ childrenGap: 10 }} >
                {this.state.appPublished ?
                    <StackItem className={this.styles.content} grow disableShrink styles={{ root: { height: 500 } }}> {this.renderPlayer()} </StackItem> :
                    <StackItem className={this.styles.content} grow disableShrink> {this.renderAuthoring()} </StackItem>
                }
                <StackItem className={this.styles.item} grow><PrimaryButton disabled={this.state.makerSession !== null} onClick={this.onLaunchMaker} >{this.state.appPublished ? "Edit this app" : "Add a PowerApp"}</PrimaryButton></StackItem>
            </Stack>
        );
    }
}
