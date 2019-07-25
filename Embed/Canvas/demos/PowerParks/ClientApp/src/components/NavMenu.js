import * as React from 'react';
import { Nav } from 'office-ui-fabric-react';

export class NavMenu extends React.Component {
    displayName = NavMenu.name

    render() {
        return (
            <Nav groups={
                [
                    {
                        name: 'Power Bucketlist',
                        links: [
                            {
                                key: 'Home',
                                name: 'National Parks',
                                url: '/'
                            }
                        ]
                    }
                ]
            }
            />
        );
    }
}

