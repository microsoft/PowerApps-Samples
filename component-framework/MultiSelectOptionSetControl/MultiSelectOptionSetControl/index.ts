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

export class MultiSelectOptionSetControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens
	private _notifyOutputChanged: () => void;

	// Simple label used to display the currently selected options
	private _rawLabel: HTMLLabelElement;

	// Simple label used as a static title for the control
	private _optionsLabel: HTMLLabelElement;

	// Dropdown element used to contain all select options for the MultiSelectOptionSet
	private _dropdown: HTMLSelectElement;

	// Internal structure used to store the values of the currently selected options
	private _selectedOptions: number[];

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
		this._container = container;
		this._notifyOutputChanged = notifyOutputChanged;

		// Store the values currently selected
		this._selectedOptions = context.parameters.controlValue.raw ?? [];

		const contentContainer = document.createElement("div");

		this._optionsLabel = document.createElement("label");

		// Create the dropdown element that allows multi-select
		this._dropdown = document.createElement("select");
		this._dropdown.setAttribute("multiple", "true");

		// Create a select option for each option specified by the target OptionSet and add it to the dropdown
		context.parameters.controlValue.attributes?.Options?.forEach((option) => {
			const dropdownOption = document.createElement("option");
			dropdownOption.setAttribute("value", String(option.Value));
			dropdownOption.innerText = option.Label;
			dropdownOption.onclick = this.updateIndividualSelected.bind(this, dropdownOption);
			dropdownOption.selected = this._selectedOptions.includes(option.Value);
			this._dropdown.appendChild(dropdownOption);
		});

		// Add the dropdown and all its contained options to the main container
		contentContainer.appendChild(this._dropdown);

		// Formatting for ease of view
		contentContainer.appendChild(document.createElement("br"));
		contentContainer.appendChild(document.createElement("br"));

		// Add static label to title the selected options
		this._optionsLabel.textContent = "Selected Options:";
		contentContainer.appendChild(this._optionsLabel);
		contentContainer.append(document.createElement("br"));

		// Add label element that will display the currently selected options
		this._rawLabel = document.createElement("label");
		this._rawLabel.textContent = "";
		contentContainer.appendChild(this._rawLabel);

		// Add the entire container to the control's main container
		this._container.appendChild(contentContainer);
	}

	/**
	 * onClick callback for individual options in the dropdown. Clicking an option will add/remove it
	 * to the currently selected options, then update the control's view.
	 * @param option The target option that has just been selected
	 */
	private updateIndividualSelected(option: HTMLOptionElement): void {
		const value = Number(option.getAttribute("value"));
		const index = this._selectedOptions.indexOf(value);
		if (index === -1) {
			this._selectedOptions.push(value);
			option.selected = true;
		} else {
			this._selectedOptions.splice(index, 1);
			option.selected = false;
		}
		this._notifyOutputChanged();
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		// Update the displayed label to show the currently selected options
		this._rawLabel.textContent = context.parameters.controlValue.formatted ?? "";
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs {
		// Send the currently selected options back to the ComponentFramework
		return { controlValue: this._selectedOptions };
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
