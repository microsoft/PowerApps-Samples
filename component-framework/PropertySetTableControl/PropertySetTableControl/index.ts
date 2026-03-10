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

import DataSetInterfaces = ComponentFramework.PropertyHelper.DataSetApi;
type DataSet = ComponentFramework.PropertyTypes.DataSet;

// Define const here
const RowRecordId = "rowRecId";

// Style name of disabled buttons
const Button_Disabled_style = "loadNextPageButton_Disabled_Style";

export class PropertySetTableControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	private contextObj: ComponentFramework.Context<IInputs>;

	// Div element created as part of this control's main container
	private mainContainer: HTMLDivElement;

	// Table element created as part of this control's table
	private dataTable: HTMLTableElement;

	// Button element created as part of this control
	private loadNextPageButton: HTMLButtonElement;

	// Button element created as part of this control
	private loadPrevPageButton: HTMLButtonElement;

	private getValueResultLabel: HTMLLabelElement;

	private selectedRecord: DataSetInterfaces.EntityRecord;

	private selectedRecords: Record<string, boolean> = {};

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
		// Need to track container resize so that control could get the available width.
		// In Model-driven app, the available height won't be provided even this is true
		// In Canvas-app, the available height will be provided in context.mode.allocatedHeight
		context.mode.trackContainerResize(true);

		// Create main table container div.
		this.mainContainer = document.createElement("div");
		this.mainContainer.classList.add("SimpleTable_MainContainer_Style");
		this.mainContainer.id = "SimpleTableMainContainer";
		// Create data table container div.
		this.dataTable = document.createElement("table");
		this.dataTable.classList.add("SimpleTable_Table_Style");

		this.loadPrevPageButton = document.createElement("button");
		this.loadPrevPageButton.setAttribute("type", "button");
		this.loadPrevPageButton.innerText = context.resources.getString("PropertySetTableControl_LoadPrev_ButtonLabel");
		this.loadPrevPageButton.classList.add(Button_Disabled_style);
		this.loadPrevPageButton.classList.add("Button_Style");
		this.loadPrevPageButton.addEventListener("click", this.onLoadPrevButtonClick.bind(this));

		this.loadNextPageButton = document.createElement("button");
		this.loadNextPageButton.setAttribute("type", "button");
		this.loadNextPageButton.innerText = context.resources.getString("PropertySetTableControl_LoadNext_ButtonLabel");
		this.loadNextPageButton.classList.add(Button_Disabled_style);
		this.loadNextPageButton.classList.add("Button_Style");
		this.loadNextPageButton.addEventListener("click", this.onLoadNextButtonClick.bind(this));

		// Create main table container div.
		this.mainContainer = document.createElement("div");

		// Adding the main table and loadNextPage button created to the container DIV.
		this.mainContainer.appendChild(this.createGetValueDiv());
		this.mainContainer.appendChild(this.loadPrevPageButton);
		this.mainContainer.appendChild(this.loadNextPageButton);
		this.mainContainer.appendChild(this.dataTable);
		this.mainContainer.classList.add("main-container");
		container.appendChild(this.mainContainer);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this.contextObj = context;
		this.toggleLoadMoreButtonWhenNeeded(context.parameters.sampleDataSet);
		this.toggleLoadPreviousButtonWhenNeeded(context.parameters.sampleDataSet);

		if (!context.parameters.sampleDataSet.loading) {
			// Get sorted columns on View
			const columnsOnView = this.getSortedColumnsOnView(context);
			if (!columnsOnView?.length) {
				return;
			}

			//calculate the width for each column
			const columnWidthDistribution = this.getColumnWidthDistribution(context, columnsOnView);

			//When new data is received, it needs to first remove the table element, allowing it to properly render a table with updated data
			//This only needs to be done on elements having child elements which is tied to data received from canvas/model ..
			while (this.dataTable.firstChild) {
				this.dataTable.removeChild(this.dataTable.firstChild);
			}

			this.dataTable.appendChild(this.createTableHeader(columnsOnView, columnWidthDistribution));
			this.dataTable.appendChild(
				this.createTableBody(columnsOnView, columnWidthDistribution, context.parameters.sampleDataSet)
			);

			if (this.dataTable.parentElement) {
				this.dataTable.parentElement.style.height = `${context.mode.allocatedHeight - 50}px`;
			}
		}
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
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

	private createGetValueDiv(): HTMLDivElement {
		const getValueDiv = document.createElement("div");
		const inputBox = document.createElement("input");
		const getValueButton = document.createElement("button");
		const resultText = document.createElement("label");

		inputBox.id = "getValueInputBox";
		inputBox.placeholder = "select a row and enter the alias name";
		inputBox.classList.add("GetValueInput_Style");

		getValueButton.innerText = "GetValue";
		getValueButton.onclick = () => {
			if (this.selectedRecord) {
				resultText.innerText = this.selectedRecord.getFormattedValue(inputBox.value);
			}
		};
		resultText.innerText = "Select a row first";
		resultText.classList.add("GetValueResult_Style");
		this.getValueResultLabel = resultText;
		getValueDiv.appendChild(inputBox);
		getValueDiv.appendChild(getValueButton);
		getValueDiv.appendChild(resultText);
		return getValueDiv;
	}

	/**
	 * Get sorted columns on view, columns are sorted by DataSetInterfaces.Column.order
	 * Property-set columns will always have order = -1.
	 * In Model-driven app, the columns are ordered in the same way as columns defined in views.
	 * In Canvas-app, the columns are ordered by the sequence fields added to control
	 * Note that property set columns will have order = 0 in test harness, this is a bug.
	 * @param context
	 * @return sorted columns object on View
	 */
	private getSortedColumnsOnView(context: ComponentFramework.Context<IInputs>): DataSetInterfaces.Column[] {
		if (!context.parameters.sampleDataSet.columns) {
			return [];
		}

		const columns = context.parameters.sampleDataSet.columns;

		return columns;
	}

	/**
	 * Get column width distribution using visualSizeFactor.
	 * In model-driven app, visualSizeFactor can be configured from view's settiong.
	 * In Canvas app, currently there is no way to configure this value. In all data sources, all columns will have the same visualSizeFactor value.
	 * Control does not have to render the control using these values, controls are free to display any columns with any width, or making column width adjustable.
	 * However, these kind of configurations will be lost when leaving the page
	 * @param context context object of this cycle
	 * @param columnsOnView columns array on the configured view
	 * @returns column width distribution
	 */
	private getColumnWidthDistribution(
		context: ComponentFramework.Context<IInputs>,
		columnsOnView: DataSetInterfaces.Column[]
	): string[] {
		const widthDistribution: string[] = [];

		// Considering need to remove border & padding length
		const totalWidth: number = context.mode.allocatedWidth;
		const widthSum = columnsOnView.reduce((sum, columnItem) => (sum += columnItem.visualSizeFactor), 0);

		let remainWidth: number = totalWidth;

		columnsOnView.forEach((item, index) => {
			let widthPerCell = "";
			if (index !== columnsOnView.length - 1) {
				const cellWidth = Math.round((item.visualSizeFactor / widthSum) * totalWidth);
				remainWidth = remainWidth - cellWidth;
				widthPerCell = `${cellWidth}px`;
			} else {
				widthPerCell = `${remainWidth}px`;
			}
			widthDistribution.push(widthPerCell);
		});

		return widthDistribution;
	}

	private createTableHeader(
		columnsOnView: DataSetInterfaces.Column[],
		widthDistribution: string[]
	): HTMLTableSectionElement {
		const tableHeader: HTMLTableSectionElement = document.createElement("thead");
		const tableHeaderRow: HTMLTableRowElement = document.createElement("tr");
		tableHeaderRow.classList.add("SimpleTable_TableRow_Style");
		columnsOnView.forEach((columnItem, index) => {
			const tableHeaderCell = document.createElement("th");
			const innerDiv = document.createElement("div");
			innerDiv.classList.add("SimpleTable_TableCellInnerDiv_Style");
			innerDiv.style.maxWidth = widthDistribution[index];
			let columnDisplayName: string;
			if (columnItem.order < 0) {
				tableHeaderCell.classList.add("SimpleTable_TableHeader_PropertySet_Style");
				columnDisplayName = `${columnItem.displayName}(propertySet)`;
			} else {
				tableHeaderCell.classList.add("SimpleTable_TableHeader_Style");
				columnDisplayName = columnItem.displayName;
			}
			innerDiv.innerText = columnDisplayName;

			tableHeaderCell.appendChild(innerDiv);
			tableHeaderRow.appendChild(tableHeaderCell);
		});

		tableHeader.appendChild(tableHeaderRow);
		return tableHeader;
	}

	private createTableBody(
		columnsOnView: DataSetInterfaces.Column[],
		widthDistribution: string[],
		gridParam: DataSet
	): HTMLTableSectionElement {
		const tableBody: HTMLTableSectionElement = document.createElement("tbody");

		if (gridParam.sortedRecordIds.length > 0) {
			for (const currentRecordId of gridParam.sortedRecordIds) {
				const tableRecordRow: HTMLTableRowElement = document.createElement("tr");
				tableRecordRow.classList.add("SimpleTable_TableRow_Style");
				tableRecordRow.addEventListener("click", this.onRowClick.bind(this));

				// Set the recordId on the row dom, this is the simplest way to help us track which record has been clicked.
				tableRecordRow.setAttribute(RowRecordId, gridParam.records[currentRecordId].getRecordId());

				columnsOnView.forEach((columnItem, index) => {
					const tableRecordCell = document.createElement("td");
					tableRecordCell.classList.add("SimpleTable_TableCell_Style");
					const innerDiv = document.createElement("div");
					innerDiv.classList.add("SimpleTable_TableCellInnerDiv_Style");
					innerDiv.style.width = widthDistribution[index];
					// Currently there is a bug in canvas preventing retrieving value using alias for property set columns.
					// In this sample, we use the column's actual attribute name to retrieve the formatted value to work around the issue
					// columnItem.alias should be used after bug is addressed
					innerDiv.innerText = gridParam.records[currentRecordId].getFormattedValue(columnItem.name);
					tableRecordCell.appendChild(innerDiv);
					tableRecordRow.appendChild(tableRecordCell);
				});

				tableBody.appendChild(tableRecordRow);
			}
		} else {
			const tableRecordRow: HTMLTableRowElement = document.createElement("tr");
			const tableRecordCell: HTMLTableCellElement = document.createElement("td");
			tableRecordCell.classList.add("No_Record_Style");
			tableRecordCell.colSpan = columnsOnView.length;
			tableRecordCell.innerText = this.contextObj.resources.getString("PropertySetTableControl_No_Record_Found");
			tableRecordRow.appendChild(tableRecordCell);
			tableBody.appendChild(tableRecordRow);
		}

		return tableBody;
	}

	/**
	 * Row Click Event handler for the associated row when being clicked
	 * @param event
	 */
	private onRowClick(event: Event): void {
		const rowElement = event.currentTarget as HTMLTableRowElement;
		const rowRecordId = rowElement.getAttribute(RowRecordId);
		if (rowRecordId) {
			const record = this.contextObj.parameters.sampleDataSet.records[rowRecordId];
			this.selectedRecord = record;
			this.getValueResultLabel.innerText = "";
			this.selectedRecords[rowRecordId] = !this.selectedRecords[rowRecordId];
			const selectedRecordsArray = [];
			for (const recordId in this.selectedRecords) {
				if (this.selectedRecords[recordId]) {
					selectedRecordsArray.push(recordId);
				}
			}
			this.contextObj.parameters.sampleDataSet.setSelectedRecordIds(selectedRecordsArray);
			this.contextObj.parameters.sampleDataSet.openDatasetItem(record.getNamedReference());
		}
	}

	/**
	 * Toggle 'LoadMore' button when needed
	 */
	private toggleLoadMoreButtonWhenNeeded(gridParam: DataSet): void {
		if (gridParam.paging.hasNextPage) {
			this.loadNextPageButton.disabled = false;
		} else if (!gridParam.paging.hasNextPage) {
			this.loadNextPageButton.disabled = true;
		}
	}

	/**
	 * Toggle 'LoadMore' button when needed
	 */
	private toggleLoadPreviousButtonWhenNeeded(gridParam: DataSet): void {
		if (gridParam.paging.hasPreviousPage) {
			this.loadPrevPageButton.disabled = false;
		} else if (!gridParam.paging.hasPreviousPage) {
			this.loadPrevPageButton.disabled = true;
		}
	}

	/**
	 * 'LoadMore' Button Event handler when load more button clicks
	 * @param event
	 */
	private onLoadNextButtonClick(event: Event): void {
		this.contextObj.parameters.sampleDataSet.paging.loadNextPage();
		this.toggleLoadMoreButtonWhenNeeded(this.contextObj.parameters.sampleDataSet);
		this.toggleLoadPreviousButtonWhenNeeded(this.contextObj.parameters.sampleDataSet);
	}

	/**
	 * 'LoadPrevous' Button Event handler when load more button clicks
	 * @param event
	 */
	private onLoadPrevButtonClick(event: Event): void {
		this.contextObj.parameters.sampleDataSet.paging.loadPreviousPage();
		this.toggleLoadPreviousButtonWhenNeeded(this.contextObj.parameters.sampleDataSet);
		this.toggleLoadMoreButtonWhenNeeded(this.contextObj.parameters.sampleDataSet);
	}
}
