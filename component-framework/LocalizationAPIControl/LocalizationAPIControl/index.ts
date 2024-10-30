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

export class LocalizationAPIControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Value of the field is stored and used inside the control
	private _value: number;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens.
	private _notifyOutputChanged: () => void;

	// label element created as part of this control
	private label: HTMLInputElement;

	// button element created as part of this control
	private button: HTMLButtonElement;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

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
		// Creating the label for the control and setting the relevant values.
		this.label = document.createElement("input");
		this.label.setAttribute("type", "label");
		this.label.addEventListener("blur", this.onInputBlur.bind(this));

		//Create a button to increment the value by 1.
		this.button = document.createElement("button");

		// Get the localized string from localized string
		this.button.innerHTML = context.resources.getString("PCF_LocalizationSample_ButtonLabel");

		this.button.classList.add("LocalizationSample_Button_Style");
		this._notifyOutputChanged = notifyOutputChanged;
		//this.button.addEventListener("click", (event) => { this._value = this._value + 1; this._notifyOutputChanged();});
		this.button.addEventListener("click", this.onButtonClick.bind(this));

		// Adding the label and button created to the container DIV.
		this._container = document.createElement("div");
		this._container.appendChild(this.label);
		this._container.appendChild(this.button);
		container.appendChild(this._container);
	}

	/**
	 * Button Event handler for the button created as part of this control
	 * @param event
	 */
	private onButtonClick(event: Event): void {
		this._value = this._value + 1;
		this._notifyOutputChanged();
	}

	/**
	 * Input Blur Event handler for the input created as part of this control
	 * @param event
	 */
	private onInputBlur(event: Event): void {
		const inputNumber = Number(this.label.value);
		this._value = isNaN(inputNumber) ? (this.label.value as unknown as number) : inputNumber;
		this._notifyOutputChanged();
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		// This method would rerender the control with the updated values after we call NotifyOutputChanged
		//set the value of the field control to the raw value from the configured field
		this._value = context.parameters.value.raw!;
		this.label.value = this._value != null ? this._value.toString() : "";

		if (context.parameters.value.error) {
			this.label.classList.add("LocalizationSample_Input_Error_Style");
		} else {
			this.label.classList.remove("LocalizationSample_Input_Error_Style");
		}
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		// custom code goes here - remove the line below and return the correct output
		const result: IOutputs = {
			value: this._value,
		};
		return result;
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
