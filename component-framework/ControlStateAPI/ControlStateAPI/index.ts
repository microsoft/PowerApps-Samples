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

// Key used to store the selected color into the context object to persist across navigations
const PERSISTED_SELECTED_COLOR_KEY_NAME = "selectedColor";

// Key used to store the selected color into the context object to persist across navigations
const PERSISTED_SELECTED_LABEL_KEY_NAME = "selectedLabel";

export class ControlStateAPI implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Flag if control view has been rendered
	private _controlViewRendered: boolean;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Div element to show the current selected color
	private _selectedColorElement: HTMLDivElement;

	// Control context object
	private _context: ComponentFramework.Context<IInputs>;

	// The color selected from the previous navigation
	private _persistedSelectedColor: string;

	// The label selected from the previous navigation
	private _persistedSelectedLabel: string;

	// Data type used to store the various information as part of the state object.
	private _stateDictionary: ComponentFramework.Dictionary = {};

	// References to HTML Button Elements rendered on the control
	private _buttonRed: HTMLButtonElement;
	private _buttonBlue: HTMLButtonElement;
	private _buttonGreen: HTMLButtonElement;

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
		this._controlViewRendered = false;
		this._container = document.createElement("div");
		this._context = context;

		this._container.classList.add("ControlState_Container");
		container.appendChild(this._container);

		// Check if state was persisted from the last time the control was loaded.
		if (state) {
			// If you are storing a collection of key value pairs as part of the state object, maintain a local copy of it so that it can be used to
			// add, update or remove keys during the control life cycle.
			this._stateDictionary = state;

			// Retrieve persisted state and set values into variables so state can be used during control rendering.
			this._persistedSelectedColor = state[PERSISTED_SELECTED_COLOR_KEY_NAME] as string;
			this._persistedSelectedLabel = state[PERSISTED_SELECTED_LABEL_KEY_NAME] as string;
		}

		// State not perisited in control -- set variable to default values
		if (!this._persistedSelectedColor) {
			this._persistedSelectedColor = "transparent";
		}

		// State not perisited in control -- set variable to default values
		if (!this._persistedSelectedLabel) {
			this._persistedSelectedLabel = "none";
		}
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		if (!this._controlViewRendered) {
			// Render header label
			const chooseColorLabel: HTMLElement = this.renderLabelDivElement("Select a Color");
			this._container.appendChild(chooseColorLabel);

			// Render 3 color buttons
			this._buttonGreen = this.renderButtonElement("Green", "#80ff80");
			this._container.appendChild(this._buttonGreen);

			this._buttonBlue = this.renderButtonElement("Blue", "#add8e6");
			this._container.appendChild(this._buttonBlue);

			this._buttonRed = this.renderButtonElement("Red", "#ff8080");
			this._container.appendChild(this._buttonRed);

			// Render label for 'selected color' area
			const selectedColorLabel: HTMLElement = this.renderLabelDivElement("Selected Color");
			this._container.appendChild(selectedColorLabel);

			// Render 'selected color' display area
			this.renderSelectedColorElement();
			this.updateSelectedColorElement(
				this._selectedColorElement,
				this._persistedSelectedLabel,
				this._persistedSelectedColor
			);

			// Mark the control view as rendered so we do not re-render next time this method is invoked
			this._controlViewRendered = true;
		}
	}

	/**
	 * Creates an HTML Div Element with the provided label
	 *
	 * @param labelText : label for the div
	 */
	private renderLabelDivElement(labelText: string): HTMLDivElement {
		const div: HTMLDivElement = document.createElement("div");
		div.innerText = labelText;
		div.classList.add("ControlState_DivLabelClass");
		return div;
	}

	/**
	 * Renders a button element that the user can click to select a color
	 *
	 * @param label : label for the button (color name)
	 * @param color : Hex code for the color
	 */
	private renderButtonElement(label: string, color: string): HTMLButtonElement {
		const button: HTMLButtonElement = document.createElement("button");
		button.innerHTML = label;
		button.setAttribute("value", label);
		button.setAttribute("buttonColor", color);
		button.classList.add("ControlState_ButtonClass");
		button.addEventListener("click", (event) => this.onButtonClick(event, this._selectedColorElement));

		return button;
	}

	/**
	 * This method updates the selected color element to reflect the last color the user selected.
	 * The label and background color will be updated to reflect the selected color.
	 *
	 * @param selectedColorElement element to make the changes to
	 * @param label new label for the selectedColorElement
	 * @param backgroundColor new background color for the selectedColorElement
	 */
	private updateSelectedColorElement(selectedColorElement: HTMLDivElement, label: string, backgroundColor: string) {
		selectedColorElement.innerText = label;
		this._selectedColorElement.style.backgroundColor = backgroundColor;
	}

	/**
	 * Renders a div Element that will contain information regarding the last color the user selected
	 */
	private renderSelectedColorElement() {
		this._selectedColorElement = document.createElement("div");
		this._selectedColorElement.classList.add("ControlState_SelectedColorElement");
		this._container.appendChild(this._selectedColorElement);
	}

	/**
	 * Onclick event handler for click of a color button
	 * This method checks the button that was clicked (red / blue / green) and updates the selected color element
	 * with the selected color as the element label and background
	 *
	 * @param event Click Event
	 * @param selectedColorElement The HTML Div Element that the results should be injected into
	 */
	private onButtonClick(event: Event, selectedColorElement: HTMLDivElement) {
		const eventTarget: Element = event.target as Element;

		if (eventTarget) {
			// Get the label and the selected color attributes from the div element that was clicked
			const label: string = eventTarget.attributes.getNamedItem("value")?.value ?? "";
			const selectedColor: string = eventTarget?.attributes.getNamedItem("buttonColor")?.value ?? "";

			// Update the selected color div element with the results
			this.updateSelectedColorElement(selectedColorElement, label, selectedColor);

			// store the label and selected color into the local state dictionary that will be passed onto the framework.
			this._stateDictionary[PERSISTED_SELECTED_LABEL_KEY_NAME] = label;
			this._stateDictionary[PERSISTED_SELECTED_COLOR_KEY_NAME] = selectedColor;

			// Store the state dictionary object into the setControlState interface method to allow the
			// selected data to persist within the user session
			// In scenarios where you don't need a collection of key value pairs but just one key value pair to be stored, just pass that object to the setControlState method.
			this._context.mode.setControlState(this._stateDictionary);
		}
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
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
