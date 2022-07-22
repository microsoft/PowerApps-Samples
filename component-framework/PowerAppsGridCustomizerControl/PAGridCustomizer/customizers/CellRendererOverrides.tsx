import { Label } from '@fluentui/react';
import * as React from 'react';
import { CellRendererOverrides } from '../types';

export const cellRendererOverrides: CellRendererOverrides = {
    ["Text"]: (props, col) => {
        return <Label style={{ color: 'green' }}>{props.formattedValue}</Label>
    }
}
