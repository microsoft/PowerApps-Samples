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

export class MapControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// HTML IFrame element that will be used to render the map
	private _iFrameElement: HTMLIFrameElement;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens.
	private _notifyOutputChanged: () => void;

	// Reference to ComponentFramework Context object
	private _context: ComponentFramework.Context<IInputs>;

	// API Key used to activate and embed the maps automatically
	// NOTE: You can follow the documentation at https://developers.google.com/maps/documentation/embed/get-api-key to generate your own API Key
	private MAPS_API_KEY = "<Replace your Key here>";

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
		this._iFrameElement = document.createElement("iframe");
		this._iFrameElement.setAttribute("class", "mapHiddenStyle");
		this.renderMap(this.buildMapUrl(context.parameters.controlValue.formatted));
		container.appendChild(this._iFrameElement);
	}

	/**
	 * Checks if the url is not null and sets the value to the iFrame source to be loaded inside it and then notifies the ContorlFramework that the output has changed
	 * @param mapUrl : The url for the map that needs to be loaded inside the iFrame.
	 */
	public renderMap(mapUrl: string): void {
		if (mapUrl) {
			this._iFrameElement.setAttribute("src", mapUrl);
			this._iFrameElement.setAttribute("class", "mapVisibleStyle");
		} else {
			this._iFrameElement.setAttribute("class", "mapHiddenStyle");
		}
		this._notifyOutputChanged();
	}

	/**
	 * Converts the string that is passed to a valid url that can be used to render the map for the location
	 * @param addressString : any string that can be used to search for a location in maps
	 * @returns the HTML encoded url that can be used to load the map if the addressString is non empty string
	 */
	public buildMapUrl(addressString: string | undefined): string {
		return addressString
			? `https://www.google.com/maps/embed/v1/place?key=${
					this._context.parameters.controlApiKey.raw ?? this.MAPS_API_KEY
				}&q=${encodeURIComponent(addressString)}`
			: "";
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this._context = context;
		this.renderMap(this.buildMapUrl(context.parameters.controlValue.formatted));
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
