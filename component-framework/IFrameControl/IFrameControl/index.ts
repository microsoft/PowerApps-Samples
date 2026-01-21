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

export class IFrameControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Reference to Bing Map IFrame HTMLElement
	private _bingMapIFrame: HTMLElement;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Flag if control view has been rendered
	private _controlViewRendered: boolean;

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
		this._controlViewRendered = false;
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		if (!this._controlViewRendered) {
			this._controlViewRendered = true;
			this.renderBingMapIFrame();
		}

		const latitude = context.parameters.latitudeValue.raw;
		const longitude = context.parameters.longitudeValue.raw;
		if (latitude && longitude) {
			this.updateBingMapURL(latitude, longitude);
		}
	}

	/**
	 * Render IFrame HTML Element that hosts the Bing Map and appends the IFrame to the control container
	 */
	private renderBingMapIFrame(): void {
		this._bingMapIFrame = this.createIFrameElement();
		this._container.appendChild(this._bingMapIFrame);
	}

	/**
	 * Updates the URL of the Bing Map IFrame to display the updated lat/long coordinates
	 * @param latitude : latitude of center point of Bing map
	 * @param longitude : longitude of center point of Bing map
	 */
	private updateBingMapURL(latitude: number, longitude: number): void {
		// Bing Map API:
		// https://learn.microsoft.com/bingmaps/articles/create-a-custom-map-url

		// Provide bing map query string parameters to format and style map view
		const bingMapUrlPrefix = "https://www.bing.com/maps/embed?h=400&w=300&cp=";
		const bingMapUrlPostfix = "&lvl=12&typ=d&sty=o&src=SHELL&FORM=MBEDV8";

		// Build the entire URL with the user provided latitude and longitude
		const iFrameSrc = `${bingMapUrlPrefix + latitude}~${longitude}${bingMapUrlPostfix}`;

		// Update the IFrame to point to the updated URL
		this._bingMapIFrame.setAttribute("src", iFrameSrc);
	}

	/**
	 * Helper method to create an IFrame HTML Element
	 */
	private createIFrameElement(): HTMLElement {
		const iFrameElement: HTMLElement = document.createElement("iframe");
		iFrameElement.setAttribute("class", "SampleControl_IFrame");
		return iFrameElement;
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
