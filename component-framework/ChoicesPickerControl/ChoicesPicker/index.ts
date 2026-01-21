import { IInputs, IOutputs } from "./generated/ManifestTypes";
import * as React from "react";
import * as ReactDOM from "react-dom";
import { initializeIcons } from "@fluentui/react/lib/Icons";
import { ChoicesPickerComponent } from "./ChoicesPickerComponent";

initializeIcons(undefined, { disableWarnings: true });
const SmallFormFactorMaxWidth = 350;

const enum FormFactors {
	Unknown = 0,
	Desktop = 1,
	Tablet = 2,
	Phone = 3,
}

export class ChoicesPicker implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	notifyOutputChanged: () => void;
	rootContainer: HTMLDivElement;
	selectedValue: number | undefined;
	context: ComponentFramework.Context<IInputs>;

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
		// Add control initialization code
		this.notifyOutputChanged = notifyOutputChanged;
		this.rootContainer = container;
		this.context = context;
		this.context.mode.trackContainerResize(true);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		const { value, configuration } = context.parameters;

		let disabled = context.mode.isControlDisabled;
		let masked = false;
		if (value.security) {
			disabled = disabled || !value.security.editable;
			masked = !value.security.readable;
		}

		if (value?.attributes && configuration) {
			ReactDOM.render(
				React.createElement(ChoicesPickerComponent, {
					label: value.attributes.DisplayName,
					options: value.attributes.Options,
					configuration: configuration.raw,
					value: value.raw,
					onChange: this.onChange,
					disabled: disabled,
					masked: masked,
					formFactor:
						context.client.getFormFactor() == (FormFactors.Phone as number) ||
						context.mode.allocatedWidth < SmallFormFactorMaxWidth
							? "small"
							: "large",
				}),
				this.rootContainer
			);
		}
	}

	onChange = (newValue: number | undefined): void => {
		this.selectedValue = newValue;
		this.notifyOutputChanged();
	};

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs {
		return { value: this.selectedValue } as IOutputs;
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// Add code to cleanup control if necessary
		ReactDOM.unmountComponentAtNode(this.rootContainer);
	}
}
