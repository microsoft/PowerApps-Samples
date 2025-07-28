import { IInputs } from "./generated/ManifestTypes";

declare const Xrm: {
    Utility?: {
        getGlobalContext?: () => {
            getClientUrl: () => string;
        };
    };
};

export interface ValidationResult {
    isValid: boolean;
    errorMessage?: string;
    baseUrl?: string;
    modelId?: string;
    recordId?: string;
}

export class ValidationHandler {
    private context: ComponentFramework.Context<IInputs>;
    private guidRegex = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

    constructor(context: ComponentFramework.Context<IInputs>) {
        this.context = context;
    }

    public validateInputs(): ValidationResult {
        const errors: string[] = [];
        let baseUrl = "";

        // Validate XRM context and get base URL
        if (typeof Xrm !== "undefined" && Xrm?.Utility?.getGlobalContext) {
            const globalContext = Xrm.Utility.getGlobalContext();
            baseUrl = globalContext.getClientUrl();
        } else {
            errors.push(this.context.resources.getString("Error_Preview_Required"));
        }

        // Validate Model ID
        const modelId = this.context.parameters.modelId.raw ?? "";
        if (!modelId || !this.guidRegex.test(modelId)) {
            errors.push(this.context.resources.getString("Error_ModelId_Required"));
        }

        // Validate Record ID
        const recordId = this.context.parameters.entityId.raw ?? "";
        if (!recordId || !this.guidRegex.test(recordId)) {
            errors.push(this.context.resources.getString("Error_RecordId_Required"));
        }

        if (errors.length > 0) {
            return {
                isValid: false,
                errorMessage: errors.join("<br><br>")
            };
        }

        return {
            isValid: true,
            baseUrl,
            modelId,
            recordId
        };
    }
}