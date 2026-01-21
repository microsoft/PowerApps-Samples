import { IInputs, IOutputs } from "./generated/ManifestTypes";
import * as React from "react";
import { FacepileBasicExample, IFacepileBasicExampleProps } from "./components/Facepile";

const DEFAULT_NUMBER_OF_FACES = 3;

export class FacepileReact implements ComponentFramework.ReactControl<IInputs, IOutputs> {
	private notifyOutputChanged: () => void;
	private props: IFacepileBasicExampleProps = {
		numberFacesChanged: this.numberFacesChanged.bind(this),
	};

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
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary
	): void {
		this.notifyOutputChanged = notifyOutputChanged;
		// eslint-disable-next-line @typescript-eslint/prefer-nullish-coalescing
		this.props.numberOfFaces = context.parameters.numberOfFaces.raw || DEFAULT_NUMBER_OF_FACES;
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
		if (context.updatedProperties.includes("numberOfFaces")) {
			// eslint-disable-next-line @typescript-eslint/prefer-nullish-coalescing
			this.props.numberOfFaces = context.parameters.numberOfFaces.raw || DEFAULT_NUMBER_OF_FACES;
		}
		return React.createElement(FacepileBasicExample, this.props);
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs {
		return {
			numberOfFaces: this.props.numberOfFaces,
		};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no op
	}

	private numberFacesChanged(newValue: number) {
		if (this.props.numberOfFaces !== newValue) {
			this.props.numberOfFaces = newValue;
			this.notifyOutputChanged();
		}
	}
}
