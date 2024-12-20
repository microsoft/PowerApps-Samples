import { IInputs, IOutputs } from "./generated/ManifestTypes";
import * as React from "react";
import { StaticDataSchema, StaticData } from "./StaticData";
import { MainContainer } from "./MainContainer";

export class ObjectOutputControl implements ComponentFramework.ReactControl<IInputs, IOutputs> {
	private notifyOutputChanged: () => void;
	private _staticData?: typeof StaticData;

	/**
	 * Empty constructor.
	 */
	constructor() {
		// Empty
	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void): void {
		context.mode.trackContainerResize(true);
		this.notifyOutputChanged = notifyOutputChanged;
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 * @returns ReactElement root react element for the control
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
		return React.createElement(MainContainer, {
			width: context.mode.allocatedWidth,
			height: context.mode.allocatedHeight,
			onLoadData: this.onLoadData,
			onClearData: this.onClearData,
		});
	}

	/**
	 * It is called by the framework prior to a control init to get the output object(s) schema
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 * @returns an object schema based on nomenclature defined in manifest
	 */
	public async getOutputSchema(context: ComponentFramework.Context<IInputs>): Promise<Record<string, unknown>> {
		return Promise.resolve({
			Data: StaticDataSchema,
		});
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs {
		return {
			Data: this._staticData,
		};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// Add code to cleanup control if necessary
	}

	/**
	 * Sets the output object and notifies the framework that the control has new outputs ready.
	 */
	private onLoadData = () => {
		this._staticData = StaticData;
		this._staticData.loadCounter = (this._staticData.loadCounter || 0) + 1;
		this.notifyOutputChanged();
	};

	/**
	 * Clears the output object and notifies the framework that the control has new outputs ready.
	 */
	private onClearData = () => {
		this._staticData = undefined;
		this.notifyOutputChanged();
	};
}
