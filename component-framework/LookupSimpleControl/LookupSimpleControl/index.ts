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

export class LookupSimpleControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Reference to ComponentFramework Context object
	private _context: ComponentFramework.Context<IInputs>;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens
	private _notifyOutputChanged: () => void;

	// Simple label used to render a title for this control
	private _contentLabel = "Simple Lookup Control";

	// Containers to store and display raw lookup data for both properties
	private _inputData1: HTMLLabelElement;
	private _inputData2: HTMLLabelElement;

	// Values to be filled based on control context, passed as arguments to lookupObjects API
	private _entityType1 = "";
	private _defaultViewId1 = "";
	private _entityType2 = "";
	private _defaultViewId2 = "";

	// Used to store necessary data for a single lookup entity selected during runtime
	private _selectedItem1: ComponentFramework.LookupValue;
	private _selectedItem2: ComponentFramework.LookupValue;

	// Used to track which lookup property is being updated
	private _updateSelected1 = false;

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

		// Cache information necessary for lookupObjects API based on control context
		this._entityType1 = context.parameters.controlValue.getTargetEntityType?.();
		this._defaultViewId1 = context.parameters.controlValue.getViewId?.();
		this._entityType2 = context.parameters.controlValue1.getTargetEntityType?.();
		this._defaultViewId2 = context.parameters.controlValue1.getViewId?.();

		const contentContainer = document.createElement("div");

		// Add title label to control
		const contentLabel = document.createElement("label");
		contentLabel.textContent = this._contentLabel;
		contentContainer.appendChild(contentLabel);

		// Formatting for ease of view
		contentContainer.append(document.createElement("br"));
		contentContainer.append(document.createElement("br"));

		// Add element to display raw entity data
		this._inputData1 = document.createElement("label");
		this._inputData1.textContent = "";
		contentContainer.appendChild(this._inputData1);

		// Formatting for ease of view
		contentContainer.append(document.createElement("br"));
		contentContainer.append(document.createElement("br"));

		// Add button to trigger lookupObjects API for the primary lookup property
		const lookupObjectsButton1 = document.createElement("button");
		lookupObjectsButton1.innerText = "Lookup Objects";
		lookupObjectsButton1.onclick = this.performLookupObjects.bind(
			this,
			this._entityType1,
			this._defaultViewId1,
			(value, update = true) => {
				this._selectedItem1 = value;
				this._updateSelected1 = update;
			}
		);
		contentContainer.appendChild(lookupObjectsButton1);

		contentContainer.append(document.createElement("br"));
		contentContainer.append(document.createElement("br"));

		// Add element to display raw entity data
		this._inputData2 = document.createElement("label");
		this._inputData2.textContent = "";
		contentContainer.appendChild(this._inputData2);

		// Formatting for ease of view
		contentContainer.append(document.createElement("br"));
		contentContainer.append(document.createElement("br"));

		// Add button to trigger lookupObjects API for the secondary lookup property
		const lookupObjectsButton2 = document.createElement("button");
		lookupObjectsButton2.innerText = "Lookup Objects";
		lookupObjectsButton2.onclick = this.performLookupObjects.bind(
			this,
			this._entityType2,
			this._defaultViewId2,
			(value) => {
				this._selectedItem2 = value;
				this._updateSelected1 = false;
			}
		);
		contentContainer.appendChild(lookupObjectsButton2);

		contentContainer.append(document.createElement("br"));
		contentContainer.append(document.createElement("br"));

		this._container.appendChild(contentContainer);
	}

	/**
	 * Helper to utilize the lookupObjects API. Gets whatever lookup record is selected and allows
	 * the control to use the received data.
	 * @param entityType The entity logical name bound to the target lookup property
	 * @param viewId The viewId bound to the target lookup property
	 * @param setSelected Specified function to set the selected lookup value
	 */
	private performLookupObjects(
		entityType: string,
		viewId: string,
		setSelected: (value: ComponentFramework.LookupValue, update?: boolean) => void
	): Promise<void> {
		// Used cached values from lookup parameter to set options for lookupObjects API
		const lookupOptions = {
			defaultEntityType: entityType,
			defaultViewId: viewId,
			allowMultiSelect: false,
			entityTypes: [entityType],
			viewIds: [viewId],
		};

		return this._context.utils.lookupObjects(lookupOptions).then(
			(success) => {
				if (success && success.length > 0) {
					// Cache the necessary information for the newly selected entity lookup
					const selectedReference = success[0];
					const selectedLookupValue: ComponentFramework.LookupValue = {
						id: selectedReference.id,
						name: selectedReference.name,
						entityType: selectedReference.entityType,
					};

					// Update the primary or secondary lookup property
					setSelected(selectedLookupValue);

					// Trigger a control update
					this._notifyOutputChanged();
				} else {
					setSelected({} as ComponentFramework.LookupValue);
				}
				return;
			},
			(error) => {
				console.log(error);
			}
		);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		// Update the main text field of the control to contain the raw data of the entity selected via lookup

		const lookupValue1: ComponentFramework.LookupValue = context.parameters.controlValue.raw[0];
		const lookupValue2: ComponentFramework.LookupValue = context.parameters.controlValue1.raw[0];
		this._context = context;
		let propertyValue = `name: ${lookupValue1.name} entityType: ${lookupValue1.entityType} id: ${lookupValue1.id}`;
		this._inputData1.textContent = propertyValue;
		propertyValue = `name: ${lookupValue2.name} entityType: ${lookupValue2.entityType} id: ${lookupValue2.id}`;
		this._inputData2.textContent = propertyValue;
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		// Send the updated selected lookup item back to the ComponentFramework, based on the currently selected item
		return this._updateSelected1 ? { controlValue: [this._selectedItem1] } : { controlValue1: [this._selectedItem2] };
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
