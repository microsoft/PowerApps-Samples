import React, { Component } from 'react';
import { Fabric, SelectionMode, ShimmeredDetailsList, Panel, PanelType, Stack, StackItem, mergeStyleSets, Selection } from 'office-ui-fabric-react';
import { PlainDetails } from './PlainDetails';
import { PowerAppsDetails } from './PowerAppsDetails';

export class ParksView extends Component {
    displayName = ParksView.name;

    constructor(props) {
        super(props);
        this.componentDidMount = this.componentDidMount.bind(this);
        this.dismiss = this.dismiss.bind(this);
        this.appUpdated = this.appUpdated.bind(this);
        this.onSelectionChange = this.onSelectionChange.bind(this);
        this.selection = new Selection({
            onSelectionChanged: () => this.onSelectionChange()
        });

        this.state = {
            parks: [], selectedItem: {}, panelOpen: false, loaded: false, appId: ""
        };

    }

    componentDidMount() {
        fetch('/api/Data/Parks')
            .then(response => response.json())
            .then(data => { this.setState({ parks: data, loaded: true }) });
    }

    columns = [
        { key: "Column1", name: "Park Name", fieldName: "name", minWidth: 100, maxWidth: 200, isResizeable: true },
        { key: "Column2", name: "Street Address", fieldName: "address", minWidth: 100, maxWidth: 400, isResizeable: true },
    ];

    styles = mergeStyleSets({
        stackRoot: {
            height: '100%'
        },
        header: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        },
        content: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: 10
        }
    }
    );

    dismiss() {
        this.setState({ panelOpen: false });
    }

    onSelectionChange() {
        if (!this.selection) {
            return;
        }
        const selectedItem = this.selection.getSelection()[0];
        if (selectedItem) {
            this.setState({ selectedItem: selectedItem, panelOpen: true });
        }
    }

    appUpdated(appId) {
        this.setState({ appId: appId });
        if (this.props.onAppIdChanged) {
            this.props.onAppIdChanged(appId);
        }
    }

    render() {
        return (
            <Fabric>
                <ShimmeredDetailsList
                    setKey="set"
                    items={this.state.parks}
                    enableShimmer={!this.state.loaded}
                    columns={this.columns}
                    selectionMode={SelectionMode.single}
                    selection={this.selection}
                />
                <Panel
                    isOpen={this.state.panelOpen}
                    type={PanelType.medium}
                    isLightDismiss="true"
                    onDismiss={this.dismiss}
                >
                    <Stack vertical className={this.styles.stackRoot}>
                        <StackItem> <PlainDetails item={this.state.selectedItem} /></StackItem>
                        <br />
                        <StackItem grow><PowerAppsDetails item={this.state.selectedItem} appId={this.props.appId} onAppUpdated={this.appUpdated} /></StackItem>
                    </Stack>
                </Panel>
            </Fabric>
        );
    }
}
