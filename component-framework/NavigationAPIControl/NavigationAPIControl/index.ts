/*
	This file is part of the Microsoft PowerApps code samples.
	Copyright (C) Microsoft Corporation.  All rights reserved.
	This source code is intended only as a supplement to Microsoft Development Tools and/or
	on-line documentation.  See these other materials for detailed information regarding
	Microsoft code samples.

	THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER
	EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
	MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */

import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class NavigationAPIControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens.
	private _notifyOutputChanged: () => void;

	// Reference to the div element that hold together all the HTML elements that we are creating as part of this control
	private divElement: HTMLDivElement;

	// Reference to the button that invokes the openAlertDialog command
	private openAlertDialogButton: HTMLButtonElement;

	// Reference to the button that invokes the openConfirmDialog command
	private openConfirmDialogButton: HTMLButtonElement;

	// Reference to the button that invokes the openFile command
	private openFileButton: HTMLButtonElement;

	// Reference to the button that invokes the openUrl command
	private openUrlButton: HTMLButtonElement;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Reference to ComponentFramework Context object
	private _context: ComponentFramework.Context<IInputs>;

	/**
	 * Empty constructor.
	 */
	constructor() {
		// no-op: method not leveraged by this example custom control
	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	): void {
		this._notifyOutputChanged = notifyOutputChanged;
		this._context = context;
		this._container = container;

		this.divElement = document.createElement("div");
		this.divElement.setAttribute("class", "NavigationAPI");

		// Create the HTML button elements for openAlertDialog button
		this.openAlertDialogButton = document.createElement("button");
		this.openAlertDialogButton.setAttribute("id", "openAlertDialogButton");
		this.openAlertDialogButton.innerHTML = "openAlertDialogButton";

		// Create the HTML button elements for openConfirmDialog button
		this.openConfirmDialogButton = document.createElement("button");
		this.openConfirmDialogButton.setAttribute("id", "openConfirmDialogButton");
		this.openConfirmDialogButton.innerHTML = "openConfirmDialogButton";

		// Create the HTML button elements for openFile button
		this.openFileButton = document.createElement("button");
		this.openFileButton.setAttribute("id", "openFileButton");
		this.openFileButton.innerHTML = "openFileButton";

		// Create the HTML button elements for openUrl button
		this.openUrlButton = document.createElement("button");
		this.openUrlButton.setAttribute("id", "openUrlButton");
		this.openUrlButton.innerHTML = "openUrlButton";

		// bind the function which invokes the respective API's to each of the buttons
		this.openAlertDialogButton.addEventListener("click", this.raiseEvent.bind(this));
		this.openConfirmDialogButton.addEventListener("click", this.raiseEvent.bind(this));
		this.openFileButton.addEventListener("click", this.raiseEvent.bind(this));
		this.openUrlButton.addEventListener("click", this.raiseEvent.bind(this));

		// append all the button elements to the parent div element for control.
		this.divElement.appendChild(this.openAlertDialogButton);
		this.divElement.appendChild(this.openConfirmDialogButton);
		this.divElement.appendChild(this.openFileButton);
		this.divElement.appendChild(this.openUrlButton);

		// append the parent div element in the control to the control's container
		this._container.appendChild(this.divElement);
	}

	/**
	 * Handles the events raised by each of the buttons that are binded according to their id
	 * @param event : event object that contains all the properties regarding the raised event
	 */
	public raiseEvent(event: Event): Promise<unknown> | void {
		const inputSource = (event.target as Element)?.id;
		switch (inputSource) {
			case "openAlertDialogButton":
				return this._context.navigation.openAlertDialog({ text: "This is an alert.", confirmButtonLabel: "Yes" }).then(
					() => (this.openAlertDialogButton.innerHTML = "Alert dialog closed"),
					() => (this.openAlertDialogButton.innerHTML = "Error in Alert Dialog")
				);

			case "openConfirmDialogButton":
				return this._context.navigation
					.openConfirmDialog(
						{ title: "Confirmation Dialog", text: "This is a confirmation." },
						{ height: 200, width: 450 }
					)
					.then((success) => {
						if (success.confirmed) {
							this.openConfirmDialogButton.innerHTML = "Ok button clicked.";
						} else {
							this.openConfirmDialogButton.innerHTML = "Cancel or X button clicked.";
						}
						return;
					});

			case "openFileButton": {
				const file: ComponentFramework.FileObject = {
					fileContent: "U2FtcGxlIGNvbnRlbnQgZm9yIERlbW8=", //Contents of the file in base64 encoded format.
					fileName: "Sample.txt", //Name of the file.
					fileSize: 29, //Size of the file in KB.
					mimeType: "text/plain", //MIME type of the file.
				};
				return this._context.navigation.openFile(file, { openMode: 2 });
			}

			case "openUrlButton":
				return this._context.navigation.openUrl("https://www.microsoft.com");
		}

		//this._notifyOutputChanged();
		return;
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this._context = context;
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		// no-op: method not leveraged by this example custom control
		return {};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
