import React, { Component } from 'react';
import { Stack, PrimaryButton, DefaultButton, TextField } from 'office-ui-fabric-react';

export class Settings extends Component {

    constructor(props) {
        super(props);
        this.change = this.change.bind(this);
        this.save = this.save.bind(this);
        this.clear = this.clear.bind(this);
        this.state = { appId: "" };
    }

    componentDidMount() {
        this.setState({ appId: this.props.appId });
    }

    change(e, value) {
        var appId = "/providers/Microsoft.PowerApps/apps/" + value;
        this.setState({ appId: appId });
    }

    save() {
        if (this.props.onAppIdChanged) {
            this.props.onAppIdChanged(this.state.appId);
        }
    }

    clear() {
        this.setState({ appId: "" });
    }

    render() {
        return (
            <Stack vertical>
                <TextField label="App ID:" placeholder="Enter app id here" defaultValue={this.state.appId} onChange={this.change} />
                <DefaultButton text="Clear" onClick={this.clear} />
                <PrimaryButton text="Save" onClick={this.save} />
            </Stack>
        );
    }
}
