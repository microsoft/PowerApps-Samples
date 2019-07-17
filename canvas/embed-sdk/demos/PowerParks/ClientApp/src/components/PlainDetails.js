import React, { Component } from 'react';
import { Stack, mergeStyleSets, DefaultPalette, StackItem, TextField} from 'office-ui-fabric-react';

export class PlainDetails extends Component {
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
        field: {
            width: "100%"
        }
    }
    );

    render() {
        return (
            <Stack vertical disableShrink className={this.styles.stackRoot} padding={5} >
                <StackItem className={this.styles.header} grow><h2>{this.props.item.name}</h2></StackItem>
                <StackItem className={this.styles.content} grow>
                    <TextField className={this.styles.field} label="Address:" resizable={false} defaultValue={this.props.item.address} />    
                </StackItem>
                <StackItem className={this.styles.content} grow disableShrink styles={{ root: { height: 100 } }}>
                    <TextField className={this.styles.field} label="Notes:" multiline rows={3} resizable={false} defaultValue={this.props.item.notes}/>    
                </StackItem>
            </Stack>
            );
    }

}
