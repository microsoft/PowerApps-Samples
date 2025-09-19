import * as mammoth from "mammoth";
import * as XLSX from "xlsx";
import { IInputs } from "./generated/ManifestTypes";

/**
 * Result of generating a preview for a file returned by the service.
 */
export interface FilePreviewResult {
	previewHtml: string;
	downloadUrl: string;
	fileName: string;
}

/**
 * Handles generating HTML previews and download links for common file types.
 */
export class FilePreviewHandler {
	private context: ComponentFramework.Context<IInputs>;

	constructor(context: ComponentFramework.Context<IInputs>) {
		this.context = context;
	}

	/**
	 * Builds a preview and download URL for the provided file content.
	 * @param base64Content Base64-encoded file content.
	 * @param mimeType MIME type of the file.
	 * @param fileName Suggested download file name.
	 * @returns Preview HTML, download URL, and final file name.
	 */
	public async generatePreview(base64Content: string, mimeType: string, fileName: string): Promise<FilePreviewResult> {
		const byteCharacters = atob(base64Content);
		const byteNumbers = new Array(byteCharacters.length);
		for (let i = 0; i < byteCharacters.length; i++) {
			byteNumbers[i] = byteCharacters.charCodeAt(i);
		}
	const byteArray = new Uint8Array(byteNumbers);
	const arrayBuffer = byteArray.buffer instanceof ArrayBuffer ? byteArray.buffer : new Uint8Array(byteArray).buffer;
	const blob = new Blob([arrayBuffer], { type: mimeType });
		const downloadUrl = URL.createObjectURL(blob);

		const previewHtml = await this.generatePreviewHtml(byteArray, mimeType);

		return {
			previewHtml,
			downloadUrl,
			fileName: fileName || `output.${mimeType.split("/")[1] || "file"}`,
		};
	}

	/**
	 * Chooses a preview strategy based on the MIME type.
	 * @param byteArray Raw file bytes.
	 * @param mimeType MIME type to preview.
	 */
	private async generatePreviewHtml(byteArray: Uint8Array, mimeType: string): Promise<string> {
		switch (mimeType) {
			case "application/pdf":
				return this.generatePdfPreview(byteArray);

			case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
			case "application/msword":
				return await this.generateWordPreview(byteArray);

			case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
			case "application/vnd.ms-excel":
				return this.generateExcelPreview(byteArray);

			case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
			case "application/vnd.ms-powerpoint":
				return this.generateErrorMessage(this.context.resources.getString("Preview_PowerPoint_NotSupported"));

			default:
				return this.generateErrorMessage(this.context.resources.getString("Preview_FileType_NotAvailable"));
		}
	}

	/**
	 * Renders a simple iframe-based preview for PDF files.
	 */
	private generatePdfPreview(byteArray: Uint8Array): string {
	const arrayBuffer = byteArray.buffer instanceof ArrayBuffer ? byteArray.buffer : new Uint8Array(byteArray).buffer;
	const blob = new Blob([arrayBuffer], { type: "application/pdf" });
		const url = URL.createObjectURL(blob);
		return `<iframe src="${url}" style="width:100%;height:400px;border:none;"></iframe>`;
	}

	/**
	 * Converts a Word document into HTML using Mammoth and wraps it in a scrollable container.
	 */
	private async generateWordPreview(byteArray: Uint8Array): Promise<string> {
		try {
			const arrayBuffer = byteArray.buffer instanceof ArrayBuffer ? byteArray.buffer : new Uint8Array(byteArray).buffer;
			const result = await mammoth.convertToHtml({ arrayBuffer: arrayBuffer as ArrayBuffer });
			return `
                <div style="padding:20px;max-width:800px;margin:0 auto;height:400px;overflow-y:auto;border:1px solid #ddd;background:white;">
                    ${result.value}
                </div>`;
		} catch (error) {
			const message = error instanceof Error ? error.message : String(error);
			return this.generateErrorMessage(this.context.resources.getString("Preview_Word_Failed").replace("{0}", message));
		}
	}

	/**
	 * Parses the first worksheet and renders it as HTML along with basic metadata.
	 */
	private generateExcelPreview(byteArray: Uint8Array): string {
		try {
			const workbook = XLSX.read(byteArray, { type: "array" });

			const sheetName = workbook.SheetNames[0];
			const worksheet = workbook.Sheets[sheetName];

			const range = XLSX.utils.decode_range(worksheet["!ref"] || "A1:A1");
			const totalCells = (range.e.r + 1) * (range.e.c + 1);

			const htmlString = XLSX.utils.sheet_to_html(worksheet);

			let sheetInfo = `
                <div style="background:#f8f9fa;padding:10px;border-bottom:1px solid #ddd;font-size:11px;color:#666;">
                    <strong>${this.context.resources.getString("Label_Sheet")}</strong> ${sheetName} 
                    <span style="margin-left:15px;"><strong>${this.context.resources.getString("Label_Range")}</strong> ${worksheet["!ref"] || "Empty"}</span>
                    <span style="margin-left:15px;"><strong>${this.context.resources.getString("Label_Cells")}</strong> ${totalCells}</span>
            `;

			if (workbook.SheetNames.length > 1) {
				sheetInfo += `
                    <br style="margin:4px 0;">
                    <strong>${this.context.resources.getString("Label_Available_Sheets")}</strong> ${workbook.SheetNames.join(", ")}
                `;
			}

			sheetInfo += `</div>`;

			return `
                <div style="height:400px;border:1px solid #ddd;background:white;display:flex;flex-direction:column;">
                    ${sheetInfo}
                    <div style="flex:1;overflow:auto;padding:0;">
                        ${htmlString}
                    </div>
                </div>`;
		} catch (error) {
			const message = error instanceof Error ? error.message : String(error);
			return this.generateErrorMessage(
				this.context.resources.getString("Preview_Excel_Failed").replace("{0}", message)
			);
		}
	}

	/**
	 * Returns a minimal error HTML layout for preview failures.
	 */
	private generateErrorMessage(message: string): string {
		return `
            <html>
                <head>
                    <style>
                        body { 
                            margin: 0; 
                            padding: 20px; 
                            height: 100vh;
                            overflow: hidden;
                            display: flex;
                            flex-direction: column;
                            justify-content: center;
                            align-items: center;
                            font-family: 'Segoe UI Variable Text', sans-serif;
                            text-align: center;
                            box-sizing: border-box;
                        }
                        .message {
                            max-width: 80%;
                            line-height: 1.5;
                            color: #666;
                        }
                    </style>
                </head>
                <body>
                    <div class="message">${message}</div>
                </body>
            </html>`;
	}
}
