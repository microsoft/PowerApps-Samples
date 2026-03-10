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

export class TableControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Flag to track if control is in full screen mode or not
	private _isFullScreen: boolean;

	// Reference to HTMLTableElement rendered by control
	private _tableElement: HTMLTableElement;

	// Reference to 'Set Full Screen' HTMLButtonElement
	private _setFullScreenButton: HTMLButtonElement;

	// Reference to 'Lookup Objects' HTMLButtonElement
	private _lookupObjectsButton: HTMLButtonElement;

	// Reference to 'Lookup Result Div' HTMLDivElement
	// Used to display information about the item selected by the lookup
	private _lookupObjectsResultDiv: HTMLDivElement;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Reference to ComponentFramework Context object
	private _context: ComponentFramework.Context<IInputs>;

	// Flag if control view has been rendered
	private _controlViewRendered: boolean;

	// Label displayed in lookup result div
	// NOTE: See localization sample control for information on how to localize strings into multiple languages
	private LOOKUP_OBJECRESULT_DIV_STRING = "Item selected by lookupObjects method:";

	// Prefix for label displayed
	// NOTE: See localization sample control for information on how to localize strings into multiple languages
	private BUTTON_LABEL_CLICK_STRING = "Click to invoke:";

	// Name of entity to use for metadata retrieve example (this entity needs to exist in your org)
	private ENTITY_LOGICAL_NAME_FOR_METADATA_EXAMPLE = "account";

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
		this._isFullScreen = false;
		this._controlViewRendered = false;
		this._context = context;
		this._container = document.createElement("div");
		this._container.classList.add("Table_Container");
		container.appendChild(this._container);
	}

	/**
	 * Creates an HTMLButtonElement with the provided label
	 * Attaches the provided method to the "onclick" event of the button element
	 * @param buttonLabel : Label to set on button element
	 * @param onClickHandler : event handler to attach to button's "onclick" event
	 * @param entityName : entityName to store in the button's attribute
	 */
	private createHTMLButtonElement(
		buttonLabel: string,
		onClickHandler: (event: Event) => void,
		entityName: string | null
	): HTMLButtonElement {
		const button: HTMLButtonElement = document.createElement("button");
		button.innerHTML = buttonLabel;
		if (entityName) {
			button.setAttribute("entityName", entityName);
		}

		button.classList.add("SampleControlHtmlTable_ButtonClass");
		button.addEventListener("click", onClickHandler);
		return button;
	}

	/**
	 * Returns the label to display on the 'set full screen' button
	 * @param isFullScreenVal : True if control is currently in 'full screen' mode
	 */
	private getSetFullScreenButtonLabel(isFullScreenVal: boolean): string {
		return `${this.BUTTON_LABEL_CLICK_STRING} setFullScreen(${String(isFullScreenVal)})`;
	}

	/**
	 * Event handler for 'Set Full Screen' button
	 *
	 * This method will transition the control to full screen state if it is currently in non-full screen state, or transition
	 * the control out of full screen state if it is currently in full screen state
	 *
	 * It will also update the label on the 'Set Full Screen' button, and update the interal _isFullScreen state variable
	 * to maintain the control's updated state
	 *
	 * @param event : OnClick Event
	 */
	private onSetFullScreenButtonClick(event: Event): void {
		this._context.mode.setFullScreen(!this._isFullScreen);
		this._setFullScreenButton.innerHTML = this.getSetFullScreenButtonLabel(this._isFullScreen);
		this._isFullScreen = !this._isFullScreen;
	}

	/**
	 * Event handler for 'lookup objects' button
	 *
	 * This method invokes the lookup dialog for the entity name specified by the buttons attribute
	 * Once the user selects an item in the lookup, the selected item is passed back to our callback method.
	 * Our callback method retrieves the id, name, entity type fields from the selected item and injects the
	 * values into a resultDiv on the control to showcase the selected values.
	 *
	 * @param event : OnClick Event
	 */
	private onLookupObjectsButtonClick(event: Event): Promise<void> {
		// Get the entity name for the button
		const entityName: string | null = (event.target as Element)?.getAttribute("entityName");

		const lookUpOptions: ComponentFramework.UtilityApi.LookupOptions = {
			// Note: lookup can support multiple entity types with the below syntax
			// entityTypes: ["account", "contact"]

			entityTypes: [entityName!],
		};

		const lookUpPromise = this._context.utils.lookupObjects(lookUpOptions);

		return lookUpPromise.then(
			// Callback method - invoked after user has selected an item from the lookup dialog
			// Data parameter is the item selected in the lookup dialog
			(data: ComponentFramework.LookupValue[]) => {
				if (data?.[0] && this._lookupObjectsResultDiv) {
					const id: string = data[0].id;
					const name: string | undefined = data[0].name;
					const entityType: string = data[0].entityType;

					let resultHTML: string = this.LOOKUP_OBJECRESULT_DIV_STRING;

					resultHTML += `<br/>Entity ID: ${id}`;
					resultHTML += `<br/>Entity Name: ${name}`;
					resultHTML += `<br/>Entity Type: ${entityType}`;

					this._lookupObjectsResultDiv.innerHTML = resultHTML;
				}
				return;
			},
			(error) => {
				// Error handling code here
			}
		);
	}

	/**
	 * Generates HTML Button that invokes the lookup dialog and appends to custom control container
	 * Generates HTML Div that displays the result of the call to the lookup dialog
	 * @param entityName : name of entity that should be used by the lookup dialog
	 */
	private GenerateLookupObjectElements(entityName: string): void {
		this._lookupObjectsButton = this.createHTMLButtonElement(
			`${this.BUTTON_LABEL_CLICK_STRING} lookupObjects(${entityName})`,
			// eslint-disable-next-line @typescript-eslint/no-misused-promises
			this.onLookupObjectsButtonClick.bind(this),
			entityName
		);

		this._container.appendChild(this._lookupObjectsButton);

		this._lookupObjectsResultDiv = document.createElement("div");
		this._lookupObjectsResultDiv.setAttribute("class", "lookupObjectsResultDiv");

		let resultDivString: string = this.LOOKUP_OBJECRESULT_DIV_STRING;
		resultDivString += "<br />";
		resultDivString += "none";

		this._lookupObjectsResultDiv.innerHTML = resultDivString;
		this._container.appendChild(this._lookupObjectsResultDiv);
	}

	/**
	 * Creates an HTML Table that showcases examples of basic methods available to the custom control
	 * The left column of the table shows the method name or property that is being used
	 * The right column of the table shows the result of that method name or property
	 */
	private createHTMLTableElement(): HTMLTableElement {
		// Create HTML Table Element
		const tableElement: HTMLTableElement = document.createElement("table");
		tableElement.setAttribute("class", "SampleControlHtmlTable_HtmlTable");

		// Create header row for table
		let key = "Example Method";
		let value = "Result";
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, true));

		// Example use of getFormFactor() method
		// Open the control on different form factors to see the value change
		key = "getFormFactor()";
		value = String(this._context.client.getFormFactor());
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example use of getClient() method
		// Open the control on different clients (phone / tablet/ web) to see the value change
		key = "getClient()";
		value = String(this._context.client.getClient());
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of userName property
		// Log in with a different user to see the user name change
		key = "userName";
		value = String(this._context.userSettings.userName);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of isRTL property
		// Update your language to an RTL language (for example: Hebrew) to see this value change
		key = "User Language isRTL";
		value = String(this._context.userSettings.isRTL);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of numberFormattingInfo and formatCurrency
		// Retrieve the currencyDecimalDigits and currencySymbol from the numberFormattingInfo object to retrieve the
		// preferences set in the current users 'User Settings'
		// Pass these values as parameters into the formatting.formatCurrency utility method to format the number per the users preferences
		key = "formatting formatCurrency";
		const numberFormattingInfo: ComponentFramework.UserSettingApi.NumberFormattingInfo =
			this._context.userSettings.numberFormattingInfo;
		const percision: number = numberFormattingInfo.currencyDecimalDigits;
		const currencySymbol: string = numberFormattingInfo.currencySymbol;
		value = this._context.formatting.formatCurrency(100500, percision, currencySymbol);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of formatDateLong
		// Pass a JavaScript Data object set to the current time into formatDateLong method to format the data
		// per the users preferences in 'User Settings'
		key = "formatting formatDateLong";
		value = this._context.formatting.formatDateLong(new Date());
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of getEntityMetadata
		// Retrieve the Entity Metadata for the entityName parameter. In the callback method, retrieve the primaryNameAttribute, logicalName,
		// and isCustomEntity attributes and inject the results into the Example HTML Table
		const metadataPromise = this._context.utils.getEntityMetadata(this.ENTITY_LOGICAL_NAME_FOR_METADATA_EXAMPLE).then(
			(entityMetadata) => {
				// Generate the HTML Elements used for the lookup control example
				this.GenerateLookupObjectElements(this.ENTITY_LOGICAL_NAME_FOR_METADATA_EXAMPLE);
				return;
			},
			(error) => {
				// Error handling code here
			}
		);

		return tableElement;
	}

	/**
	 * Helper method to create an HTML Table Row Element
	 *
	 * @param key : string value to show in left column cell
	 * @param value : string value to show in right column cell
	 * @param isHeaderRow : true if method should generate a header row
	 */
	private createHTMLTableRowElement(key: string, value: string, isHeaderRow: boolean): HTMLTableRowElement {
		const keyCell: HTMLTableCellElement = this.createHTMLTableCellElement(
			key,
			"SampleControlHtmlTable_HtmlCell_Key",
			isHeaderRow
		);
		const valueCell: HTMLTableCellElement = this.createHTMLTableCellElement(
			value,
			"SampleControlHtmlTable_HtmlCell_Value",
			isHeaderRow
		);

		const rowElement: HTMLTableRowElement = document.createElement("tr");
		rowElement.setAttribute("class", "SampleControlHtmlTable_HtmlRow");
		rowElement.appendChild(keyCell);
		rowElement.appendChild(valueCell);

		return rowElement;
	}

	/**
	 * Helper method to create an HTML Table Cell Element
	 *
	 * @param cellValue : string value to inject in the cell
	 * @param className : class name for the cell
	 * @param isHeaderRow : true if method should generate a header row cell
	 */
	private createHTMLTableCellElement(cellValue: string, className: string, isHeaderRow: boolean): HTMLTableCellElement {
		let cellElement: HTMLTableCellElement;
		if (isHeaderRow) {
			cellElement = document.createElement("th");
			cellElement.setAttribute("class", `SampleControlHtmlTable_HtmlHeaderCell ${className}`);
		} else {
			cellElement = document.createElement("td");
			cellElement.setAttribute("class", `SampleControlHtmlTable_HtmlCell ${className}`);
		}

		const textElement: Text = document.createTextNode(cellValue);

		cellElement.appendChild(textElement);
		return cellElement;
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		if (!this._controlViewRendered) {
			// Render and add HTMLTable to the custom control container element
			const tableElement: HTMLTableElement = this.createHTMLTableElement();
			this._container.appendChild(tableElement);

			// Render and add set full screen button to the custom control container element
			this._setFullScreenButton = this.createHTMLButtonElement(
				this.getSetFullScreenButtonLabel(!this._isFullScreen),
				this.onSetFullScreenButtonClick.bind(this),
				null
			);

			this._container.appendChild(this._setFullScreenButton);

			this._controlViewRendered = true;
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
}
