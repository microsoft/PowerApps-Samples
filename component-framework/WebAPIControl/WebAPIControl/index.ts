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

export class WebAPIControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Reference to ComponentFramework Context object
	private _context: ComponentFramework.Context<IInputs>;

	// Name of entity to use for example Web API calls performed by this control
	private static _entityName = "account";

	// Required field on _entityName of type 'single line of text'
	// Example Web API calls performed by example custom control will set this field for new record creation examples
	private static _requiredAttributeName = "name";

	// Value the _requiredAttributeName field will be set to for new created records
	private static _requiredAttributeValue = "Web API Custom Control (Sample)";

	// Name of currency field on _entityName to populate during record create
	// Example Web API calls performed by example custom control will set and read this field
	private static _currencyAttributeName = "revenue";

	// Friendly name of currency field (only used for control UI - no functional impact)
	private static _currencyAttributeNameFriendlyName = "annual revenue";

	// Flag if control view has been rendered
	private _controlViewRendered: boolean;

	// References to button elements rendered by example custom control
	private _createEntity1Button: HTMLButtonElement;
	private _createEntity2Button: HTMLButtonElement;
	private _createEntity3Button: HTMLButtonElement;
	private _deleteRecordButton: HTMLButtonElement;
	private _fetchXmlRefreshButton: HTMLButtonElement;
	private _oDataRefreshButton: HTMLButtonElement;

	// References to div elements rendered by the example custom control
	private _odataStatusContainerDiv: HTMLDivElement;
	private _resultContainerDiv: HTMLDivElement;

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
		this._context = context;
		this._controlViewRendered = false;
		this._container = document.createElement("div");
		this._container.classList.add("WebAPIControl_Container");
		container.appendChild(this._container);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this._context = context;
		if (!this._controlViewRendered) {
			this._controlViewRendered = true;

			// Render Web API Examples
			this.renderCreateExample();
			this.renderDeleteExample();
			this.renderFetchXmlRetrieveMultipleExample();
			this.renderODataRetrieveMultipleExample();

			// Render result div to display output of Web API calls
			this.renderResultsDiv();
		}
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

	/**
	 * Renders example use of CreateRecord Web API
	 */
	private renderCreateExample() {
		// Create header label for Web API sample
		const headerDiv: HTMLDivElement = this.createHTMLDivElement(
			"create_container",
			true,
			`Click to create ${WebAPIControl._entityName} record`
		);
		this._container.appendChild(headerDiv);

		// Create button 1 to create record with revenue field set to 100
		const value1 = "100";
		this._createEntity1Button = this.createHTMLButtonElement(
			this.getCreateRecordButtonLabel(value1),
			this.getCreateButtonId(value1),
			value1,
			this.createButtonOnClickHandler.bind(this)
		);

		// Create button 2 to create record with revenue field set to 200
		const value2 = "200";
		this._createEntity2Button = this.createHTMLButtonElement(
			this.getCreateRecordButtonLabel(value2),
			this.getCreateButtonId(value2),
			value2,
			this.createButtonOnClickHandler.bind(this)
		);

		// Create button 3 to create record with revenue field set to 300
		const value3 = "300";
		this._createEntity3Button = this.createHTMLButtonElement(
			this.getCreateRecordButtonLabel(value3),
			this.getCreateButtonId(value3),
			value3,
			this.createButtonOnClickHandler.bind(this)
		);

		// Append all button HTML elements to custom control container div
		this._container.appendChild(this._createEntity1Button);
		this._container.appendChild(this._createEntity2Button);
		this._container.appendChild(this._createEntity3Button);
	}

	/**
	 * Renders example use of DeleteRecord Web API
	 */
	private renderDeleteExample(): void {
		// Create header label for Web API sample
		const headerDiv: HTMLDivElement = this.createHTMLDivElement(
			"delete_container",
			true,
			`Click to delete ${WebAPIControl._entityName} record`
		);

		// Render button to invoke DeleteRecord Web API call
		this._deleteRecordButton = this.createHTMLButtonElement(
			"Select record to delete",
			"delete_button",
			null,
			this.deleteButtonOnClickHandler.bind(this)
		);

		// Append elements to custom control container div
		this._container.appendChild(headerDiv);
		this._container.appendChild(this._deleteRecordButton);
	}

	/**
	 * Renders example use of RetrieveMultiple Web API with OData
	 */
	private renderODataRetrieveMultipleExample(): void {
		const containerClassName = "odata_status_container";

		// Create header label for Web API sample
		const statusDivHeader: HTMLDivElement = this.createHTMLDivElement(
			containerClassName,
			true,
			"Click to refresh record count"
		);
		this._odataStatusContainerDiv = this.createHTMLDivElement(containerClassName, false, undefined);

		// Create button to invoke OData RetrieveMultiple Example
		this._fetchXmlRefreshButton = this.createHTMLButtonElement(
			"Refresh record count",
			"odata_refresh",
			null,
			this.refreshRecordCountButtonOnClickHandler.bind(this)
		);

		// Append HTML elements to custom control container div
		this._container.appendChild(statusDivHeader);
		this._container.appendChild(this._odataStatusContainerDiv);
		this._container.appendChild(this._fetchXmlRefreshButton);
	}

	/**
	 * Renders example use of RetrieveMultiple Web API with Fetch XML
	 */
	private renderFetchXmlRetrieveMultipleExample(): void {
		const containerName = "fetchxml_status_container";

		// Create header label for Web API sample
		const statusDivHeader: HTMLDivElement = this.createHTMLDivElement(
			containerName,
			true,
			`Click to calculate average value of ${WebAPIControl._currencyAttributeNameFriendlyName}`
		);
		const statusDiv: HTMLDivElement = this.createHTMLDivElement(containerName, false, undefined);

		// Create button to invoke Fetch XML RetrieveMultiple Web API example
		this._oDataRefreshButton = this.createHTMLButtonElement(
			`Calculate average value of ${WebAPIControl._currencyAttributeNameFriendlyName}`,
			"odata_refresh",
			null,
			this.calculateAverageButtonOnClickHandler.bind(this)
		);

		// Append HTML Elements to custom control container div
		this._container.appendChild(statusDivHeader);
		this._container.appendChild(statusDiv);
		this._container.appendChild(this._oDataRefreshButton);
	}

	/**
	 * Renders a 'result container' div element to inject the status of the example Web API calls
	 */
	private renderResultsDiv() {
		// Render header label for result container
		const resultDivHeader: HTMLDivElement = this.createHTMLDivElement(
			"result_container",
			true,
			"Result of last action"
		);
		this._container.appendChild(resultDivHeader);

		// Div elements to populate with the result text
		this._resultContainerDiv = this.createHTMLDivElement("result_container", false, undefined);
		this._container.appendChild(this._resultContainerDiv);

		// Init the result container with a notification the control was loaded
		this.updateResultContainerText("Web API sample custom control loaded");
	}

	/**
	 * Event Handler for onClick of create record button
	 * @param event : click event
	 */
	private createButtonOnClickHandler(event: Event): Promise<void> {
		// Retrieve the value to set the currency field to from the button's attribute
		const currencyAttributeValue: number = parseInt(
			(event.target as Element)?.attributes?.getNamedItem("buttonvalue")?.value ?? "0"
		);

		// Generate unique record name by appending timestamp to _requiredAttributeValue
		const recordName = `${WebAPIControl._requiredAttributeValue}_${Date.now()}`;

		// Set the values for the attributes we want to set on the new record
		// If you want to set additional attributes on the new record, add to data dictionary as key/value pair
		const data: ComponentFramework.WebApi.Entity = {};
		data[WebAPIControl._requiredAttributeName] = recordName;
		data[WebAPIControl._currencyAttributeName] = currencyAttributeValue;

		// Invoke the Web API to creat the new record
		return this._context.webAPI.createRecord(WebAPIControl._entityName, data).then(
			(response: ComponentFramework.LookupValue) => {
				// Callback method for successful creation of new record

				// Get the ID of the new record created
				const id: string = response.id;

				// Generate HTML to inject into the result div to showcase the fields and values of the new record created
				let resultHtml = `Created new ${WebAPIControl._entityName} record with below values:`;
				resultHtml += "<br />";
				resultHtml += "<br />";
				resultHtml += `id: ${id}`;
				resultHtml += "<br />";
				resultHtml += "<br />";
				resultHtml += `${WebAPIControl._requiredAttributeName}: ${recordName}`;
				resultHtml += "<br />";
				resultHtml += "<br />";
				resultHtml += `${WebAPIControl._currencyAttributeName}: ${currencyAttributeValue}`;

				this.updateResultContainerText(resultHtml);
				return;
			},
			(errorResponse) => {
				// Error handling code here - record failed to be created
				this.updateResultContainerTextWithErrorResponse(errorResponse);
			}
		);
	}

	/**
	 * Event Handler for onClick of delete record button
	 * @param event : click event
	 */
	private deleteButtonOnClickHandler(): Promise<void> {
		// Invoke a lookup dialog to allow the user to select an existing record of type _entityName to delete
		const lookUpOptions = {
			entityTypes: [WebAPIControl._entityName],
		};

		const lookUpPromise = this._context.utils.lookupObjects(lookUpOptions);

		return lookUpPromise.then(
			// Callback method - invoked after user has selected an item from the lookup dialog
			// Data parameter is the item selected in the lookup dialog
			(data: ComponentFramework.LookupValue[]) => {
				if (data?.[0]) {
					// Get the ID and entityType of the record selected by the lookup
					const id: string = data[0].id;
					const entityType: string = data[0].entityType;

					// Invoke the deleteRecord method of the WebAPI to delete the selected record
					return this._context.webAPI.deleteRecord(entityType, id).then(
						(response: ComponentFramework.LookupValue) => {
							// Record was deleted successfully
							const responseId: string = response.id;
							const responseEntityType: string = response.entityType;

							// Generate HTML to inject into the result div to showcase the deleted record
							this.updateResultContainerText(`Deleted ${responseEntityType} record with ID: ${responseId}`);
							return;
						},
						(errorResponse) => {
							// Error handling code here
							this.updateResultContainerTextWithErrorResponse(errorResponse);
						}
					);
				}
				return;
			},
			(error) => {
				// Error handling code here
				this.updateResultContainerTextWithErrorResponse(error);
			}
		);
	}

	/**
	 * Event Handler for onClick of calculate average value button
	 * @param event : click event
	 */
	private calculateAverageButtonOnClickHandler(): Promise<void> {
		// Build FetchXML to retrieve the average value of _currencyAttributeName field for all _entityName records
		// Add a filter to only aggregate on records that have _currencyAttributeName not set to null
		let fetchXML = "<fetch distinct='false' mapping='logical' aggregate='true'>";
		fetchXML += `<entity name='${WebAPIControl._entityName}'>`;
		fetchXML += `<attribute name='${WebAPIControl._currencyAttributeName}' aggregate='avg' alias='average_val' />`;
		fetchXML += "<filter>";
		fetchXML += `<condition attribute='${WebAPIControl._currencyAttributeName}' operator='not-null' />`;
		fetchXML += "</filter>";
		fetchXML += "</entity>";
		fetchXML += "</fetch>";

		// Invoke the Web API RetrieveMultipleRecords method to calculate the aggregate value
		return this._context.webAPI.retrieveMultipleRecords(WebAPIControl._entityName, `?fetchXml=${fetchXML}`).then(
			(response: ComponentFramework.WebApi.RetrieveMultipleResponse) => {
				// Retrieve multiple completed successfully -- retrieve the averageValue
				const averageVal: number = response.entities[0].average_val as number;

				// Generate HTML to inject into the result div to showcase the result of the RetrieveMultiple Web API call
				const resultHTML = `Average value of ${WebAPIControl._currencyAttributeNameFriendlyName} attribute for all ${WebAPIControl._entityName} records: ${averageVal}`;
				this.updateResultContainerText(resultHTML);
				return;
			},
			(errorResponse) => {
				// Error handling code here
				this.updateResultContainerTextWithErrorResponse(errorResponse);
			}
		);
	}

	/**
	 * Event Handler for onClick of calculate record count button
	 * @param event : click event
	 */
	private refreshRecordCountButtonOnClickHandler(): Promise<void> {
		// Generate OData query string to retrieve the _currencyAttributeName field for all _entityName records
		// Add a filter to only retrieve records with _requiredAttributeName field which contains _requiredAttributeValue
		const queryString = `?$select=${WebAPIControl._currencyAttributeName}&$filter=contains(${WebAPIControl._requiredAttributeName},'${WebAPIControl._requiredAttributeValue}')`;

		// Invoke the Web API Retrieve Multiple call
		return this._context.webAPI.retrieveMultipleRecords(WebAPIControl._entityName, queryString).then(
			(response: ComponentFramework.WebApi.RetrieveMultipleResponse) => {
				// Retrieve Multiple Web API call completed successfully
				let count1 = 0;
				let count2 = 0;
				let count3 = 0;

				// Loop through each returned record
				for (const entity of response.entities) {
					// Retrieve the value of _currencyAttributeName field
					const value: number = entity[WebAPIControl._currencyAttributeName] as number;

					// Check the value of _currencyAttributeName field and increment the correct counter
					if (value == 100) {
						count1++;
					} else if (value == 200) {
						count2++;
					} else if (value == 300) {
						count3++;
					}
				}

				// Generate HTML to inject into the fetch xml status div to showcase the results of the OData retrieve example
				let innerHtml = "Use above buttons to create or delete a record to see count update";
				innerHtml += "<br />";
				innerHtml += "<br />";
				innerHtml += `Count of ${WebAPIControl._entityName} records with ${WebAPIControl._currencyAttributeName} of 100: ${count1}`;
				innerHtml += "<br />";
				innerHtml += `Count of ${WebAPIControl._entityName} records with ${WebAPIControl._currencyAttributeName} of 200: ${count2}`;
				innerHtml += "<br />";
				innerHtml += `Count of ${WebAPIControl._entityName} records with ${WebAPIControl._currencyAttributeName} of 300: ${count3}`;

				// Inject the HTML into the fetch xml status div
				if (this._odataStatusContainerDiv) {
					this._odataStatusContainerDiv.innerHTML = innerHtml;
				}

				// Inject a success message into the result div
				this.updateResultContainerText("Record count refreshed");
				return;
			},
			(errorResponse) => {
				// Error handling code here
				this.updateResultContainerTextWithErrorResponse(errorResponse);
			}
		);
	}

	/**
	 * Helper method to inject HTML into result container div
	 * @param statusHTML : HTML to inject into result container
	 */
	private updateResultContainerText(statusHTML: string): void {
		if (this._resultContainerDiv) {
			this._resultContainerDiv.innerHTML = statusHTML;
		}
	}

	/**
	 * Helper method to inject error string into result container div after failed Web API call
	 * @param errorResponse : error object from rejected promise
	 */
	private updateResultContainerTextWithErrorResponse(errorResponse: unknown): void {
		if (this._resultContainerDiv) {
			// Retrieve the error message from the errorResponse and inject into the result div
			let errorHTML = "Error with Web API call:";
			errorHTML += "<br />";
			errorHTML += (errorResponse as { message: string }).message;
			this._resultContainerDiv.innerHTML = errorHTML;
		}
	}

	/**
	 * Helper method to generate Label for Create Buttons
	 * @param entityNumber : value to set _currencyAttributeNameFriendlyName field to for this button
	 */
	private getCreateRecordButtonLabel(entityNumber: string): string {
		return `Create record with ${WebAPIControl._currencyAttributeNameFriendlyName} of ${entityNumber}`;
	}

	/**
	 * Helper method to generate ID for Create Button
	 * @param entityNumber : value to set _currencyAttributeNameFriendlyName field to for this button
	 */
	private getCreateButtonId(entityNumber: string): string {
		return `create_button_${entityNumber}`;
	}

	/**
	 * Helper method to create HTML Button used for CreateRecord Web API Example
	 * @param buttonLabel : Label for button
	 * @param buttonId : ID for button
	 * @param buttonValue : value of button (attribute of button)
	 * @param onClickHandler : onClick event handler to invoke for the button
	 */
	private createHTMLButtonElement(
		buttonLabel: string,
		buttonId: string,
		buttonValue: string | null,
		onClickHandler: (event: Event) => void
	): HTMLButtonElement {
		const button: HTMLButtonElement = document.createElement("button");
		button.innerHTML = buttonLabel;

		if (buttonValue) {
			button.setAttribute("buttonvalue", buttonValue);
		}

		button.id = buttonId;

		button.classList.add("SampleControl_WebAPIControl_ButtonClass");
		button.addEventListener("click", onClickHandler);
		return button;
	}

	/**
	 * Helper method to create HTML Div Element
	 * @param elementClassName : Class name of div element
	 * @param isHeader : True if 'header' div - adds extra class and post-fix to ID for header elements
	 * @param innerText : innerText of Div Element
	 */
	private createHTMLDivElement(elementClassName: string, isHeader: boolean, innerText?: string): HTMLDivElement {
		const div: HTMLDivElement = document.createElement("div");

		if (isHeader) {
			div.classList.add("SampleControl_WebAPIControl_Header");
			elementClassName += "_header";
		}

		if (innerText) {
			div.innerText = innerText.toUpperCase();
		}

		div.classList.add(elementClassName);
		return div;
	}
}
