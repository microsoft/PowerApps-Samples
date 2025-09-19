import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { retrievePromptResponse, RetrievePromptResponseOutput } from "./retrievePromptResponse";
import { FilePreviewHandler } from "./filePreviewHandler";
import { ErrorHandler } from "./errorHandler";
import { ValidationHandler } from "./validationHandler";
import { UIManager } from "./uiManager";

declare const Xrm: {
	Utility?: {
		getGlobalContext?: () => {
			getClientUrl: () => string;
		};
	};
};

/**
 * Minimal shape of a file payload returned by the prediction service.
 */
interface FileOutput {
	base64_content?: string;
	content_type: string;
	file_name?: string;
}

/**
 * PCF control that invokes a prompt-based model and renders either text output or a file preview.
 */
export class CodeInterpreterControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	private context: ComponentFramework.Context<IInputs>;
	private uiManager: UIManager;
	private filePreviewHandler: FilePreviewHandler;
	private errorHandler: ErrorHandler;
	private validationHandler: ValidationHandler;

	/** Initializes the control and kicks off the initial content load. */
	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	): void {
		this.context = context;
		this.context.mode.trackContainerResize(true);

		this.uiManager = new UIManager(context, container);
		this.filePreviewHandler = new FilePreviewHandler(context);
		this.errorHandler = new ErrorHandler(context);
		this.validationHandler = new ValidationHandler(context);

		this.loadContent();
	}

	/** Refreshes the view when inputs change. */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this.context = context;
		this.loadContent();
	}

	/** No outputs are produced by this control. */
	public getOutputs(): IOutputs {
		return {};
	}

	/** Cleanup hook if needed by the control lifecycle. */
	public destroy(): void {
		// Cleanup if necessary
	}

	/**
	 * Validates inputs, calls the prediction API, and renders the resulting output.
	 */
	private async loadContent(): Promise<void> {
		this.uiManager.setContent("");

		// Validate inputs
		const validation = this.validationHandler.validateInputs();
		if (!validation.isValid) {
			this.uiManager.setContent(this.errorHandler.createErrorHtml(validation.errorMessage!));
			return;
		}

		this.uiManager.showSpinner();

		try {
			const response = await retrievePromptResponse({
				baseUrl: validation.baseUrl!,
				modelId: validation.modelId!,
				requestInputs: { RecordId: validation.recordId! },
			});

			this.uiManager.hideSpinner();
			await this.handleResponse(response);
		} catch (error: unknown) {
			this.uiManager.hideSpinner();
			this.handleError(error);
		}
	}

	/** Handles the overall response shape from the prediction API. */
	private async handleResponse(data: RetrievePromptResponseOutput): Promise<void> {
		// Check for API errors
		if (data.error) {
			const errorMessage = this.errorHandler.getErrorMessage(data.error.status, data.error.message);
			this.uiManager.setContent(this.errorHandler.createErrorHtml(errorMessage));
			return;
		}

		// Handle file output
		const file = data?.responsev2?.predictionOutput?.files?.[0];
		if (file) {
			await this.handleFileOutput(file);
			return;
		}

		// Handle text output
		this.handleTextOutput(data?.responsev2?.predictionOutput?.structuredOutput?.text);
	}

	/** Builds and renders a file preview and a download button. */
	private async handleFileOutput(file: FileOutput): Promise<void> {
		const preview = await this.filePreviewHandler.generatePreview(
			file.base64_content ?? "",
			file.content_type,
			file.file_name ?? ""
		);

		const downloadButton = this.uiManager.createDownloadButton(preview.downloadUrl, preview.fileName);
		this.uiManager.setContent(`${preview.previewHtml}${downloadButton}`);
	}

	/** Renders plain text output or an error when no text is present. */
	private handleTextOutput(text?: string): void {
		if (text) {
			this.uiManager.setContent(text);
		} else {
			const errorMessage = this.context.resources.getString("Error_Empty_Output");
			this.uiManager.setContent(this.errorHandler.createErrorHtml(errorMessage));
		}
		this.uiManager.setHeight(`${this.context.mode.allocatedHeight - 8}px`);
	}

	/** Normalizes unknown errors and renders a friendly message. */
	private handleError(error: unknown): void {
		let status: number | undefined;
		let message: string;

		if (error && typeof error === "object" && "status" in error) {
			status = typeof error.status === "number" ? error.status : undefined;
		}

		if (error && typeof error === "object" && "message" in error) {
			message = String(error.message);
		} else {
			message = String(error);
		}

		const errorMessage = this.errorHandler.getErrorMessage(status, message);
		this.uiManager.setContent(this.errorHandler.createErrorHtml(errorMessage));
	}
}
