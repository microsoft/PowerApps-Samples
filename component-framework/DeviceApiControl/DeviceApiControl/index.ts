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

interface ILocationResult {
	Latitude: number;
	Longitude: number;
}

export class DeviceApiControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	private container: HTMLDivElement;
	private result?: ILocationResult;
	private locationClickHandler: EventListener;
	private imageClickHandler: EventListener;
	private getBarcodeClickHandler: EventListener;
	private captureAudioHandler: EventListener;
	private captureVideoHandler: EventListener;

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
		this.container = container;
		this.locationClickHandler = this.getLocation.bind(this, context, notifyOutputChanged);
		this.imageClickHandler = this.getImage.bind(this, context);
		this.captureAudioHandler = this.captureAudio.bind(this, context);
		this.captureVideoHandler = this.captureVideo.bind(this, context);
		this.getBarcodeClickHandler = this.getBarcode.bind(this, context);

		this.getResultFromContext(context);

		container.innerHTML = `
			<div style="display: flex; flex-direction: row; align-items: center;">
				<div style="display: flex; flex-direction: column;">
					<input type="button" id="getLocationBtn" value="Get Location" />
					<input type="button" id="getImageBtn" value="Get Image" />
					<input type="button" id="captureVideo" value="Capture Video" />
					<input type="button" id="captureAudio" value="Capture Audio" />
					<input type="button" id="getBarcode" value="Get Barcode" />
				</div>
				<div style="display: flex; flex-direction: column;">
					<div id="locationResult"></div>
					<figure>
						<img id="imageResult" src="https://via.placeholder.com/400x250.png?text=No Image Loaded"
							style="height: 250px; width: 400px" />
						<figcaption></figcaption>
					</figure>
				</div>
			</div>
		`;

		container.querySelector("input#getLocationBtn")?.addEventListener("pointerup", this.locationClickHandler);
		container.querySelector("input#getImageBtn")?.addEventListener("pointerup", this.imageClickHandler);
		container.querySelector("input#captureVideo")?.addEventListener("pointerup", this.captureVideoHandler);
		container.querySelector("input#captureAudio")?.addEventListener("pointerup", this.captureAudioHandler);
		container.querySelector("input#getBarcode")?.addEventListener("pointerup", this.getBarcodeClickHandler);
	}

	private getResultFromContext(context: ComponentFramework.Context<IInputs>): ILocationResult | null {
		if (context.parameters.Location?.raw?.length) {
			const location: string[] = context.parameters.Location.raw.split(",");
			location.forEach((val, idx) => {
				location[idx] = val.trim();
			});
			if (location.every((val) => val && val.length > 0)) {
				return {
					Latitude: Number(location[0]),
					Longitude: Number(location[1]),
				};
			}
		}

		return null;
	}

	public getImage(context: ComponentFramework.Context<IInputs>): Promise<void> | void {
		const lbl: HTMLElement = this.container.querySelector("figcaption")!;

		if (context.device.captureImage) {
			return context.device
				.captureImage({ height: 250, width: 400, allowEdit: true, preferFrontCamera: false, quality: 100 })
				.then((file) => {
					if (file) {
						this.processFile(file, context);
					} else {
						// if captureImage failed: device not capable, user cancel, etc.
						return this.pickFile(context, lbl);
					}
					return;
				});
		} else {
			return this.pickFile(context, lbl);
		}
	}

	public captureAudio(context: ComponentFramework.Context<IInputs>): Promise<void> | void {
		try {
			return context.device
				.captureAudio()
				.then((audioFile) => {
					alert(`Success ${audioFile.fileName}, ${audioFile.fileSize}, ${audioFile.mimeType}`);
					console.log(audioFile);
					return;
				})
				.catch((er) => console.log(er));
		} catch (err) {
			alert(err);
		}
	}

	public captureVideo(context: ComponentFramework.Context<IInputs>): Promise<void> | void {
		try {
			return context.device
				.captureVideo()
				.then((videoFile) => {
					alert(`Success ${videoFile.fileName}, ${videoFile.fileSize}, ${videoFile.mimeType}`);
					console.log(videoFile);
					return;
				})
				.catch((er) => console.log(er));
		} catch (err) {
			alert(err);
		}
	}

	public getBarcode(context: ComponentFramework.Context<IInputs>): Promise<void> | void {
		try {
			return context.device
				.getBarcodeValue()
				.then((barcode) => {
					alert(barcode);
					return;
				})
				.catch((er) => console.log(er));
		} catch (err) {
			alert(err);
		}
	}

	private pickFile(context: ComponentFramework.Context<IInputs>, lbl: HTMLElement): Promise<void> {
		return context.device
			.pickFile({
				accept: "image",
				allowMultipleFiles: false,
				maximumAllowedFileSize: 1024 * 1024 * 1024 * 5 /* 5MB */,
			})
			.then((files) => {
				if (files.length > 0) {
					const file: ComponentFramework.FileObject = files[0];

					this.processFile(file, context);
				} else {
					// no file returned
					lbl.setAttribute("class", "warning");
					lbl.innerText = "⚠ Image not available";
				}
				return;
			})
			.catch((error) => this.showError(error as Error, context, "figcaption"));
	}

	private processFile(file: ComponentFramework.FileObject, context: ComponentFramework.Context<IInputs>): void {
		try {
			const caption: HTMLElement = this.container.querySelector("figcaption")!;

			if (file?.fileContent && file.mimeType?.length) {
				// eslint-disable-next-line @typescript-eslint/non-nullable-type-assertion-style
				(this.container.querySelector("img#imageResult") as HTMLImageElement).src =
					`data:${file.mimeType};base64, ${file.fileContent}`;
				caption.innerText = `✔ ${file.fileName}`;
				caption.setAttribute("class", "success");
			} else {
				this.showError(new Error("Could not determine the image type"), context, "figcaption");
			}
		} catch (error) {
			this.showError(error as Error, context, "figcaption");
		}
	}

	private showError(error: Error, context: ComponentFramework.Context<IInputs>, selector: string): void {
		const element: HTMLElement = this.container.querySelector(selector)!;

		element.setAttribute("class", "error");
		element.innerText = `❌ Error: ${error.message}`;
	}

	public getLocation(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void
	): Promise<void> | void {
		if (context.device.getCurrentPosition) {
			const lbl: HTMLDivElement = this.container.querySelector("div#locationResult")!;

			try {
				return context.device
					.getCurrentPosition()
					.then((location) => {
						if (location?.coords?.latitude && location?.coords?.longitude) {
							lbl.setAttribute("class", "success");

							this.result = {
								Latitude: location.coords.latitude,
								Longitude: location.coords.longitude,
							};
							notifyOutputChanged();
						} else {
							lbl.setAttribute("class", "warning");
							lbl.innerText = "⚠ Location data not available";
						}
						return;
					})
					.catch(() => {
						// location not available from host device/app
						lbl.setAttribute("class", "warning");
						lbl.innerText = "⚠ Location data not available";
					});
			} catch (error) {
				this.showError(error as Error, context, "div#locationResult");
			}
		}
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		if (context.updatedProperties.includes("Location")) {
			this.result = this.getResultFromContext(context) ?? undefined;

			const lbl: HTMLDivElement = this.container.querySelector("div#locationResult")!;
			lbl.innerText = `✔ Location: ${this.getResultString()}`;

			if (this.result) {
				this.result = {
					Latitude: this.result.Latitude,
					Longitude: this.result.Longitude,
				};
			}
		}
	}

	private getResultString(): string | undefined {
		return this.result ? `${this.result.Latitude}, ${this.result.Longitude}` : undefined;
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		return {
			Location: this.getResultString(),
		};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		this.container.querySelector("#getLocationBtn")?.removeEventListener("onpointerup", this.locationClickHandler);
		this.container.querySelector("#getImageBtn")?.removeEventListener("onpointerup", this.imageClickHandler);
	}
}
