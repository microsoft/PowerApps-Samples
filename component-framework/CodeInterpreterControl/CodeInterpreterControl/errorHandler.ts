import { IInputs } from "./generated/ManifestTypes";

export class ErrorHandler {
    private context: ComponentFramework.Context<IInputs>;

    constructor(context: ComponentFramework.Context<IInputs>) {
        this.context = context;
    }

    public getErrorMessage(status?: number, originalMessage?: string): string {
        if (!status) {
            return this.context.resources.getString("Error_Unexpected").replace("{0}", originalMessage || "Unknown error");
        }

        switch (status) {
            case 401:
                return this.context.resources.getString("Error_Auth_Failed");
            case 404:
                return this.context.resources.getString("Error_Model_NotFound");
            case 403:
                return this.context.resources.getString("Error_Access_Denied");
            case 400:
                return this.context.resources.getString("Error_Bad_Request");
            case 500:
                return this.context.resources.getString("Error_Server_Error");
            case 0:
                return this.context.resources.getString("Error_Network_Failed");
            default:
                if (status >= 500) {
                    return this.context.resources.getString("Error_Server_Generic");
                } else if (status >= 400) {
                    return this.context.resources.getString("Error_Client_Generic").replace("{0}", status.toString());
                } else {
                    return `API Error (${status}): ${originalMessage || "Unknown error"}`;
                }
        }
    }

    public createErrorHtml(message: string): string {
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