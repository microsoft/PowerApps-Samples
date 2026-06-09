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

	export class PortalWebAPIControl implements ComponentFramework.StandardControl<IInputs, IOutputs>
	{
		private _container: HTMLDivElement;
		private _context: ComponentFramework.Context<IInputs>;
		private static _entityName = "account";
		private static _requiredAttributeName = "name";
		private static _requiredAttributeValue = "Web API Custom Control (Sample)";
		private static _currencyAttributeName = "revenue";
		private static _currencyAttributeNameFriendlyName = "annual revenue";
		private _controlViewRendered: boolean;
		private _createEntity1Button: HTMLButtonElement;
		private _createEntity2Button: HTMLButtonElement;
		private _createEntity3Button: HTMLButtonElement;
		private _deleteRecordButton: HTMLButtonElement;
		private _fetchXmlRefreshButton: HTMLButtonElement;
		private _oDataRefreshButton: HTMLButtonElement;
		private _odataStatusContainerDiv: HTMLDivElement;
		private _resultContainerDiv: HTMLDivElement;
		private _dropDownList:HTMLSelectElement;
	
		public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container: HTMLDivElement): void {
			this._context = context;
			this._controlViewRendered = false;
			this._container = document.createElement("div");
			this._container.classList.add("PortalWebAPIControl_Container");
			container.appendChild(this._container);
		}
	
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
	
		public getOutputs(): IOutputs {
			// no-op: method not leveraged by this example custom control
			return {};
		}
	
		public destroy(): void {
			// no-op: method not leveraged by this example custom control
		}
	
		private renderCreateExample() {
			// Create header label for Web API sample
			const headerDiv: HTMLDivElement = this.createHTMLDivElement("create_container", true, `Click to create ${PortalWebAPIControl._entityName} record`);
			this._container.appendChild(headerDiv);
	
			// Create button 1 to create record with revenue field set to 100
			const value1 = "100";
			this._createEntity1Button = this.createHTMLButtonElement(
				this.getCreateRecordButtonLabel(value1),
				this.getCreateButtonId(value1),
				value1,
				this.createButtonOnClickHandler.bind(this));
	
			// Create button 2 to create record with revenue field set to 200
			const value2 = "200";
			this._createEntity2Button = this.createHTMLButtonElement(
				this.getCreateRecordButtonLabel(value2),
				this.getCreateButtonId(value2),
				value2,
				this.createButtonOnClickHandler.bind(this));
	
			// Create button 3 to create record with revenue field set to 300
			const value3 = "300";
			this._createEntity3Button = this.createHTMLButtonElement(
				this.getCreateRecordButtonLabel(value3),
				this.getCreateButtonId(value3),
				value3,
				this.createButtonOnClickHandler.bind(this));
	
			// Append all button HTML elements to custom control container div
			this._container.appendChild(this._createEntity1Button);
			this._container.appendChild(this._createEntity2Button);
			this._container.appendChild(this._createEntity3Button);
		}
	
		private renderDeleteExample(): void {
			// Create header label for Web API sample
			const headerDiv: HTMLDivElement = this.createHTMLDivElement("delete_container", true, `Click to delete ${PortalWebAPIControl._entityName} record`);

			this._deleteRecordButton = document.createElement("button");
			this._deleteRecordButton.innerHTML = "Delete Record";
			this._deleteRecordButton.id = "delete_button";	
			this._deleteRecordButton.classList.add("SampleControl_PortalWebAPIControl_DeleteButtonClass");
			this._deleteRecordButton.addEventListener("click", this.deleteButtonOnClickHandler.bind(this));
			this._dropDownList = document.createElement("select");
			this._dropDownList.name = "Delete Entity";
			this._dropDownList.id = "DeleteEntity";
			this._dropDownList.classList.add("SampleControl_PortalWebAPIControl_SelectClass");

			// Append elements to custom control container div
			this._container.appendChild(headerDiv);
			this._container.appendChild(this._dropDownList);
			this._container.appendChild(this._deleteRecordButton);
			this.PopulateItemsToDelete();
		}
	
		private renderODataRetrieveMultipleExample(): void {
			const containerClassName = "odata_status_container";
	
			// Create header label for Web API sample
			const statusDivHeader: HTMLDivElement = this.createHTMLDivElement(containerClassName, true, "Click to refresh record count");
			this._odataStatusContainerDiv = this.createHTMLDivElement(containerClassName, false, undefined);
	
			// Create button to invoke OData RetrieveMultiple Example
			this._fetchXmlRefreshButton = this.createHTMLButtonElement(
				"Refresh record count",
				"odata_refresh",
				null,
				this.refreshRecordCountButtonOnClickHandler.bind(this));
	
			// Append HTML elements to custom control container div
			this._container.appendChild(statusDivHeader);
			this._container.appendChild(this._odataStatusContainerDiv);
			this._container.appendChild(this._fetchXmlRefreshButton);
		}
	
		private renderFetchXmlRetrieveMultipleExample(): void {
			const containerName = "fetchxml_status_container";
	
			// Create header label for Web API sample
			const statusDivHeader: HTMLDivElement = this.createHTMLDivElement(containerName, true,
				`Click to calculate average value of ${PortalWebAPIControl._currencyAttributeNameFriendlyName}`);
			const statusDiv: HTMLDivElement = this.createHTMLDivElement(containerName, false, undefined);

			statusDivHeader.style.marginTop = "40px";
	
			// Create button to invoke Fetch XML RetrieveMultiple Web API example
			this._oDataRefreshButton = this.createHTMLButtonElement(
				`Calculate average value of ${PortalWebAPIControl._currencyAttributeNameFriendlyName}`,
				"odata_refresh",
				null,
				this.calculateAverageButtonOnClickHandler.bind(this));
	
			// Append HTML Elements to custom control container div
			this._container.appendChild(statusDivHeader);
			this._container.appendChild(statusDiv);
			this._container.appendChild(this._oDataRefreshButton);
		}
	
		private renderResultsDiv() {
			// Render header label for result container
			const resultDivHeader: HTMLDivElement = this.createHTMLDivElement("result_container", true,
				"Result of last action");
			this._container.appendChild(resultDivHeader);
	
			// Div elements to populate with the result text
			this._resultContainerDiv = this.createHTMLDivElement("result_container", false, undefined);
			this._container.appendChild(this._resultContainerDiv);
	
			// Init the result container with a notification the control was loaded
			this.updateResultContainerText("Web API sample custom control loaded");
		}	
		
		private createButtonOnClickHandler(event: Event): void {
			// Retrieve the value to set the currency field to from the button's attribute
			const currencyAttributeValue: number = parseInt(
				(event.target as Element)?.attributes?.getNamedItem("buttonvalue")?.value ?? "0"
			);
	
			// Generate unique record name by appending timestamp to _requiredAttributeValue
			const recordName = `${PortalWebAPIControl._requiredAttributeValue}_${Date.now()}`;
	
			// Set the values for the attributes we want to set on the new record
			// If you want to set additional attributes on the new record, add to data dictionary as key/value pair
			const data: ComponentFramework.WebApi.Entity = {};
			data[PortalWebAPIControl._requiredAttributeName] = recordName;
			data[PortalWebAPIControl._currencyAttributeName] = currencyAttributeValue;
	
			// Invoke the Web API to creat the new record
			this._context.webAPI.createRecord(PortalWebAPIControl._entityName, data).then(
				(response: ComponentFramework.LookupValue) => {
					// Callback method for successful creation of new record
	
					// Get the ID of the new record created
					const id: string = response.id;
	
					// Generate HTML to inject into the result div to showcase the fields and values of the new record created
					let resultHtml = `Created new ${  PortalWebAPIControl._entityName  } record with below values:`;
					resultHtml += "<br />";
					resultHtml += "<br />";
					resultHtml += `id: ${id}`;
					resultHtml += "<br />";
					resultHtml += "<br />";
					resultHtml += `${PortalWebAPIControl._requiredAttributeName}: ${recordName}`;
					resultHtml += "<br />";
					resultHtml += "<br />";
					resultHtml += `${PortalWebAPIControl._currencyAttributeName}: ${currencyAttributeValue}`;
	
					this.updateResultContainerText(resultHtml);
					this.PopulateItemsToDelete();
				},
				(errorResponse) => {
					// Error handling code here - record failed to be created
					this.updateResultContainerTextWithErrorResponse(errorResponse);
				}
			);
		}

		private PopulateItemsToDelete(): void{
			var i, L = this._dropDownList.options.length - 1;
			for(i = L; i >= 0; i--) 
			{
				this._dropDownList.options.remove(i);
			}

			const queryString = `?$select=${PortalWebAPIControl._requiredAttributeName}&$filter=contains(${PortalWebAPIControl._requiredAttributeName},'${PortalWebAPIControl._requiredAttributeValue}')`;
			// Invoke the Web API Retrieve Multiple call
			this._context.webAPI.retrieveMultipleRecords(PortalWebAPIControl._entityName, queryString).then(
				(response: ComponentFramework.WebApi.RetrieveMultipleResponse) => {
					var option = document.createElement("option");
					option.value = "";
					option.text = "";
					this._dropDownList.appendChild(option);
					for (const entity of response.entities) {
						var option = document.createElement("option");
						option.value = entity.accountid;
						option.text = entity.name;
						this._dropDownList.appendChild(option);
					}
				});
		}
	
		private deleteButtonOnClickHandler(): void {			
			if(this._dropDownList.value != "")
			{
				var entityId = this._dropDownList.value;
				this._context.webAPI.deleteRecord(PortalWebAPIControl._entityName, this._dropDownList.value).then(
					(response: ComponentFramework.LookupValue) => {
						const responseEntityType: string = response.entityType;

						this.updateResultContainerText(`Deleted ${responseEntityType} record with ID: ${entityId}`);
						this.PopulateItemsToDelete();
					},
					(errorResponse) => {
						// Error handling code here
						this.updateResultContainerTextWithErrorResponse(errorResponse);
					});
			}

		}
	
		private calculateAverageButtonOnClickHandler(): void {
			// Build FetchXML to retrieve the average value of _currencyAttributeName field for all _entityName records
			// Add a filter to only aggregate on records that have _currencyAttributeName not set to null
			let fetchXML = "<fetch distinct='false' mapping='logical' aggregate='true'>";
			fetchXML += `<entity name='${PortalWebAPIControl._entityName}'>`;
			fetchXML += `<attribute name='${PortalWebAPIControl._currencyAttributeName}' aggregate='avg' alias='average_val' />`;
			fetchXML += "<filter>";
			fetchXML += `<condition attribute='${PortalWebAPIControl._currencyAttributeName}' operator='not-null' />`;
			fetchXML += "</filter>";
			fetchXML += "</entity>";
			fetchXML += "</fetch>";
	
			// Invoke the Web API RetrieveMultipleRecords method to calculate the aggregate value
			this._context.webAPI.retrieveMultipleRecords(PortalWebAPIControl._entityName, `?fetchXml=${  fetchXML}`).then(
				(response: ComponentFramework.WebApi.RetrieveMultipleResponse) => {
					// Retrieve multiple completed successfully -- retrieve the averageValue 
					const averageVal: number = response.entities[0].average_val;
	
					// Generate HTML to inject into the result div to showcase the result of the RetrieveMultiple Web API call
					const resultHTML = `Average value of ${PortalWebAPIControl._currencyAttributeNameFriendlyName} attribute for all ${PortalWebAPIControl._entityName} records: ${averageVal}`;
					this.updateResultContainerText(resultHTML);
				},
				(errorResponse) => {
					// Error handling code here
					this.updateResultContainerTextWithErrorResponse(errorResponse);
				}
			);
		}
	
		private refreshRecordCountButtonOnClickHandler(): void {
			// Generate OData query string to retrieve the _currencyAttributeName field for all _entityName records
			// Add a filter to only retrieve records with _requiredAttributeName field which contains _requiredAttributeValue
			const queryString = `?$select=${PortalWebAPIControl._currencyAttributeName  }&$filter=contains(${PortalWebAPIControl._requiredAttributeName},'${PortalWebAPIControl._requiredAttributeValue}')`;
	
			// Invoke the Web API Retrieve Multiple call
			this._context.webAPI.retrieveMultipleRecords(PortalWebAPIControl._entityName, queryString).then(
				(response: ComponentFramework.WebApi.RetrieveMultipleResponse) => {
					// Retrieve Multiple Web API call completed successfully
					let count1 = 0;
					let count2 = 0;
					let count3 = 0;
	
					// Loop through each returned record
					for (const entity of response.entities) {
						// Retrieve the value of _currencyAttributeName field
						const value: number = entity[PortalWebAPIControl._currencyAttributeName];
	
						// Check the value of _currencyAttributeName field and increment the correct counter
						if (value == 100) {
							count1++;
						}
						else if (value == 200) {
							count2++;
						}
						else if (value == 300) {
							count3++;
						}
					}
	
					// Generate HTML to inject into the fetch xml status div to showcase the results of the OData retrieve example
					let innerHtml = "Use above buttons to create or delete a record to see count update";
					innerHtml += "<br />";
					innerHtml += "<br />";
					innerHtml += `Count of ${PortalWebAPIControl._entityName} records with ${PortalWebAPIControl._currencyAttributeName} of 100: ${count1}`;
					innerHtml += "<br />";
					innerHtml += `Count of ${PortalWebAPIControl._entityName} records with ${PortalWebAPIControl._currencyAttributeName} of 200: ${count2}`;
					innerHtml += "<br />";
					innerHtml += `Count of ${PortalWebAPIControl._entityName} records with ${PortalWebAPIControl._currencyAttributeName} of 300: ${count3}`;
	
					// Inject the HTML into the fetch xml status div
					if (this._odataStatusContainerDiv) {
						this._odataStatusContainerDiv.innerHTML = innerHtml;
					}
	
					// Inject a success message into the result div
					this.updateResultContainerText("Record count refreshed");
				},
				(errorResponse) => {
					// Error handling code here
					this.updateResultContainerTextWithErrorResponse(errorResponse);
				}
			);
		}
	
		private updateResultContainerText(statusHTML: string): void {
			if (this._resultContainerDiv) {
				this._resultContainerDiv.innerHTML = statusHTML;
			}
		}
	
		private updateResultContainerTextWithErrorResponse(errorResponse: any): void {
			if (this._resultContainerDiv) {
				// Retrieve the error message from the errorResponse and inject into the result div
				let errorHTML = "Error with Web API call:";
				errorHTML += "<br />";
				errorHTML += errorResponse.message;
				this._resultContainerDiv.innerHTML = errorHTML;
			}
		}
	
		private getCreateRecordButtonLabel(entityNumber: string): string {
			return `Create record with ${PortalWebAPIControl._currencyAttributeNameFriendlyName} of ${entityNumber}`;
		}
	
		private getCreateButtonId(entityNumber: string): string {
			return `create_button_${entityNumber}`;
		}
	
		private createHTMLButtonElement(buttonLabel: string, buttonId: string, buttonValue: string | null, onClickHandler: (event?: any) => void): HTMLButtonElement {
			const button: HTMLButtonElement = document.createElement("button");
			button.innerHTML = buttonLabel;
	
			if (buttonValue) {
				button.setAttribute("buttonvalue", buttonValue);
			}
	
			button.id = buttonId;
	
			button.classList.add("SampleControl_PortalWebAPIControl_ButtonClass");
			button.addEventListener("click", onClickHandler);
			return button;
		}
	
		private createHTMLDivElement(elementClassName: string, isHeader: boolean, innerText?: string): HTMLDivElement {
			const div: HTMLDivElement = document.createElement("div");
	
			if (isHeader) {
				div.classList.add("SampleControl_PortalWebAPIControl_Header");
				elementClassName += "_header";
			}
	
			if (innerText) {
				div.innerText = innerText.toUpperCase();
			}
	
			div.classList.add(elementClassName);
			return div;
		}
	}