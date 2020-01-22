import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { PointerEvent } from "react";

interface ILocationResult {
	Latitude: Number;
	Longitude: Number;
}

export class DeviceApiComponent implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	private container: HTMLDivElement;
	private result?: ILocationResult;
	private locationClickHandler: EventListener;
	private imageClickHandler: EventListener;

	/**
	 * Empty constructor.
	 */
	constructor() {

	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='starndard', it will receive an empty div element within which it can render its content.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container: HTMLDivElement) {
		this.container = container;
		this.locationClickHandler = this.getLocation.bind(this, context, notifyOutputChanged);
		this.imageClickHandler = this.getImage.bind(this, context);

		this.getResultFromContext(context);

		container.innerHTML = `
			<input type="button" id="getLocationBtn" value="Get Location" />
			<input type="button" id="getImageBtn" value="Get Image" />
			<div id="locationResult"></div>
			<figure>
				<img id="imageResult" src="https://via.placeholder.com/400x250.png?text=No Image Loaded"
					style="height: 250px; width: 400px" />
				<figcaption></figcaption>
			</figure>
		`;

		container.querySelector("input#getLocationBtn")!.addEventListener("pointerup", this.locationClickHandler);
		container.querySelector("input#getImageBtn")!.addEventListener("pointerup", this.imageClickHandler);
	}

	private getResultFromContext(context: ComponentFramework.Context<IInputs>): ILocationResult | null {
		if (context.parameters.Location.raw && context.parameters.Location.raw.length > 0) {
			let location: string[] = context.parameters.Location.raw.split(",");
			location.forEach((val, idx) => { location[idx] = val.trim(); });
			if (location.every((val) => val && val.length > 0)) {
				return {
					Latitude: Number(location[0]),
					Longitude: Number(location[1])
				};
			}
		}

		return null;
	}

	public getImage(context: ComponentFramework.Context<IInputs>): void {
		let lbl: HTMLElement = this.container.querySelector("figcaption") as HTMLElement;

		if (context.device.captureImage) {
			context.device.captureImage({ height: 250, width: 400, allowEdit: true, preferFrontCamera: false, quality: 100 })
				.then((file) => {
					if (file) {
						this.processFile(file, context);
					} else { // if captureImage failed: device not capable, user cancel, etc.
						this.pickFile(context, lbl);
					}
				});
		} else {
			this.pickFile(context, lbl);
		}
	}

	private pickFile(context: ComponentFramework.Context<IInputs>, lbl:HTMLElement):void {
		context.device.pickFile({ accept: "image", allowMultipleFiles: false, maximumAllowedFileSize: 1024 * 1024 * 1024 * 5 /* 5MB */ })
			.then((files) => {
				if (files.length > 0) {
					let file: ComponentFramework.FileObject = files[0];

					this.processFile(file, context);
				} else {
					// no file returned
					lbl.setAttribute("class", "warning");
					lbl.innerText = "⚠ Image not available";
				}
			})
			.catch((error) => this.showError(error, context, "figcaption"));
	}

	private processFile(file: ComponentFramework.FileObject, context: ComponentFramework.Context<IInputs>): void {
		try {
			let fileExtension: string | undefined;
			let caption:HTMLElement = this.container.querySelector("figcaption")! as HTMLElement;

			if (file && file.fileContent && file.mimeType && file.mimeType.length > 0) {
				(this.container.querySelector("img#imageResult")! as HTMLImageElement).src =
					`data:${file.mimeType};base64, ${file.fileContent}`;
					caption.innerText = `✔ ${file.fileName}`;
					caption.setAttribute("class", "success");
			} else {
				this.showError(new Error("Could not determine the image type"), context, "figcaption");
			}
		} catch (error) { this.showError(error, context, "figcaption"); }
	}

	private showError(error: Error, context: ComponentFramework.Context<IInputs>, selector: string): void {
		let element:HTMLElement = this.container.querySelector(selector) as HTMLElement;

		element.setAttribute("class", "error");
		element.innerText = `❌ Error: ${error.message}`;
	}

	public getLocation(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void): void {
		if (context.device.getCurrentPosition) {
			let lbl: HTMLDivElement = this.container.querySelector("div#locationResult") as HTMLDivElement;

			try {
				context.device.getCurrentPosition()
					.then((location) => {
						if (location && location.coords && location.coords.latitude && location.coords.longitude) {
							lbl.setAttribute("class", "success");

							this.result = {
								Latitude: location.coords.latitude,
								Longitude: location.coords.longitude
							};
							notifyOutputChanged();
						} else {
							lbl.setAttribute("class", "warning");
							lbl.innerText = "⚠ Location data not available";
						}
					})
					.catch((error) => {
						// location not available from host device/app
						lbl.setAttribute("class", "warning");
						lbl.innerText = "⚠ Location data not available";
					});
			} catch (error) {
				this.showError(error, context, "div#locationResult");
			}
		}
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		if (context.updatedProperties.includes("Location")) {
			this.result = this.getResultFromContext(context) || undefined;

			let lbl: HTMLDivElement = this.container.querySelector("div#locationResult") as HTMLDivElement;
			lbl.innerText = `✔ Location: ${this.getResultString()}`;

			if (this.result) {
				this.result = {
					Latitude: this.result.Latitude,
					Longitude: this.result.Longitude
				};
			}
		}
	}

	private getResultString(): string | undefined {
		return this.result ? `${this.result.Latitude}, ${this.result.Longitude}` : undefined;
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs {
		return {
			Location: this.getResultString()
		};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		this.container.querySelector("#getLocationBtn")!.removeEventListener("onpointerup", this.locationClickHandler);
		this.container.querySelector("#getImageBtn")!.removeEventListener("onpointerup", this.imageClickHandler);
	}
}