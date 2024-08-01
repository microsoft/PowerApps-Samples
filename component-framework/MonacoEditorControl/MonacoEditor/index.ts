/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 */

/* eslint-disable */

import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { Editor, IEditorProps } from "./components/editor";
import * as React from "react";

export class MonacoEditor implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    private notifyOutputChanged: () => void;
    private currentValue: string;
    private _isLoaded: boolean = false;
    private _defaultString: string = "";
    private _defaultLanguage: string = "";

    constructor() { }

    public init(
        context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void,
        state: ComponentFramework.Dictionary
    ): void {
        this.notifyOutputChanged = notifyOutputChanged;
    }

    public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
        this._defaultString = context.parameters.selectedRecord.raw ? context.parameters.selectedRecord.raw : "";
        this._defaultLanguage = context.parameters.languageSelectionMode.raw?  context.parameters.languageSelectionMode.raw : "liquid";
        const props: IEditorProps = { 
            callback: this.callback.bind(this),
            defaultValue: this._defaultString,
            defaultLanguage: this._defaultLanguage
        };
        return React.createElement(
            Editor, props
        );
    }

    public callback(newString: string): void {
        this.currentValue = newString;
        this.notifyOutputChanged();
    }

    public getOutputs(): IOutputs {
        return { selectedRecord: this.currentValue };
    }

    public destroy(): void {
        // Add code to cleanup control if necessary
    }
}