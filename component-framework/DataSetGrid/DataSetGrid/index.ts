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

// Style name of Load More Button
const DataSetControl_LoadMoreButton_Hidden_Style = "DataSetControl_LoadMoreButton_Hidden_Style";

export class DataSetGrid implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Cached context object for the latest updateView
	private contextObj: ComponentFramework.Context<IInputs>;

	// Div element created as part of this control's main container
	private mainContainer: HTMLDivElement;

	// Table element created as part of this control's table
	private gridContainer: HTMLDivElement;

	// Button element created as part of this control
	private loadPageButton: HTMLButtonElement;

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
		// Need to track container resize so that control could get the available width. The available height won't be provided even this is true
		context.mode.trackContainerResize(true);

		// Create main table container div.
		this.mainContainer = document.createElement("div");

		// Create data table container div.
		this.gridContainer = document.createElement("div");
		this.gridContainer.classList.add("DataSetControl_grid-container");

		// Create data table container div.
		this.loadPageButton = document.createElement("button");
		this.loadPageButton.setAttribute("type", "button");
		this.loadPageButton.innerText = context.resources.getString("PCF_DataSetControl_LoadMore_ButtonLabel");
		this.loadPageButton.classList.add(DataSetControl_LoadMoreButton_Hidden_Style);
		this.loadPageButton.classList.add("DataSetControl_LoadMoreButton_Style");
		this.loadPageButton.addEventListener("click", this.onLoadMoreButtonClick.bind(this));

		// Adding the main table and loadNextPage button created to the container DIV.
		this.mainContainer.appendChild(this.gridContainer);
		this.mainContainer.appendChild(this.loadPageButton);
		this.mainContainer.classList.add("DataSetControl_main-container");
		container.appendChild(this.mainContainer);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this.contextObj = context;
		this.toggleLoadMoreButtonWhenNeeded(context.parameters.dataSetGrid);

		if (!context.parameters.dataSetGrid.loading) {
			// Get sorted columns on View
			const columnsOnView = this.getSortedColumnsOnView(context);

			if (!columnsOnView || columnsOnView.length === 0) {
				return;
			}

			while (this.gridContainer.firstChild) {
				this.gridContainer.removeChild(this.gridContainer.firstChild);
			}

			this.gridContainer.appendChild(this.createGridBody(columnsOnView, context.parameters.dataSetGrid));
		}
		// this is needed to ensure the scroll bar appears automatically when the grid resize happens and all the tiles are not visible on the screen.
		this.mainContainer.style.maxHeight = `${window.innerHeight - this.gridContainer.offsetTop - 75}px`;
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

	/**
	 * Get sorted columns on view
	 * @param context
	 * @return sorted columns object on View
	 */
	private getSortedColumnsOnView(context: ComponentFramework.Context<IInputs>): DataSetInterfaces.Column[] {
		if (!context.parameters.dataSetGrid.columns) {
			return [];
		}

		const columns = context.parameters.dataSetGrid.columns.filter((columnItem: DataSetInterfaces.Column) => {
			// some column are supplementary and their order is not > 0
			return columnItem.order >= 0;
		});

		// Sort those columns so that they will be rendered in order
		columns.sort((a: DataSetInterfaces.Column, b: DataSetInterfaces.Column) => {
			return a.order - b.order;
		});

		return columns;
	}

	/**
	 * funtion that creates the body of the grid and embeds the content onto the tiles.
	 * @param columnsOnView columns on the view whose value needs to be shown on the UI
	 * @param gridParam data of the Grid
	 */
	private createGridBody(columnsOnView: DataSetInterfaces.Column[], gridParam: DataSet): HTMLDivElement {
		const gridBody: HTMLDivElement = document.createElement("div");

		if (gridParam.sortedRecordIds.length > 0) {
			for (const currentRecordId of gridParam.sortedRecordIds) {
				const gridRecord: HTMLDivElement = document.createElement("div");
				gridRecord.classList.add("DataSetControl_grid-item");
				gridRecord.addEventListener("click", this.onRowClick.bind(this));

				// Set the recordId on the row dom
				gridRecord.setAttribute(RowRecordId, gridParam.records[currentRecordId].getRecordId());

				columnsOnView.forEach((columnItem) => {
					const labelPara = document.createElement("p");
					labelPara.classList.add("DataSetControl_grid-text-label");

					const valuePara = document.createElement("p");
					valuePara.classList.add("DataSetControl_grid-text-value");

					labelPara.textContent = `${columnItem.displayName} : `;
					gridRecord.appendChild(labelPara);
					if (
						gridParam.records[currentRecordId].getFormattedValue(columnItem.name) != null &&
						gridParam.records[currentRecordId].getFormattedValue(columnItem.name) != ""
					) {
						valuePara.textContent = gridParam.records[currentRecordId].getFormattedValue(columnItem.name);
						gridRecord.appendChild(valuePara);
					} else {
						valuePara.textContent = "-";
						gridRecord.appendChild(valuePara);
					}
				});

				gridBody.appendChild(gridRecord);
			}
		} else {
			const noRecordLabel: HTMLDivElement = document.createElement("div");
			noRecordLabel.classList.add("DataSetControl_grid-norecords");
			noRecordLabel.style.width = `${this.contextObj.mode.allocatedWidth - 25}px`;
			noRecordLabel.innerHTML = this.contextObj.resources.getString("PCF_DataSetControl_No_Record_Found");
			gridBody.appendChild(noRecordLabel);
		}

		return gridBody;
	}

	/**
	 * Row Click Event handler for the associated row when being clicked
	 * @param event
	 */
	private onRowClick(event: Event): void {
		const rowRecordId = (event.currentTarget as HTMLTableRowElement).getAttribute(RowRecordId);

		if (rowRecordId) {
			const entityReference = this.contextObj.parameters.dataSetGrid.records[rowRecordId].getNamedReference();
			const entityFormOptions = {
				entityName: entityReference.name,
				entityId: entityReference.id.guid,
			};
			void this.contextObj.navigation.openForm(entityFormOptions);
		}
	}

	/**
	 * Toggle 'LoadMore' button when needed
	 */
	private toggleLoadMoreButtonWhenNeeded(gridParam: DataSet): void {
		if (
			gridParam.paging.hasNextPage &&
			this.loadPageButton.classList.contains(DataSetControl_LoadMoreButton_Hidden_Style)
		) {
			this.loadPageButton.classList.remove(DataSetControl_LoadMoreButton_Hidden_Style);
		} else if (
			!gridParam.paging.hasNextPage &&
			!this.loadPageButton.classList.contains(DataSetControl_LoadMoreButton_Hidden_Style)
		) {
			this.loadPageButton.classList.add(DataSetControl_LoadMoreButton_Hidden_Style);
		}
	}

	/**
	 * 'LoadMore' Button Event handler when load more button clicks
	 * @param event
	 */
	private onLoadMoreButtonClick(): void {
		this.contextObj.parameters.dataSetGrid.paging.loadNextPage();
		this.toggleLoadMoreButtonWhenNeeded(this.contextObj.parameters.dataSetGrid);
	}
}
