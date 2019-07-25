import React, { Component } from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import { IconButton, Panel, PanelType, Fabric } from 'office-ui-fabric-react';
import { ParksView } from './ParksView';
import { Settings } from './Settings';

export class Home extends Component {
    displayName = Home.name

    constructor(props) {
        super(props);
        this.settings = this.settings.bind(this);
        this.dismiss = this.dismiss.bind(this);
        this.appIdChanged = this.appIdChanged.bind(this);
        this.state = {
            appId: "", showSettings: false
        };
    }

    settings() {
        this.setState({ showSettings: true });
    }

    dismiss() {
        this.setState({ showSettings: false });
    }

    appIdChanged(appId) {
        this.setState({ appId: appId });
        this.dismiss();
    }

    render() {
        return (
            <Fabric>
                <Grid fluid>
                    <Row >
                        <Col sm={10}>
                            <h1>National Parks</h1>
                        </Col>
                        <Col sm={1}>
                            <IconButton iconProps={{ iconName: "Settings" }} title="Settings" onClick={this.settings} />
                        </Col>
                    </Row>
                    <Row>
                        <ParksView appId={this.state.appId} onAppIdChanged={this.appIdChanged} />
                    </Row>
                </Grid>
                <Panel
                    headerText="Settings"
                    isOpen={this.state.showSettings}
                    type={PanelType.medium}
                    isLightDismiss="true"
                    onDismiss={this.dismiss}
                >
                    <Settings appId={this.state.appId} onAppIdChanged={this.appIdChanged} />
                </Panel>
            </Fabric>
        );
    }
}
