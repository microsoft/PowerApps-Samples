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

// Define const here

// Default Image FileName
const DefaultImageFileName = "default.png";

// Show Error css classname
const ShowErrorClassName = "ShowError";

// No Image css classname
const NoImageClassName = "NoImage";

// 'RemoveButton' css class name
const RemoveButtonClassName = "RemoveButton";

export class ImageUploadControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Value of the field is stored and used inside the control
	private _value: string | null;

	// PCF framework context, "Input Properties" containing the parameters, control metadata and interface functions.
	private _context: ComponentFramework.Context<IInputs>;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens.
	private _notifyOutputChanged: () => void;

	// Control's container
	private controlContainer: HTMLDivElement;

	// button element created as part of this control
	private uploadButton: HTMLButtonElement;

	// button element created as part of this control
	private removeButton: HTMLButtonElement;

	// label element created as part of this control
	private imgElement: HTMLImageElement;

	// label element created as part of this control
	private errorLabelElement: HTMLLabelElement;

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
		this._context = context;
		this._notifyOutputChanged = notifyOutputChanged;
		this.controlContainer = document.createElement("div");

		//Create an upload button to upload the image
		this.uploadButton = document.createElement("button");
		// Get the localized string from localized string
		this.uploadButton.innerHTML = context.resources.getString("PCF_ImageUploadControl_Upload_ButtonLabel");
		this.uploadButton.addEventListener("click", this.onUploadButtonClick.bind(this));

		// Creating the label for the control and setting the relevant values.
		this.imgElement = document.createElement("img");

		//Create a remove button to reset the image
		this.removeButton = document.createElement("button");
		this.removeButton.classList.add(RemoveButtonClassName);
		// Get the localized string from localized string
		this.removeButton.innerHTML = context.resources.getString("PCF_ImageUploadControl_Remove_ButtonLabel");
		this.removeButton.addEventListener("click", this.onRemoveButtonClick.bind(this));

		// Create an error label element
		this.errorLabelElement = document.createElement("label");

		// If there is a raw value bound means there already have an image
		if (this._context.parameters.value.raw) {
			this.imgElement.src = this._context.parameters.value.raw;
		} else {
			this.setDefaultImage();
		}

		// Adding the label and button created to the container DIV.
		this.controlContainer.appendChild(this.uploadButton);
		this.controlContainer.appendChild(this.imgElement);
		this.controlContainer.appendChild(this.removeButton);
		this.controlContainer.appendChild(this.errorLabelElement);
		container.appendChild(this.controlContainer);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		// Always need to update the _context obj
		this._context = context;
	}

	/**
	 * Button Event handler for the button created as part of this control
	 * @param event
	 */
	private onUploadButtonClick(): Promise<void> {
		// context.device.pickFile(successCallback, errorCallback) is used to initiate the File Explorer
		// successCallback will be triggered if there successfully pick a file
		// errorCallback will be triggered if there is an error
		return this._context.device.pickFile().then(this.processFile.bind(this), this.showError.bind(this));
	}

	/**
	 * Button Event handler for the button created as part of this control
	 * @param event
	 */
	private onRemoveButtonClick(): void {
		this.setDefaultImage();
	}

	/**
	 *
	 * @param files
	 */
	private processFile(files: ComponentFramework.FileObject[]): void {
		if (files.length) {
			const file: ComponentFramework.FileObject = files[0];

			try {
				let fileExtension: string | undefined;

				if (file?.fileName) {
					fileExtension = file.fileName.split(".").pop();
				}

				if (fileExtension) {
					this.setImage(true, fileExtension, file.fileContent);
					this.controlContainer.classList.remove(NoImageClassName);
				} else {
					this.showError();
				}
			} catch (err) {
				this.showError();
			}
		}
	}

	/**
	 * Set Default Image
	 */
	private setDefaultImage(): void {
		this._context.resources.getResource(
			DefaultImageFileName,
			this.setImage.bind(this, false, "png"),
			this.showError.bind(this)
		);
		this.controlContainer.classList.add(NoImageClassName);

		// If it already has value, we need to update the output
		if (this._context.parameters.value.raw) {
			this._value = null;
			this._notifyOutputChanged();
		}
	}

	/**
	 * Set the Image content
	 * @param shouldUpdateOutput indicate if needs to inform the infra of the change
	 * @param fileType file extension name like "png", "gif", "jpg"
	 * @param fileContent file content, base64 format
	 */
	private setImage(shouldUpdateOutput: boolean, fileType: string, fileContent: string): void {
		const imageUrl: string = this.generateImageSrcUrl(fileType, fileContent);
		this.imgElement.src = imageUrl;

		if (shouldUpdateOutput) {
			this.controlContainer.classList.remove(ShowErrorClassName);
			this._value = imageUrl;
			this._notifyOutputChanged();
		}
	}

	/**
	 * Generate Image Element src url
	 * @param fileType file extension
	 * @param fileContent file content, base 64 format
	 */
	private generateImageSrcUrl(fileType: string, fileContent: string): string {
		return `data:image/${fileType};base64, ${fileContent}`;
	}

	/**
	 *  Show Error Message
	 */
	private showError(): void {
		this.errorLabelElement.innerText = this._context.resources.getString("PCF_ImageUploadControl_Can_Not_Find_File");
		this.controlContainer.classList.add(ShowErrorClassName);
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		// return outputs
		const result: IOutputs = {
			value: this._value ?? undefined,
		};

		return result;
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
