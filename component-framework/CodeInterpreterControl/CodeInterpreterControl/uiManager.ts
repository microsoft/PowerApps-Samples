import { IInputs } from "./generated/ManifestTypes";

/**
 * Encapsulates all DOM operations for the control (iframe content, sizing, and spinner overlay).
 */
export class UIManager {
	private context: ComponentFramework.Context<IInputs>;
	private container: HTMLDivElement;
	private iframe: HTMLIFrameElement;
	private spinner: HTMLDivElement | null = null;

	constructor(context: ComponentFramework.Context<IInputs>, container: HTMLDivElement) {
		this.context = context;
		this.container = container;
		this.initializeContainer();
		this.createIframe();
	}

	/** Sets up base layout styles on the host container. */
	private initializeContainer(): void {
		this.container.style.height = "100%";
		this.container.style.minHeight = "450px";
		this.container.style.width = "100%";
		this.container.style.display = "flex";
		this.container.style.flexDirection = "column";
		this.container.style.alignItems = "center";
		this.container.style.overflow = "hidden";
		this.container.style.boxSizing = "border-box";
		this.container.style.position = "relative";
	}

	/** Creates the embedded iframe that hosts rendered HTML content. */
	private createIframe(): void {
		this.iframe = document.createElement("iframe");
		this.iframe.style.width = "100%";
		this.iframe.style.height = "100%";
		this.iframe.style.minHeight = "450px";
		this.iframe.style.border = "none";
		this.iframe.style.display = "block";
		this.iframe.style.overflow = "hidden";
		this.iframe.style.boxSizing = "border-box";
		this.iframe.style.transition = "opacity 0.3s ease";
		this.container.appendChild(this.iframe);
	}

	/** Shows a simple overlay spinner while loading async content. */
	public showSpinner(): void {
		if (!this.spinner) {
			this.spinner = document.createElement("div");
			this.spinner.style.position = "absolute";
			this.spinner.style.top = "0";
			this.spinner.style.left = "0";
			this.spinner.style.width = "100%";
			this.spinner.style.height = "100%";
			this.spinner.style.display = "flex";
			this.spinner.style.flexDirection = "column";
			this.spinner.style.justifyContent = "center";
			this.spinner.style.alignItems = "center";
			this.spinner.style.backgroundColor = "rgba(255, 255, 255, 0.9)";
			this.spinner.style.zIndex = "999";
			this.spinner.innerHTML = `
                <div style="border: 4px solid #f3f3f3; border-top: 4px solid #3498db; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; margin-bottom: 10px;"></div>
                <style>
                    @keyframes spin {
                        0% { transform: rotate(0deg);}
                        100% { transform: rotate(360deg);}
                    }
                </style>`;
			this.container.appendChild(this.spinner);
		}
	}

	/** Hides the spinner overlay when loading completes. */
	public hideSpinner(): void {
		if (this.spinner) {
			this.container.removeChild(this.spinner);
			this.spinner = null;
		}
	}

	/** Sets the iframe's srcdoc to the given HTML content. */
	public setContent(content: string): void {
		this.iframe.srcdoc = content;
	}

	/** Applies an explicit height to the iframe. */
	public setHeight(height: string): void {
		this.iframe.style.height = height;
	}

	/**
	 * Returns HTML for a styled anchor that downloads the provided URL with the given file name.
	 */
	public createDownloadButton(url: string, fileName: string): string {
		const downloadLabel = this.context.resources.getString("Label_Download").replace("{0}", fileName);
		return `
            <div style="margin-top:16px;text-align:center;">
                <a href="${url}" download="${fileName}" style="padding:8px 16px;background:#3498db;color:#fff;border-radius:4px;text-decoration:none;">${downloadLabel}</a>
            </div>`;
	}
}
